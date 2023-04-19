using HarmonyLib;
using RimWorld;
using System;
using Verse;

namespace Warmup
{
    [HarmonyPatch(typeof(Building_TurretGun), "BurstCooldownTime")]
    public static class Patch_TurretGun
    {
        [HarmonyPostfix]
        public static void BurstCooldownTime_RapidFire_Postfix(ref Building_TurretGun __instance, ref float __result)
        {
            if (__instance != null)
            {
                CompMannable mannable = __instance.TryGetComp<CompMannable>();
                if (mannable != null && mannable.MannedNow)
                {
                    CompWarmUpReduction GunExt = __instance.TryGetComp<CompWarmUpReduction>();
                    if (GunExt != null)
                    {
                        __result = Math.Max(__result - (GunExt.Props.WarmUpReductionPerShot * GunExt.shotstack), 0);
                    }
                }
            }
        }
    }
}