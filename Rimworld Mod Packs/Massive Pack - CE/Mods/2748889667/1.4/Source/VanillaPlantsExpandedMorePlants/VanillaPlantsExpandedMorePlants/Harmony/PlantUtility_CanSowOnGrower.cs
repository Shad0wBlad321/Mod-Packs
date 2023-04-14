using HarmonyLib;
using RimWorld;
using System.Reflection;
using Verse;

using System;


namespace VanillaPlantsExpandedMorePlants
{



    [HarmonyPatch(typeof(PlantUtility))]
    [HarmonyPatch("CanSowOnGrower")]
    public static class VanillaPlantsExpandedMorePlants_PlantUtility_CanSowOnGrower_Patch
    {
        [HarmonyPostfix]
        public static void SowTagsOnAquaticPlants( ThingDef plantDef, object obj, ref bool __result)
        {
            if (obj is Zone_GrowingAquatic)
            {
                __result = plantDef.plant.sowTags.Contains("VCE_Aquatic");
            }
            if (obj is Zone_GrowingSandy)
            {
                __result = plantDef.plant.sowTags.Contains("VCE_Sandy");
            }


        }


    }


}











