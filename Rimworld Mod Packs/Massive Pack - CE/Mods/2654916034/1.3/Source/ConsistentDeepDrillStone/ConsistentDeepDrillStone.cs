using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using Verse;
using HarmonyLib;

namespace CDDS
{
    [StaticConstructorOnStartup]
    public static class ConsistentDeepDrillStone
    {
        static ConsistentDeepDrillStone()
        {
            Log.Message("Consistent Deep Drill Stone Loaded!");
            var harmony = new Harmony("UdderlyEvelyn.ConsistentDeepDrillStone");
            harmony.PatchAll();
        }
    }
}
