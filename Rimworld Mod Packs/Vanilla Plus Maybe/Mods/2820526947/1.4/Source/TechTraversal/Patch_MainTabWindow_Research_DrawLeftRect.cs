using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;
using HarmonyLib;

namespace TechTraversal
{
    [HarmonyPatch(typeof(MainTabWindow_Research), "DrawLeftRect")]
    public static class Patch_MainTabWindow_Research_DrawLeftRect
    {
        [HarmonyPostfix]
        public static void Postfix(MainTabWindow_Research __instance, Rect leftOutRect)
        {
            if (TechTraversalMod.settings.showTechCounter)
            {
                DoTechCounter(leftOutRect, __instance.selectedProject?.techLevel ?? TechLevel.Undefined);
            }
        }

        public static void DoTechCounter(Rect outRect, TechLevel techLevel = TechLevel.Undefined)
        {
            if(techLevel != TechLevel.Undefined)
            {
                GUI.color = Color.gray;
                Rect labelRect = new Rect(outRect.xMax - 120f, 0f, 120f, 24f);
                Widgets.Label(labelRect, $"{techLevel}: {GetFinishedTechCounter(techLevel)}");
                GUI.color = Color.white;
            }
        }

        public static string GetFinishedTechCounter(TechLevel techLevel)
        {
            TechTraversalSettings s = TechTraversalMod.settings;

            List<ResearchProjectDef> allResearchForTechLevel;
            if (s.onlyCountOfficialResearch)
            {
                allResearchForTechLevel = DefDatabase<ResearchProjectDef>.AllDefsListForReading.Where(rpd => rpd.techLevel == techLevel && rpd.modContentPack != null && (rpd.modContentPack.IsCoreMod || rpd.modContentPack.IsOfficialMod)).ToList();
            }
            else
            {
                allResearchForTechLevel = DefDatabase<ResearchProjectDef>.AllDefsListForReading.Where(rpd => rpd.techLevel == techLevel).ToList();
            }
            return $"({allResearchForTechLevel.Where(rpd => rpd.IsFinished).Count()}/{allResearchForTechLevel.Count()})";
        }
    }
}
