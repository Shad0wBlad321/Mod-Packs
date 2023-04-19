using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using Verse;
using RimWorld;
using static HarmonyLib.Code;

namespace BillDoorsFramework
{
    public class ModExtension_LGISdualModeNPCpatch : DefModExtension
    {
        public float chance = 1f;
    }

    public class ModExtension_FireAllAtOnceCE : DefModExtension
    {
        public int barrelCount = 1;
    }

    public class ModExtension_RandomBurstBreak : DefModExtension
    {
        public float chance = 0.08f;
        public IntRange randomBurst = new IntRange(0, 0);
    }

    public class ModExtension_Verb_Shotgun : DefModExtension
    {
        public int ShotgunPellets = 1;
        public ThingDef extraProjectile;
        public int extraProjectileCount = 1;
    }

    [StaticConstructorOnStartup]
    public static class LGISdualModeNPCpatch
    {
        [HarmonyPatch(typeof(PawnGenerator), "PostProcessGeneratedGear", new Type[] { typeof(Thing), typeof(Pawn) })]
        static class ProjectilesSpawn_PostFix
        {
            [HarmonyPostfix]
            static void Postfix(Thing gear)
            {
                CompSecondaryVerb comp = gear.TryGetComp<CompSecondaryVerb>();
                ModExtension_LGISdualModeNPCpatch extension = gear.def.GetModExtension<ModExtension_LGISdualModeNPCpatch>();
                if (extension != null && comp != null && Rand.Chance(extension.chance))
                {
                    comp.SwitchToSecondary();
                }
            }
        }
    }
}
