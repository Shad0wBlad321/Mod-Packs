using Verse;

namespace TribalMedicineRevamped
{
    [StaticConstructorOnStartup]
    public static class SettingsApply
    {
        private static bool ModSettings_AltNames => LoadedModManager.GetMod<TribalMedicineMod>().GetSettings<TribalMedicine_ModSettings>().EnableAltNames;

        static SettingsApply()
        {
            if (ModSettings_AltNames)
            {
                DefDatabase<ThingDef>.GetNamed("TM_BlackTea").label = "Immuni-tea";
            }
        }
    }
}