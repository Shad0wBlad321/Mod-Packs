using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.AI;
using Verse.Sound;
using Random = UnityEngine.Random;

namespace GeneRipper;

[StaticConstructorOnStartup]
public class Building_GeneRipper : Building_Enterable, IThingHolderWithDrawnPawn
{
    private static GeneRipperSettings? _settings;
    private static GeneRipperSettings Settings => _settings ??= LoadedModManager.GetMod<GeneRipperMod>().GetSettings<GeneRipperSettings>();

    private const float WorkingPowerUsageFactor = 4f;

    private static readonly Texture2D CancelIcon = ContentFinder<Texture2D>.Get("UI/Designators/Cancel");

    [Unsaved] private Texture2D? cachedInsertPawnTex;

    [Unsaved] private CompPowerTrader? cachedPowerComp;

    [Unsaved] private Effecter? progressBar;

    private GeneDef? selectedGene;

    [Unsaved] private Sustainer? sustainerWorking;

    private int ticksRemaining;

    private Pawn? ContainedPawn
    {
        get
        {
            if (innerContainer.Count <= 0) return null;

            return (Pawn)innerContainer[0];
        }
    }

    public bool PowerOn => PowerTraderComp.PowerOn;

    private CompPowerTrader PowerTraderComp => cachedPowerComp ??= this.TryGetComp<CompPowerTrader>();

    public Texture2D InsertPawnTex
    {
        get
        {
            if (cachedInsertPawnTex == null) cachedInsertPawnTex = ContentFinder<Texture2D>.Get("UI/Gizmos/InsertPawn");

            return cachedInsertPawnTex;
        }
    }

    private const float ProgressBarOffsetZ = -0.8f;

    public float HeldPawnDrawPos_Y => this.DrawPos.y + 0.04054054f;

    public float HeldPawnBodyAngle => this.Rotation.AsAngle;

    public PawnPosture HeldPawnPosture => PawnPosture.LayingOnGroundFaceUp;

    public override Vector3 PawnDrawOffset => Vector3.zero;

    

    public override void PostPostMake()
    {
        if (!ModLister.CheckBiotech("gene extractor"))
            Destroy();
        else
            base.PostPostMake();
    }

    public override void DeSpawn(DestroyMode mode = DestroyMode.Vanish)
    {
        sustainerWorking = null;
        if (progressBar != null)
        {
            progressBar.Cleanup();
            progressBar = null;
        }

        base.DeSpawn(mode);
    }

    public override void Tick()
    {
        base.Tick();
        innerContainer.ThingOwnerTick();
        if (this.IsHashIntervalTick(250))
        {
            var num = Working ? WorkingPowerUsageFactor : 1f;
            PowerTraderComp.PowerOutput = (0f - PowerComp.Props.PowerConsumption) * num;
        }

        if (Working && PowerTraderComp.PowerOn)
        {
            TickEffects();
            if (PowerOn) ticksRemaining--;

            if (ticksRemaining <= 0) Finish();
        }
        else if (progressBar != null)
        {
            progressBar.Cleanup();
            progressBar = null;
        }
    }

    private void TickEffects()
    {
        if (sustainerWorking == null || sustainerWorking.Ended)
            sustainerWorking =
                SoundDefOf.GeneExtractor_Working.TrySpawnSustainer(SoundInfo.InMap(this, MaintenanceType.PerTick));
        else
            sustainerWorking.Maintain();

        progressBar ??= EffecterDefOf.ProgressBarAlwaysVisible.Spawn();

        progressBar.EffectTick(new TargetInfo(Position + IntVec3.North.RotatedBy(Rotation), Map), TargetInfo.Invalid);
        var mote = ((SubEffecter_ProgressBar)progressBar.children[0]).mote;
        if (mote != null)
        {
            mote.progress = 1f - Mathf.Clamp01((float)ticksRemaining / (Settings.ExtractionTicks));
            mote.offsetZ = ProgressBarOffsetZ;
        }
    }

