using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;
using RimWorld;
using Verse;

namespace SeamlessEmbrasures
{
    public class EmbrasuresMod : Mod
    {
        public static EmbrasuresSettings settings;
        public static EmbrasuresMod mod;

        public EmbrasuresMod(ModContentPack content) : base(content)
        {
            settings = GetSettings<EmbrasuresSettings>();
            mod = this;
        }

        public override string SettingsCategory()
        {
            return "Seamless Embrasures";
        }

        public override void DoSettingsWindowContents(Rect inRect)
        {
            Listing_Standard listingStandard = new Listing_Standard();
            listingStandard.Begin(inRect);
            listingStandard.Label("Embrasure Coverage");
            listingStandard.Label("Maximum of 99% as 100% would mean a solid wall, it's literally what allows it to work.");
            string coverageBufferString = settings.coverage.ToString();
            listingStandard.TextFieldNumericLabeled("Percentage of coverage.", ref settings.coverage, ref coverageBufferString, 0, 99);
            UpdateCoverage(settings.coverage);
            listingStandard.End();
            base.DoSettingsWindowContents(inRect);
        }

        public void UpdateCoverage(float coverage)
        {
            EmbrasureDefOf.SeamlessEmbrasure_Hole.fillPercent = (coverage / 100);
            EmbrasureDefOf.SeamlessEmbrasure_Letterbox.fillPercent = (coverage / 100);
        }
    }

    public class EmbrasuresSettings : ModSettings
    {
        public float coverage = 80f;

        public override void ExposeData()
        {
            base.ExposeData();

            Scribe_Values.Look(ref coverage, "coverage", 80f);
        }
    }

    [StaticConstructorOnStartup]
    public static class InitialisingMod
    {
        static InitialisingMod()
        {
            float coverage = (EmbrasuresMod.settings.coverage / 100);
            EmbrasureDefOf.SeamlessEmbrasure_Hole.fillPercent = coverage;
            EmbrasureDefOf.SeamlessEmbrasure_Letterbox.fillPercent = coverage;
        }
    }
}
