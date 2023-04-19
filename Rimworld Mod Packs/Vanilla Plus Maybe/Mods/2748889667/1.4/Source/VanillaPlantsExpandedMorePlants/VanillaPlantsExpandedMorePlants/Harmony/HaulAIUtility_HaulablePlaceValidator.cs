using HarmonyLib;
using RimWorld;
using Verse.AI;
using Verse;



namespace VanillaPlantsExpandedMorePlants
{



    [HarmonyPatch(typeof(HaulAIUtility))]
    [HarmonyPatch("HaulablePlaceValidator")]
    public static class VanillaPlantsExpandedMorePlants_HaulAIUtility_HaulablePlaceValidator_Patch
    {
        [HarmonyPostfix]
        public static void MakeZonesNotHaulable(Thing haulable, Pawn worker, IntVec3 c,ref bool __result)
        {
            if (haulable != null && haulable.def.BlocksPlanting() && (worker.Map.zoneManager.ZoneAt(c) is Zone_GrowingAquatic|| worker.Map.zoneManager.ZoneAt(c) is Zone_GrowingSandy))
            {
                __result = false;
            }

        }


    }


}











