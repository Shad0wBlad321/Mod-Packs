using HarmonyLib;
using RimWorld;
using System;
using Verse;

namespace Warmup
{
    [HarmonyPatch(typeof(Verb), "TryStartCastOn", new Type[] { typeof(LocalTargetInfo), typeof(LocalTargetInfo), typeof(bool), typeof(bool), typeof(bool) })]
    public static class Patch_Verb_Shoot
    {
        [HarmonyPrefix, HarmonyPriority(100)]
        public static void TryStartCastOn_RapidFire_Prefix(ref Verb __instance, LocalTargetInfo castTarg, float __state, ref bool __result)
        {
            if (__instance.GetType() == typeof(Verb_Shoot))
            {
                if (__instance.caster is Building_TurretGun turret && turret != null)
                {
                    if (castTarg != null && turret != null && __instance.CanHitTarget(castTarg))
                    {
                        if (turret.TryGetComp<CompWarmUpReduction>() != null)
                        {
                            if (turret.TryGetComp<CompWarmUpReduction>() is CompWarmUpReduction GunExt)
                            {
                                if (turret.IsMannable)
                                {
                                    CompMannable mannable = turret.TryGetComp<CompMannable>();
                                    if (mannable == null)
                                    {
                                        return;
                                    }
                                    if (mannable.MannedNow)
                                    {
                                        GunExt.shotstack++;
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}