using HarmonyLib;
using RimWorld;
using System.Reflection;
using Verse;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Verse.AI;



namespace VanillaAnimalsExpandedRoyal
{
   
    [HarmonyPatch(typeof(Graphic_Random))]
    [HarmonyPatch("GetColoredVersion")]
    public static class VanillaAnimalsExpandedRoyal_Graphic_Random_GetColoredVersion_Patch
    {
        [HarmonyPrefix]
        public static bool AvoidStupidErrorMessages(ref Graphic __result,Graphic_Random __instance, Shader newShader, Color newColor, Color newColorTwo)

        {

           
            if (__instance.path== "Things/Building/SwanNest")
            {
                __result= GraphicDatabase.Get<Graphic_Random>(__instance.path, newShader, __instance.drawSize, newColor, Color.white, __instance.data);
                return false;
            }
            else return true;


        }
    }








}
