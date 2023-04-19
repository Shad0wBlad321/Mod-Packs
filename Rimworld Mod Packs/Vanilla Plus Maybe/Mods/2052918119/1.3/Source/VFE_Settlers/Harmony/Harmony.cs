using HarmonyLib;
using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using Verse;
using Verse.AI;
using VFE_Settlers.Hediffs;

namespace VFE_Settlers.Utilities
{
    [StaticConstructorOnStartup]
    internal static class Harmony
    {
        static Harmony()
        {
            var harmony = new HarmonyLib.Harmony("limetreesnake.settlers");
            harmony.PatchAll();
        }

        [HarmonyPatch(typeof(Pawn))]
        [HarmonyPatch("TickRare", MethodType.Normal)]
        public class PawnTickRare_Patch
        {
            [HarmonyPostfix]
            public static void PostFix(Pawn __instance)
            {
                if (!__instance.Dead && __instance.Spawned && __instance.kindDef == PawnKindDefOf.Muffalo && (__instance.Faction == Faction.OfPlayer || __instance.Faction == null) &&
                    !__instance.IsFighting() && !__instance.IsCaravanMember() && !__instance.IsFormingCaravan() && !__instance.roping.IsRoped)
                {
                    Map map = __instance.Map;

                    List<Thing> availableChemsine = map.listerThings.ThingsOfDef(Defs.ThingDefOf.Chemshine);
                    if (availableChemsine?.Count > 0)
                    {
                        Thing thing = GenClosest.ClosestThing_Global_Reachable(__instance.Position, map, availableChemsine, PathEndMode.OnCell, TraverseParms.For(__instance), 20f);

                        if (thing != null && VFESMod.settings.Chemsined)
                        {
                            float nutrition = FoodUtility.GetNutrition(thing, thing.def);
                            Job job = JobMaker.MakeJob(JobDefOf.Ingest, thing);
                            job.count = FoodUtility.WillIngestStackCountOf(__instance, thing.def, nutrition);
                            __instance.jobs.TryTakeOrderedJob(job);
                        }
                    }
                }
            }
        }

        [HarmonyPatch(typeof(GenStep_Settlement))]
        [HarmonyPatch("ScatterAt", MethodType.Normal)]
        [HarmonyPriority(0)]
        public class GenStep_SettlementScatterAt_Patch
        {
            [HarmonyPrefix]
            public static bool PreFix(IntVec3 c, Map map, int stackCount = 1)
            {
                try
                {
                    if (map.ParentFaction != null && (map.ParentFaction.def.defName == "SettlerCivil" || map.ParentFaction.def.defName == "SettlerBandits"))
                    {
                        IntRange range = new IntRange(36, 44);
                        int x = range.RandomInRange;
                        int z = range.RandomInRange;
                        return !UtilityBase.GenerateTown(map, Rand.Bool, map.ParentFaction, c, x, z, 6, 8, 3);
                    }
                    return true;
                }
                catch (Exception ex)
                {
                    Log.Message(ex.Message);
                    return true;
                }
            }
        }

        [HarmonyPatch(typeof(Pawn))]
        [HarmonyPatch("Kill", MethodType.Normal)]
        public class PawnKill_Patch
        {
            [HarmonyPrefix]
            public static void PreFix(Pawn __instance)
            {
                Hediff hediff = __instance.health.hediffSet.GetFirstHediffOfDef(Defs.HediffDefOf.Chemshined, false);
                if (hediff != null)
                {
                    float radius;
                    if (hediff.Severity < 0.1)
                    {
                        return;
                    }
                    else if (hediff.Severity < 0.25f)
                    {
                        radius = 1.9f;
                    }
                    else if (hediff.Severity < 0.50f)
                    {
                        radius = 2.9f;
                    }
                    else
                    {
                        radius = 4.9f;
                    }
                    GenExplosion.DoExplosion(__instance.Position, __instance.Map, radius, DamageDefOf.Flame, null);
                }
            }
        }

        [HarmonyPatch(typeof(Settlement))]
        [HarmonyPatch("GetCaravanGizmos", MethodType.Normal)]
        public class SettlementGetCaravanGizmos_Patch
        {
            [HarmonyPostfix]
            public static void PostFix(ref Caravan caravan, ref IEnumerable<Gizmo> __result)
            {
                try
                {
                    Settlement settlement = CaravanVisitUtility.SettlementVisitedNow(caravan);
                    if (settlement != null)
                    {
                        foreach (Pawn criminal in caravan.PawnsListForReading)
                        {
                            if (UtilityPawn.GetWantedComponent(criminal, out HediffComp_Wanted wanted) && settlement.Faction == wanted.WantedBy)
                            {
                                List<Gizmo> gizmos = __result.ToList();
                                gizmos.Add(UtilityEvent.CommandTurnInWanted(caravan, criminal, wanted.Reward * 2));
                                __result = gizmos.AsEnumerable();
                            }
                        }
                        foreach (Corpse corpse in caravan.AllThings.Where(x => x.def.IsCorpse))
                        {
                            if (UtilityPawn.GetWantedComponent(corpse, out HediffComp_Wanted wanted) && settlement.Faction == wanted.WantedBy)
                            {
                                List<Gizmo> gizmos = __result.ToList();
                                gizmos.Add(UtilityEvent.CommandTurnInWanted(caravan, corpse.InnerPawn, wanted.Reward));
                                __result = gizmos.AsEnumerable();
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Log.Message(ex.Message);
                }
            }
        }
    }
}