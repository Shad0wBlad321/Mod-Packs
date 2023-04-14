using System.Collections.Generic;
using Verse;
using UnityEngine;

namespace DoctorStupid.Sofa
{
    public class SofaSettings : ModSettings
    {
        public bool couchMode;

        public override void ExposeData()
        {
            Scribe_Values.Look(ref couchMode, "sofaCouchMode");
            base.ExposeData();
        }
    }

    public class SofaMod : Mod
    {
        SofaSettings settings;

        public SofaMod(ModContentPack content) : base(content)
        {
            this.settings = GetSettings<SofaSettings>();
        }

        public override void DoSettingsWindowContents(Rect inRect)
        {
            Listing_Standard listingStandard = new Listing_Standard();
            listingStandard.Begin(inRect);
            listingStandard.CheckboxLabeled("Couch Mode (REQUIRES RESTART)", ref settings.couchMode, "Replaces all sofas with couches when enabled");
            listingStandard.End();
            base.DoSettingsWindowContents(inRect);
        }
        public override string SettingsCategory()
        {
            return "Literally just a sofa";
        }
    }
}