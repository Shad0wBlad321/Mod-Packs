using RimWorld;
using RimWorld.Planet;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
using VFE_Settlers.Hediffs;

namespace VFE_Settlers.Utilities
{
    public static class UtilityEvent
    {
        public static bool TryFindSpawnSpot(Map map, out IntVec3 spawnSpot)
        {
            bool validator(IntVec3 c) => map.reachability.CanReachColony(c) && !c.Fogged(map);
            return CellFinder.TryFindRandomEdgeCellWith(validator, map, CellFinder.EdgeRoadChance_Neutral, out spawnSpot);
        }

        public static bool ProtectionFee(Map map, IncidentParms parms)
        {
            IEnumerable<Thing> silver = UtilityThing.GetSilverInHome(map);
            int count = UtilityThing.GetAmountSilverInHome(silver);

            int fee = (int)Mathf.Clamp(map.wealthWatcher.WealthTotal / 100, 50, 50000);

            if (parms.faction == null)
            {
                parms.faction = Find.FactionManager.RandomEnemyFaction(false, false, false);
            }

            DiaOption diaOptionClose = new DiaOption("OK".Translate())
            {
                resolveTree = true
            };
            DiaNode diaNodeInitial = new DiaNode("ProtectionFeeDialogue".Translate(parms.faction, fee));
            DiaNode diaNodeAgree = new DiaNode("ProtectionFeeAgreeDialogue".Translate(parms.faction));
            DiaNode diaNodeDisagree = new DiaNode("ProtectionFeeDisagreeDialogue".Translate(parms.faction));
            diaNodeAgree.options.Add(diaOptionClose);
            diaNodeDisagree.options.Add(diaOptionClose);
            DiaOption diaOptionAgree = new DiaOption("ProtectionFeeAgree".Translate())
            {
                action = delegate
                {
                    UtilityThing.RemoveSilverFromHome(silver, fee);
                },
                link = diaNodeAgree
            };
            diaNodeInitial.options.Add(diaOptionAgree);
            DiaOption diaOptionDisagree = new DiaOption("ProtectionFeeDisagree".Translate())
            {
                action = delegate
                {
                    parms.points = StorytellerUtility.DefaultThreatPointsNow(parms.target);
                    Find.Storyteller.incidentQueue.Add(IncidentDefOf.RaidEnemy, Find.TickManager.TicksGame + Rand.Range(5000, 30000), parms, 90000);
                },
                link = diaNodeDisagree
            };
            diaNodeInitial.options.Add(diaOptionDisagree);
            if (count < fee)
            {
                diaOptionAgree.Disable("ProtectionFeeNotEnoughSilver".Translate());
            }
            Find.WindowStack.Add(new Dialog_NodeTree(diaNodeInitial, delayInteractivity: true, radioMode: true, "ProtectionFeeLabel".Translate()));
            Find.Archive.Add(new ArchivedDialog(diaNodeInitial.text, "ProtectionFeeLabel".Translate()));
            return true;
        }

        public static bool DrunkMuffalo(Map map)
        {
            if (!TryFindSpawnSpot(map, out IntVec3 spawnSpot))
            {
                return false;
            }
            IntVec3 loc = CellFinder.RandomClosewalkCellNear(spawnSpot, map, 10);
            Pawn muffalo = null;
            for (int i = 0; i <= Rand.RangeInclusive(2, 6); i++)
            {
                muffalo = PawnGenerator.GeneratePawn(PawnKindDefOf.Muffalo);
                GenSpawn.Spawn(muffalo, loc, map, Rot4.Random);
                muffalo.needs.SetInitialLevels();
                Hediff dif = HediffMaker.MakeHediff(Defs.HediffDefOf.Chemshined, muffalo);
                muffalo.health.AddHediff(dif);
                dif.Severity = (float)Rand.RangeInclusive(50, 99) / 100f;
            }
            Find.LetterStack.ReceiveLetter("DrunkMuffaloTitle".Translate(), "DrunkMuffaloMessage".Translate(), LetterDefOf.NeutralEvent, muffalo);
            return true;
        }