    public override AcceptanceReport CanAcceptPawn(Pawn pawn)
    {
        if (!pawn.IsColonist && !pawn.IsSlaveOfColony && !pawn.IsPrisonerOfColony)
            return false;
        if (selectedPawn != null && selectedPawn != pawn)
            return false;
        if (!pawn.RaceProps.Humanlike || pawn.IsQuestLodger())
            return false;
        if (!PowerOn)
            return "NoPower".Translate().CapitalizeFirst();
        if (innerContainer.Count > 0)
            return "Occupied".Translate();
        if (pawn.genes == null || !pawn.genes.GenesListForReading.Any<Gene>())
            return "PawnHasNoGenes".Translate(pawn.Named("PAWN"));
        if(!pawn.ageTracker.Adult)
            return "GeneRipper_CantExtractFromChildren".Translate(pawn.Named("PAWN"));
        return pawn.health.hediffSet.HasHediff(HediffDefOf.XenogermReplicating)
            ? "GeneRipper_CurrentlyRegenerating".Translate()
            : true;
    }

    private void Cancel()
    {
        startTick = -1;
        selectedPawn = null;
        selectedGene = null;
        sustainerWorking = null;
        if (ContainedPawn == null)
            return;

        if (new FloatRange(0, Settings.ExtractionTicks).RandomInRange > ticksRemaining)
        {
            KillOccupant(ContainedPawn);
        }
        else
        {
            GeneUtility.ExtractXenogerm(ContainedPawn, Mathf.RoundToInt(60 * (Settings.ExtractionTicks - ticksRemaining)));
            innerContainer.TryDropAll(def.hasInteractionCell ? InteractionCell : Position, Map, ThingPlaceMode.Near);
        }

    }

    private void Finish()
    {
        if (selectedGene is null)
        {
            Cancel();
            return;
        }

        startTick = -1;
        selectedPawn = null;
        sustainerWorking = null;
        if (ContainedPawn == null)
            return;
        var containedPawn = ContainedPawn;
        var genesToAdd = new List<GeneDef> { selectedGene };

        selectedGene = null;

        var genePack = (Genepack)ThingMaker.MakeThing(ThingDefOf.Genepack);

        genePack.Initialize(genesToAdd);

        var intVec3 = def.hasInteractionCell ? InteractionCell : Position;
        GenPlace.TryPlaceThing(genePack, intVec3, Map, ThingPlaceMode.Near);

        KillOccupant(containedPawn);

        Messages.Message(
            (string)("GeneExtractionComplete".Translate(containedPawn.Named("PAWN")) + ": " +
                     genesToAdd.Select((Func<GeneDef, string>)(x => x.label)).ToCommaList().CapitalizeFirst()),
            new LookTargets((TargetInfo)(Thing)containedPawn, (TargetInfo)(Thing)genePack),
            MessageTypeDefOf.PositiveEvent);
    }

    private void KillOccupant(Pawn occupant)
    {
       
        var intVec3 = def.hasInteractionCell ? InteractionCell : Position;
        var dInfo = new DamageInfo(DamageDefOf.ExecutionCut, 9999f, 999f,
            hitPart: occupant.health.hediffSet.GetBrain());
        dInfo.SetAllowDamagePropagation(false);
        occupant.forceNoDeathNotification = true;
        occupant.Kill(dInfo);
        occupant.forceNoDeathNotification = false;
        if (new FloatRange(0, 1).RandomInRange <= Settings.BlendingChance)
        {
            innerContainer.TryDropAll(intVec3, Map, ThingPlaceMode.Near, (thing, i) =>
            {
                thing.DeSpawn();
            });
            Thing paste = ThingMaker.MakeThing(ThingDefOf.MealNutrientPaste);
            CompIngredients comp = paste.TryGetComp<CompIngredients>();
            comp.RegisterIngredient(ThingDefOf.Meat_Human);
            paste.stackCount = new IntRange(5, 10).RandomInRange;
            GenPlace.TryPlaceThing(paste, intVec3, Map, ThingPlaceMode.Near);

            Thing meat = ThingMaker.MakeThing(ThingDefOf.Meat_Human);
            meat.stackCount = new IntRange(20, 50).RandomInRange;
            GenPlace.TryPlaceThing(meat, intVec3, Map, ThingPlaceMode.Near);
            IntRange spreadRange = new(-2, 2);
            for (int i = 0; i < 5; i++)
            {

                IntVec3 result;
                if (this.Map == null || !CellFinder.TryFindRandomReachableCellNear(intVec3, this.Map, 2, TraverseParms.For(TraverseMode.NoPassClosedDoors), (Predicate<IntVec3>)(x => x.Standable(this.Map)), (Predicate<Region>)(x => true), out result))
                    return;
                FilthMaker.TryMakeFilth(result, this.Map, ThingDefOf.Filth_Blood);
            }

        }
        else
        {
            
            innerContainer.TryDropAll(intVec3, Map, ThingPlaceMode.Near);

        }

        ThoughtUtility.GiveThoughtsForPawnExecuted(occupant, null, PawnExecutionKind.GenericBrutal);
    }


