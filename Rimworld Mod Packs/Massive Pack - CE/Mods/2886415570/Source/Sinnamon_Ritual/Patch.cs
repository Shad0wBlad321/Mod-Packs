using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using RimWorld;
using RitualRewards.Sinnamon_Ritual;
using Verse;

namespace RitualRewards;

[HarmonyPatch(typeof(RitualAttachableOutcomeEffectDef), "CanAttachToRitual")]
internal static class Patch
{
#pragma warning disable IDE0051 // Remove unused private members
    private static void Postfix(ref AcceptanceReport __result, ref RitualAttachableOutcomeEffectDef __instance, Precept_Ritual ritual)
#pragma warning restore IDE0051 // Remove unused private members
    {
        if (!__instance.HasModExtension<Sinnamon_RitualExtension>())
            return;

        List<MemeDef> forbiddenMemeAny = __instance.GetModExtension<Sinnamon_RitualExtension>().forbiddenMemeAny;
        if (!forbiddenMemeAny.NullOrEmpty() && ritual.ideo.memes.SharesElementWith(forbiddenMemeAny))
        {
            string text = forbiddenMemeAny.Select((m) => m.label.ResolveTags()).ToCommaList();
            __result = "Sinnamon_MemeConflicts".Translate() + text;
        }
    }
}
