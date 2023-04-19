using HarmonyLib;
using RimWorld;
using System.Reflection;
using Verse;
using Verse.AI;
using System;
using System.Collections.Generic;

namespace VanillaPlantsExpandedMorePlants
{



    [HarmonyPatch(typeof(WorkGiver_GrowerSow))]
    [HarmonyPatch("JobOnCell")]
    public static class VanillaPlantsExpandedMorePlants_WorkGiver_GrowerSow_JobOnCell_Patch
    {
        [HarmonyPostfix]
        public static void DontSowPeanuts(Pawn pawn, IntVec3 c, ref Job __result, ThingDef ___wantedPlantDef)
        {
            if (__result!=null)
            {
                Map map = pawn.Map;
                List<Thing> thingList = c.GetThingList(map);
                Zone_Growing zone_Growing = c.GetZone(map) as Zone_Growing;
               
                for (int i = 0; i < thingList.Count; i++)
                {
                    Thing thing = thingList[i];
                    if (thing.def == InternalDefOf.VCE_PeanutSecondary&& ___wantedPlantDef== InternalDefOf.VCE_Peanut)
                    {
                        __result = null;
                    }
                   
                }

            }


        }


    }


}