    public override void TryAcceptPawn(Pawn pawn)
    {
        if (!CanAcceptPawn(pawn))
            return;
        selectedPawn = pawn;
        var num = pawn.DeSpawnOrDeselect() ? 1 : 0;
        if (innerContainer.TryAddOrTransfer(pawn))
        {
            startTick = Find.TickManager.TicksGame;
            ticksRemaining = Settings.ExtractionTicks;
        }

        if (num == 0)
            return;
        Find.Selector.Select(pawn, false, false);
    }

    protected override void SelectPawn(Pawn pawn)
    {
        Find.WindowStack.Add(new Dialog_SelectGene(pawn, (p, g) =>
        {
            Find.WindowStack.Add(Dialog_MessageBox.CreateConfirmation("GeneRipper_KillConfirmation".Translate(p.NameShortColored, g.label),
                () =>
                {
                    selectedGene = g;
                    base.SelectPawn(p);
                }, true));
            
        }));
    }

    public override IEnumerable<FloatMenuOption> GetFloatMenuOptions(
        Pawn selPawn)
    {
        var buildingGeneRipper = this;
        foreach (var floatMenuOption in base.GetFloatMenuOptions(selPawn))
            yield return floatMenuOption;
        if (!selPawn.CanReach((LocalTargetInfo)buildingGeneRipper, PathEndMode.InteractionCell, Danger.Deadly))
        {
            yield return new FloatMenuOption(
                (string)("CannotEnterBuilding".Translate((NamedArgument)buildingGeneRipper) + ": " +
                         "NoPath".Translate().CapitalizeFirst()), null);
        }
        else
        {
            var acceptanceReport = buildingGeneRipper.CanAcceptPawn(selPawn);
            if (acceptanceReport.Accepted)
                yield return FloatMenuUtility.DecoratePrioritizedTask(
                    new FloatMenuOption((string)"EnterBuilding".Translate((NamedArgument)buildingGeneRipper),
                        (() => SelectPawn(selPawn))), selPawn, (LocalTargetInfo)buildingGeneRipper);
            else if (buildingGeneRipper.SelectedPawn == selPawn && !selPawn.IsPrisonerOfColony)
                yield return FloatMenuUtility.DecoratePrioritizedTask(
                    new FloatMenuOption((string)"EnterBuilding".Translate((NamedArgument)buildingGeneRipper),
                        () =>
                            selPawn.jobs.TryTakeOrderedJob(JobMaker.MakeJob(JobDefOf.EnterBuilding,
                                (LocalTargetInfo)(Thing)this))), selPawn, (LocalTargetInfo)buildingGeneRipper);
            else if (!acceptanceReport.Reason.NullOrEmpty())
                yield return new FloatMenuOption(
                    (string)("CannotEnterBuilding".Translate((NamedArgument)buildingGeneRipper) + ": " +
                             acceptanceReport.Reason.CapitalizeFirst()), null);
        }
    }

