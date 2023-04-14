using RimDeed.Dialogs;
using RimWorld;
using RimWorld.Planet;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace Rimdeed
{
    public class Applicants : IExposable
    {
        public List<Pawn> applicants;
        public Dictionary<int, string> greetings;
        public int initTime;
        public Map map;
        public bool isGold;

        public Applicants()
        {
            applicants = new List<Pawn>();
            greetings = new Dictionary<int, string>();
        }
        public Applicants(Map map, List<Pawn> pawns)
        {
            applicants = pawns;
            this.map = map;
            greetings = new Dictionary<int, string>();
            foreach (Pawn pawn in pawns)
            {
                greetings[pawn.thingIDNumber] = RimdeedTools.GenerateTextFromRule(RD_DefOf.RD_Greetings, pawn.thingIDNumber);
            }
            initTime = Find.TickManager.TicksGame;
        }

        public void ExposeData()
        {
            Scribe_Collections.Look(ref applicants, "applicants", LookMode.Deep);
            Scribe_Collections.Look(ref greetings, "greetings", LookMode.Value, LookMode.Value);
            Scribe_References.Look(ref map, "map");
            Scribe_Values.Look(ref initTime, "initTime");
            Scribe_Values.Look(ref isGold, "isGold");
        }
    }
    public class RimdeedManager : WorldComponent
    {
        public Dictionary<Letter, Applicants> lettersWithApplicants;
        public List<Applicants> unProcessedApplicants;
        public List<Pawn> unProcessedRecruiters;
        public Dictionary<Pawn, int> hiredPawns;

        public static RimdeedManager Instance;
        public int freeTrialTicks = 0;
        public bool trialExpired;
        public int complaintResponceTick;

        private int bannedUntil;

        public int BannedUntil
        {
            get
            {
                if (RimdeedSettings.resetBan)
                {
                    bannedUntil = 0;
                    RimdeedSettings.resetBan = false;
                }
                return bannedUntil;
            }
            set => bannedUntil = value;
        }
        public PawnTrader pawnTrader;
        public RimdeedManager(World world) : base(world)
        {
            Instance = this;
        }
        public override void FinalizeInit()
        {
            Instance = this;
            base.FinalizeInit();
            lettersWithApplicants ??= new Dictionary<Letter, Applicants>();
            unProcessedApplicants ??= new List<Applicants>();
            unProcessedRecruiters ??= new List<Pawn>();
            hiredPawns ??= new Dictionary<Pawn, int>();
            pawnTrader ??= new PawnTrader();
        }
        public override void WorldComponentTick()
        {
            base.WorldComponentTick();
            if (trialExpired && Find.TickManager.TicksGame > freeTrialTicks)
            {
                trialExpired = false;
            }
        }
        public void RegisterNewOrder(Map map, int applicantCount, bool goldenOrder = false)
        {
            List<Pawn> pawns = new List<Pawn>();
            for (int i = 0; i < applicantCount; i++)
            {
                bool factionValidator(FactionDef f)
                {
                    if (f != null)
                    {
                        if (RimdeedSettings.disallowNeolithicFaction && f.techLevel == TechLevel.Neolithic)
                        {
                            return false;
                        }
                        if (RimdeedSettings.disallowSpacerFaction && f.techLevel == TechLevel.Spacer)
                        {
                            return false;
                        }
                        return !RimdeedSettings.disallowUltraFaction || f.techLevel != TechLevel.Ultra;
                    }
                    return true;
                }

                bool raceValidator(ThingDef race)
                {
                    return !(RimdeedSettings.forbiddenRaces?.Contains(race.defName) ?? false);
                }

                if (DefDatabase<PawnKindDef>.AllDefs.Where(x => x.RaceProps.Humanlike && factionValidator(x.defaultFactionType)
                    && raceValidator(x.race)).TryRandomElement(out PawnKindDef kind))
                {
                    Faction faction = kind.defaultFactionType != null
                        ? Find.FactionManager.FirstFactionOfDef(kind.defaultFactionType)
                        : Find.FactionManager.AllFactionsVisible.Where(x => x.def.humanlikeFaction && factionValidator(x.def)).RandomElement();
                    int attempts = 0;
                    Pawn pawn = null;

                    while (attempts < 100)
                    {
                        attempts++;
                        pawn = PawnGenerator.GeneratePawn(kind, faction);
                        if (goldenOrder)
                        {
                            if (pawn.ageTracker.AgeBiologicalYears > 35)
                            {
                                continue;
                            }
                            for (int num = pawn.health.hediffSet.hediffs.Count - 1; num >= 0; num--)
                            {
                                if (pawn.health.hediffSet.hediffs[num].def.isBad)
                                {
                                    pawn.health.hediffSet.hediffs.RemoveAt(num);
                                }
                            }
                        }

                        if (RimdeedSettings.ageRestriction.Includes(pawn.ageTracker.AgeBiologicalYears))
                        {
                            break;
                        }
                    }

                    pawn.relations.everSeenByPlayer = true;
                    PawnComponentsUtility.AddComponentsForSpawn(pawn);
                    pawns.Add(pawn);
                }
            }

            Applicants newApplicants = new Applicants(map, pawns);
            if (goldenOrder)
            {
                newApplicants.isGold = true;
            }
            IncidentParms parms = StorytellerUtility.DefaultParmsNow(IncidentCategoryDefOf.Misc, map);
            if (goldenOrder)
            {
                newApplicants.isGold = true;
                Find.Storyteller.incidentQueue.Add(RD_DefOf.RD_NewOrderArrived, Find.TickManager.TicksGame + GenDate.TicksPerDay, parms);
                ChoiceLetter letter = LetterMaker.MakeLetter("RimDeed.RimdeedGoldPackagePurchase".Translate(), "RimDeed.RimdeedGoldPackagePurchaseText".Translate(), LetterDefOf.PositiveEvent);
                Find.LetterStack.ReceiveLetter(letter);
            }
            else
            {
                Find.Storyteller.incidentQueue.Add(RD_DefOf.RD_NewOrderArrived, Find.TickManager.TicksGame + (int)(GenDate.TicksPerDay * Rand.Range(1f, 3f)), parms);
                ChoiceLetter letter = LetterMaker.MakeLetter("RimDeed.RimDeedPaymentTitle".Translate(), "RimDeed.RimDeedPaymentText".Translate(), LetterDefOf.PositiveEvent);
                Find.LetterStack.ReceiveLetter(letter);
            }


            unProcessedApplicants ??= new List<Applicants>();
            unProcessedApplicants.Add(newApplicants);
        }

        public void ReceiveNewOrder(Letter letter)
        {
            if (unProcessedApplicants.Any())
            {
                Applicants newApplicants = unProcessedApplicants.First();
                unProcessedApplicants.Remove(newApplicants);
                lettersWithApplicants[letter] = newApplicants;
            }
        }

        public void RegisterNewRecruiter(Pawn pawn, Map map, bool goldOrder)
        {
            IncidentParms parms = StorytellerUtility.DefaultParmsNow(IncidentCategoryDefOf.Misc, map);
            ChoiceLetter letter;
            if (goldOrder)
            {
                Find.Storyteller.incidentQueue.Add(RD_DefOf.RD_NewRecruiterArrived, Find.TickManager.TicksGame + (int)(GenDate.TicksPerHour * Rand.Range(1f, 7f)), parms);
                letter = LetterMaker.MakeLetter("RimDeed.RimdeedConfirmation".Translate(), "RimDeed.RimdeedConfirmationText".Translate(), LetterDefOf.PositiveEvent);
            }
            else
            {
                Find.Storyteller.incidentQueue.Add(RD_DefOf.RD_NewRecruiterArrived, Find.TickManager.TicksGame + (int)(GenDate.TicksPerDay * Rand.Range(0.8f, 1f)), parms);
                letter = LetterMaker.MakeLetter("RimDeed.RimdeedConfirmation".Translate(), "RimDeed.RimdeedConfirmationText".Translate(), LetterDefOf.PositiveEvent);
            }
            Find.LetterStack.ReceiveLetter(letter);
            unProcessedRecruiters ??= new List<Pawn>();
            unProcessedRecruiters.Add(pawn);
        }
        public bool TryReceiveNewRecruiter(Map map, out Pawn newApplicant)
        {
            if (unProcessedRecruiters.Any())
            {
                newApplicant = unProcessedRecruiters.First();
                unProcessedRecruiters.Remove(newApplicant);
                newApplicant.SetFaction(Faction.OfPlayer);
                newApplicant.needs.mood.thoughts.memories.TryGainMemory(RD_DefOf.RD_IGotJob);
                hiredPawns[newApplicant] = Find.TickManager.TicksGame;
                IncidentParms incidentParms = new IncidentParms
                {
                    target = map,
                    spawnCenter = DropCellFinder.TradeDropSpot(map)
                };
                PawnsArrivalModeDef obj = PawnsArrivalModeDefOf.CenterDrop;

                obj.Worker.Arrive(new List<Pawn> { newApplicant }, incidentParms);
                return true;
            }
            newApplicant = null;
            return false;
        }
        public bool TryOpenNewLetter(Letter letter)
        {
            if (lettersWithApplicants.TryGetValue(letter, out Applicants applicants))
            {
                if (applicants.initTime + (GenDate.TicksPerDay * 30) > Find.TickManager.TicksGame)
                {
                    DiaNode diaNode = new DiaNode("RimDeed");
                    Find.WindowStack.Add(new Dialog_ApplicantsPage(diaNode, false, applicants));
                }
                else
                {
                    DiaNode diaNode = new DiaNode("RimDeed");
                    Find.WindowStack.Add(new Dialog_RimDeed_LinkExpired(diaNode, false));
                }
                return true;
            }
            return false;
        }
        public override void ExposeData()
        {
            Instance = this;
            base.ExposeData();
            Scribe_Collections.Look(ref lettersWithApplicants, "lettersWithApplicants", LookMode.Reference, LookMode.Deep, ref letterKeys, ref applicantsValues);
            Scribe_Collections.Look(ref unProcessedApplicants, "unProcessedApplicants", LookMode.Deep);
            Scribe_Collections.Look(ref unProcessedRecruiters, "unProcessedRecruiters", LookMode.Deep);
            Scribe_Collections.Look(ref hiredPawns, "hiredPawns", LookMode.Reference, LookMode.Value, ref pawnKeys, ref intValues);
            Scribe_Values.Look(ref freeTrialTicks, "freeTrialTicks");
            Scribe_Values.Look(ref trialExpired, "trialExpired");
            Scribe_Values.Look(ref complaintResponceTick, "complaintResponceTick");
            Scribe_Values.Look(ref bannedUntil, "bannedUntil");
            Scribe_Deep.Look(ref pawnTrader, "pawnTrader");
        }

        private List<Letter> letterKeys;
        private List<Applicants> applicantsValues;

        private List<Pawn> pawnKeys;
        private List<int> intValues;
    }
}
