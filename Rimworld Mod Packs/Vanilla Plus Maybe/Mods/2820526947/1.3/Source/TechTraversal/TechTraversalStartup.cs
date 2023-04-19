using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace TechTraversal
{
    [StaticConstructorOnStartup]
    public static class TechTraversalStartup
    {
        static TechTraversalStartup()
        {
            BackupOriginalTechLevels();
        }

        public static void BackupOriginalTechLevels()
        {
            foreach(FactionDef faction in DefDatabase<FactionDef>.AllDefsListForReading)
            {
                if(faction != null && faction.humanlikeFaction && !TechTraversalMod.settings.factionTechMap.ContainsKey(faction))
                {
                    TechTraversalMod.settings.factionTechMap.Add(faction, faction.techLevel);
                }
            }
        }
    }
}