    public override IEnumerable<Gizmo> GetGizmos()
    {
        var buildingGeneRipper = this;
        foreach (var gizmo in base.GetGizmos())
            yield return gizmo;
        if (buildingGeneRipper.Working)
        {
            var commandAction1 = new Command_Action
            {
                defaultLabel = "CommandCancelExtraction".Translate(),
                defaultDesc = "CommandCancelExtractionDesc".Translate(),
                icon = CancelIcon,
                action = buildingGeneRipper.Cancel,
                activateSound = SoundDefOf.Designate_Cancel
            };
            yield return commandAction1;
            if (DebugSettings.ShowDevGizmos)
            {
                var commandAction2 = new Command_Action
                {
                    defaultLabel = "DEV: Finish extraction",
                    action = buildingGeneRipper.Finish
                };
                yield return commandAction2;
            }
        }
        else if (buildingGeneRipper.selectedPawn != null)
        {
            var commandAction = new Command_Action
            {
                defaultLabel = "CommandCancelLoad".Translate(),
                defaultDesc = "CommandCancelLoadDesc".Translate(),
                icon = CancelIcon,
                activateSound = SoundDefOf.Designate_Cancel,
                action = buildingGeneRipper.Cancel
            };
            yield return commandAction;
        }
        else
        {
            var commandAction = new Command_Action
            {
                defaultLabel = "InsertPerson".Translate() + "...",
                defaultDesc = "InsertPersonGeneExtractorDesc".Translate(),
                icon = buildingGeneRipper.InsertPawnTex,
                action = delegate
                {
                    var list = new List<FloatMenuOption>();
                    foreach (var item in Map.mapPawns.AllPawnsSpawned)
                    {
                        var pawn = item;
                        var acceptanceReport = CanAcceptPawn(item);
                        if (!acceptanceReport.Accepted)
                        {
                            if (!acceptanceReport.Reason.NullOrEmpty())
                                list.Add(new FloatMenuOption(item.LabelShortCap + ": " + acceptanceReport.Reason, null,
                                    pawn, Color.white));
                        }
                        else
                        {
                            list.Add(new FloatMenuOption(item.LabelShortCap + ", " + pawn.genes.XenotypeLabelCap,
                                delegate { SelectPawn(pawn); }, pawn, Color.white));
                        }
                    }

                    if (!list.Any()) list.Add(new FloatMenuOption("NoExtractablePawns".Translate(), null));

                    Find.WindowStack.Add(new FloatMenu(list));
                }
            };
            if (!buildingGeneRipper.PowerOn)
                commandAction.Disable((string)"NoPower".Translate().CapitalizeFirst());
            yield return commandAction;
        }
    }

    public override void Draw()
    {
        base.Draw();
        if (!Working || selectedPawn == null || !innerContainer.Contains(selectedPawn))
            return;
        selectedPawn.Drawer.renderer.RenderPawnAt(DrawPos + PawnDrawOffset, neverAimWeapon: true);
    }

    public override string GetInspectString()
    {
        var str1 = base.GetInspectString();
        if (selectedPawn != null && innerContainer.Count == 0)
        {
            if (!str1.NullOrEmpty())
                str1 += "\n";
            str1 += "WaitingForPawn".Translate(selectedPawn.Named("PAWN")).Resolve();
        }
        else if (Working && ContainedPawn != null)
        {
            if (!str1.NullOrEmpty())
                str1 += "\n";
            var str2 = str1 + "ExtractingXenogermFrom".Translate(ContainedPawn.Named("PAWN")).Resolve() + "\n";
            str1 = !PowerOn
                ? (string)(str2 + "ExtractionPausedNoPower".Translate())
                : str2 + "DurationLeft".Translate((NamedArgument)ticksRemaining.ToStringTicksToPeriod()).Resolve();
        }

        return str1;
    }

    public override void ExposeData()
    {
        base.ExposeData();
        Scribe_Values.Look(ref ticksRemaining, "ticksRemaining");
        Scribe_Defs.Look(ref selectedGene, "selectedGene");
    }
}