        public static bool TryFindTile(out int tile)
        {
            return TileFinder.TryFindNewSiteTile(out tile, 7, 27);
        }

        public static bool TryFindFactions(out Faction alliedFaction, out Faction enemyFaction)
        {
            if (Find.FactionManager.AllFactions.Where((Faction x) => !x.def.hidden && !x.defeated && !x.IsPlayer && !x.HostileTo(Faction.OfPlayer) && CommonHumanlikeEnemyFactionExists(Faction.OfPlayer, x) && !AnyQuestExistsFrom(x)).TryRandomElement(out alliedFaction))
            {
                enemyFaction = CommonHumanlikeEnemyFaction(Faction.OfPlayer, alliedFaction);
                return true;
            }
            alliedFaction = null;
            enemyFaction = null;
            return false;
        }

        public static bool AnyQuestExistsFrom(Faction faction)
        {
            List<Site> sites = Find.WorldObjects.Sites;
            for (int i = 0; i < sites.Count; i++)
            {
                DefeatAllEnemiesQuestComp component = sites[i].GetComponent<DefeatAllEnemiesQuestComp>();
                if (component != null && component.Active && component.requestingFaction == faction)
                {
                    return true;
                }
            }
            return false;
        }

        public static Faction CommonHumanlikeEnemyFaction(Faction f1, Faction f2)
        {
            if (Find.FactionManager.AllFactions.Where((Faction x) => x != f1 && x != f2 && !x.def.hidden && x.def.humanlikeFaction && !x.defeated && x.HostileTo(f1) && x.HostileTo(f2)).TryRandomElement(out Faction result))
            {
                return result;
            }
            return null;
        }

        public static bool CommonHumanlikeEnemyFactionExists(Faction f1, Faction f2)
        {
            return CommonHumanlikeEnemyFaction(f1, f2) != null;
        }

        public static Command CommandTurnInWanted(Caravan caravan, Pawn criminal, int reward)
        {
            Command_Action command_Action = new Command_Action
            {
                defaultLabel = "WantedTurnIn".Translate(criminal, criminal.Named("PAWN")),
                defaultDesc = criminal.Dead ? "WantedTurnInDescDead".Translate(criminal, criminal.Named("PAWN")) : "WantedTurnInDescAlive".Translate(criminal, criminal.Named("PAWN")),
                icon = ContentFinder<Texture2D>.Get("World/WorldObjects/Expanding/Sites/Wanted"),
                action = delegate
                {
                    Find.LetterStack.ReceiveLetter("WantedTurnInLabel".Translate(criminal, criminal.Named("PAWN")), criminal.Dead ? "WantedTurnInMessageDead".Translate(reward * 10, criminal, criminal.Named("PAWN")) : "WantedTurnInMessageAlive".Translate(reward * 10, criminal, criminal.Named("PAWN")), LetterDefOf.PositiveEvent);
                    Thing silver = ThingMaker.MakeThing(ThingDefOf.Silver);
                    silver.stackCount = reward * 10;
                    caravan.RemovePawn(criminal);
                    CaravanInventoryUtility.GiveThing(caravan, silver);
                    foreach (Thing item in criminal.inventory.innerContainer)
                    {
                        CaravanInventoryUtility.GiveThing(caravan, item);
                    }
                    if (criminal.Dead)
                        criminal.Corpse.Destroy();
                    else
                    {
                        caravan.RemovePawn(criminal);
                        criminal.Destroy();
                    }
                }
            };
            return command_Action;
        }

        public static void GenerateBounty(Pawn pawn, Faction faction, out HediffComp_Wanted comp, bool isQuest = false)
        {
            Hediff def = pawn.health.AddHediff(Defs.HediffDefOf.Wanted);
            comp = def.TryGetComp<HediffComp_Wanted>();
            comp.WantedBy = faction ?? Find.FactionManager.RandomNonHostileFaction();
            comp.Reward = isQuest ? comp.Reward * 10 : comp.Reward;
        }
    }
}