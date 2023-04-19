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
    [HarmonyPatch(typeof(ResearchManager), "ReapplyAllMods")]
    public static class Patch_ResearchManager_ReapplyAllMods
    {
        [HarmonyPostfix]
        public static void Postfix(ResearchManager __instance)
        {
            FactionDef playerFactionDef = Faction.OfPlayer.def;

            // Restore Original Tech Level or if setting true start with neolithic
            if (TechTraversalMod.settings.alwaysLowestUnfinishedLevel)
            {
                playerFactionDef.techLevel = TechTraversalMod.settings.lowestTechLevel;
            }
            else
            {
                playerFactionDef.techLevel = TechTraversalMod.settings.factionTechMap.GetValueSafe(Faction.OfPlayer.def);
            }

            // Advance Tech Level as Necessary
            while (playerFactionDef.techLevel < TechLevel.Ultra && TechLevelConsideredCompleted(playerFactionDef.techLevel))
            {
                playerFactionDef.techLevel++;
                LogUtil.LogMessage("Upgraded player tech level to " + playerFactionDef.techLevel.ToStringHuman());
            }
        }

        public static bool TechLevelConsideredCompleted(TechLevel techLevel)
        {
            bool result = false;
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
            float completedPercentage = (float)allResearchForTechLevel.Where(rpd => rpd.IsFinished).Count() / (float)allResearchForTechLevel.Count();

            if(completedPercentage >= s.percentageOfTechNeeded)
            {
                result = true;
            }

            return result;
        }
    }
}
