using HarmonyLib;
using Verse;

namespace RitualRewards;

[StaticConstructorOnStartup]
internal static class PatchDriver
{
    static PatchDriver()
    {
        Log.Message("Sinnamon loaded!");
        Harmony harmony = new("Sinnamon.RitualReward");
        harmony.PatchAll();
    }
}
