using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using RimWorld;
using Verse;
using HarmonyLib;
using UnityEngine;

namespace TribalMedicineRevamped
{
    internal class TribalMedicineMod : Mod
    {
        public static TribalMedicine_ModSettings settings;
        private static Harmony harmony;
        public TribalMedicineMod(ModContentPack pack) : base(pack)
        {
            harmony = new Harmony("Argon.TribalMedicineRemake");
            harmony.PatchAll();

            settings = GetSettings<TribalMedicine_ModSettings>();
        }

        // Settings.

        public override string SettingsCategory()
        {
            return "TM_SettingsCategory".Translate();
        }

        public override void DoSettingsWindowContents(Rect inRect)
        {
            settings.DoWindowContents(inRect);
        }
    }

    public class TribalMedicine_ModSettings : ModSettings
    {
        public bool EnableAltNames;

        public override void ExposeData()
        {
            Scribe_Values.Look(ref EnableAltNames, "TM_Setting_EnableAltNames");
            base.ExposeData();
        }

        public void DoWindowContents(Rect wrect)
        {
            Listing_Standard listCanvas = new Listing_Standard();
            Color color = GUI.color;
            listCanvas.Begin(wrect);
            Text.Font = GameFont.Small;
            Text.Anchor = TextAnchor.UpperLeft;
            listCanvas.Gap(12f);
            listCanvas.CheckboxLabeled("TM_SettingText_EnableAltNames".Translate(), ref EnableAltNames, 12f);
            listCanvas.End();

            TribalMedicineMod.settings.Write();
        }
    }
}
