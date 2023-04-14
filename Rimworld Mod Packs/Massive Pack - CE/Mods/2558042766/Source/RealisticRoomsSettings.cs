using RimWorld;
using System;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace RealisticRoomsRewritten
{
    [StaticConstructorOnStartup]
    public static class RealisticRoomsModOnStartup
    {
        static RealisticRoomsSettings settings;
        static RealisticRoomsModOnStartup()
        {
            settings = LoadedModManager.GetMod<RealisticRoomsMod>().GetSettings<RealisticRoomsSettings>();
            ApplySettings(settings);
        }
        public static void ApplySettings(RealisticRoomsSettings settings)
        {
            var scoreStages = DefDatabase<RoomStatDef>.GetNamed("Space").scoreStages;
            foreach (var stage in scoreStages)
            {
                if (stage.label == "rather tight") { stage.minScore = settings.minSpaceRatherTight; }
                if (stage.label == "average-sized") { stage.minScore = settings.minSpaceAverageSized; }
                if (stage.label == "somewhat spacious") { stage.minScore = settings.minSpaceSomewhatSpacious; }
                if (stage.label == "quite spacious") { stage.minScore = settings.minSpaceQuiteSpacious; }
                if (stage.label == "very spacious") { stage.minScore = settings.minSpaceVerySpacious; }
                if (stage.label == "extremely spacious") { stage.minScore = settings.minSpaceExtremelySpacious; }
            }
        }
    }

    public class RealisticRoomsSettings : ModSettings
    {
        public float minSpaceRatherTight = 6.5f;
        public float minSpaceAverageSized = 16.5f;
        public float minSpaceSomewhatSpacious = 28.5f;
        public float minSpaceQuiteSpacious = 49.5f;
        public float minSpaceVerySpacious = 84.5f;
        public float minSpaceExtremelySpacious = 174.5f;

        public override void ExposeData()
        {
            Scribe_Values.Look(ref this.minSpaceRatherTight, "minSpaceRatherTight", 6.5f);
            Scribe_Values.Look(ref this.minSpaceAverageSized, "minSpaceAverageSized", 16.5f);
            Scribe_Values.Look(ref this.minSpaceSomewhatSpacious, "minSpaceSomewhatSpacious", 28.5f);
            Scribe_Values.Look(ref this.minSpaceQuiteSpacious, "minSpaceQuiteSpacious", 49.5f);
            Scribe_Values.Look(ref this.minSpaceVerySpacious, "minSpaceVerySpacious", 84.5f);
            Scribe_Values.Look(ref this.minSpaceExtremelySpacious, "minSpaceExtremelySpacious", 174.5f);
            base.ExposeData();
        }
    }
    public class RealisticRoomsMod : Mod
    {
        RealisticRoomsSettings settings;

        public RealisticRoomsMod(ModContentPack content) : base(content)
        {
            settings = GetSettings<RealisticRoomsSettings>();
        }

        public override string SettingsCategory() => "RealisticRoomsSettingsCategoryLabel".Translate();

        public override void DoSettingsWindowContents(Rect canvas)
        {
            Rect rect = canvas;
            Listing_Standard ls = new Listing_Standard();
            ls.Begin(canvas);
            ls.Label("SettingsLabelTip".Translate());
            ls.GapLine();
            ls.ColumnWidth = (canvas.width - 40f) * 0.34f;
            Text.Anchor = (TextAnchor)4;
            ls.Label("SettingsRTLabel".Translate());
            ls.Gap(0.5f);
            ls.Label("SettingsASLabel".Translate());
            ls.Gap(0.5f);
            ls.Label("SettingsSSLabel".Translate());
            ls.Gap(0.5f);
            ls.Label("SettingsQSLabel".Translate());
            ls.Gap(0.5f);
            ls.Label("SettingsVSLabel".Translate());
            ls.Gap(0.5f);
            ls.Label("SettingsESLabel".Translate());
            Text.Anchor = (TextAnchor)0;
            ls.NewColumn();
            ls.ColumnWidth = (canvas.width - 40f) * 0.66f;
            ls.Gap(34f);
            settings.minSpaceRatherTight = ls.SliderDouble(settings.minSpaceRatherTight, 0f, settings.minSpaceAverageSized);
            settings.minSpaceAverageSized = ls.SliderDouble(settings.minSpaceAverageSized, 0f, settings.minSpaceSomewhatSpacious);
            settings.minSpaceSomewhatSpacious = ls.SliderDouble(settings.minSpaceSomewhatSpacious, 0f, settings.minSpaceQuiteSpacious);
            settings.minSpaceQuiteSpacious = ls.SliderDouble(settings.minSpaceQuiteSpacious, 0f, settings.minSpaceVerySpacious);
            settings.minSpaceVerySpacious = ls.SliderDouble(settings.minSpaceVerySpacious, 0f, settings.minSpaceExtremelySpacious);
            settings.minSpaceExtremelySpacious = ls.SliderDouble(settings.minSpaceExtremelySpacious, 0f, 400f);
            bool buttonpressed = ls.ButtonTextSound("SettingsResetButton".Translate(), null, 0.4f);
            ls.End();
            base.DoSettingsWindowContents(canvas);

            if (buttonpressed)
            {
                settings.minSpaceRatherTight = 6.5f;
                settings.minSpaceAverageSized = 16.5f;
                settings.minSpaceSomewhatSpacious = 28.5f;
                settings.minSpaceQuiteSpacious = 49.5f;
                settings.minSpaceVerySpacious = 84.5f;
                settings.minSpaceExtremelySpacious = 174.5f;
                buttonpressed = false;
            }

            RealisticRoomsModOnStartup.ApplySettings(settings);
        }
        //private void ApplySettings()
        //{
        //    var scoreStages = DefDatabase<RoomStatDef>.GetNamed("Space").scoreStages;
        //    foreach (var stage in scoreStages)
        //    {
        //        if (stage.label == "rather tight") { stage.minScore = settings.minSpaceRatherTight; }
        //        if (stage.label == "average-sized") { stage.minScore = settings.minSpaceAverageSized; }
        //        if (stage.label == "somewhat spacious") { stage.minScore = settings.minSpaceSomewhatSpacious; }
        //        if (stage.label == "quite spacious") { stage.minScore = settings.minSpaceQuiteSpacious; }
        //        if (stage.label == "very spacious") { stage.minScore = settings.minSpaceVerySpacious; }
        //        if (stage.label == "extremely spacious") { stage.minScore = settings.minSpaceExtremelySpacious; }
        //    }
        //}
    }

    public static class Extensions
    {
        public static float SliderDouble(this Listing_Standard ls,  float val, float min, float max)
        {
            string buffer = val.ToString();
            Rect rect = ls.GetRect(22f);
            rect.width -= 60f;
            Text.Anchor = (TextAnchor)5;
            Widgets.TextFieldNumeric(rect.RightPart(0.24f), ref val, ref buffer, min, max);
            Text.Anchor = (TextAnchor)0;
            float num = Widgets.HorizontalSlider(rect.LeftPart(1f - 0.26f), val, min, max, true, null, null, null, 0.5f);
            ls.Gap(ls.verticalSpacing);
            if (num > max) num = max;
            return num;
        }

        public static bool ButtonTextSound(this Listing_Standard ls, string label, string highlightTag = null, float widthPct = 1f)
        {
            Rect rect = ls.GetRect(30f, widthPct);
            bool result = false;
            if (!ls.BoundingRectCached.HasValue || rect.Overlaps(ls.BoundingRectCached.Value))
            {
                result = Widgets.ButtonText(rect, label);
                if (highlightTag != null)
                {
                    UIHighlighter.HighlightOpportunity(rect, highlightTag);
                }
            }
            ls.Gap(ls.verticalSpacing);
            if (result) SoundDefOf.Click.PlayOneShotOnCamera();
            return result;
        }
    }
}