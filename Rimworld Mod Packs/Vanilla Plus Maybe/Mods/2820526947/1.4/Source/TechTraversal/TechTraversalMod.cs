using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace TechTraversal
{
    public class TechTraversalMod : Mod
    {
        public static TechTraversalMod mod;
        public static TechTraversalSettings settings;

        public Vector2 optionsScrollPosition;
        public float optionsViewRectHeight;

        internal static string VersionDir => Path.Combine(ModLister.GetActiveModWithIdentifier("Neronix17.TechTraversal").RootDir.FullName, "Version.txt");
        public static string CurrentVersion { get; private set; }

        public TechTraversalMod(ModContentPack content) : base(content)
        {
            mod = this;
            settings = GetSettings<TechTraversalSettings>();

            Version version = Assembly.GetExecutingAssembly().GetName().Version;
            CurrentVersion = $"{version.Major}.{version.Minor}.{version.Build}";

            LogUtil.LogMessage($"{CurrentVersion} ::");

            if (Prefs.DevMode)
            {
                File.WriteAllText(VersionDir, CurrentVersion);
            }

            Harmony TechTraversalHarmony = new Harmony("Neronix17.TechTraversal.RimWorld");
            TechTraversalHarmony.PatchAll(Assembly.GetExecutingAssembly());
        }

        public override string SettingsCategory() => "Tech Traversal";

        public override void DoSettingsWindowContents(Rect inRect)
        {
            bool flag = optionsViewRectHeight > inRect.height;
            Rect viewRect = new Rect(inRect.x, inRect.y, inRect.width - (flag ? 26f : 0f), optionsViewRectHeight);
            Widgets.BeginScrollView(inRect, ref optionsScrollPosition, viewRect);
            Listing_Standard listing = new Listing_Standard();
            Rect rect = new Rect(viewRect.x, viewRect.y, viewRect.width, 999999f);
            listing.Begin(rect);
            // ============================ CONTENTS ================================
            DoOptionsCategoryContents(listing);
            // ======================================================================
            optionsViewRectHeight = listing.CurHeight;
            listing.End();
            Widgets.EndScrollView();
        }

        public void DoOptionsCategoryContents(Listing_Standard listing)
        {
            listing.CheckboxLabeled("Always Lowest Level", ref settings.alwaysLowestUnfinishedLevel, "If checked, the players tech level will always be the lowest unfinished tech level. So if you start with the vanilla Crashlanded scenario, instead of starting Ultratech, you'd start Neolithic till you advance through everything.");
            listing.AddLabeledSlider<TechLevel>("Lowest Tech Level", ref settings.lowestTechLevel);
            listing.Gap();
            GUI.color = Color.gray;
            listing.Note("If 'Always Lowest Level' is enabled, this will define what the lowest tech level is, by default this is set to Neolithic. Setting below Neolithic WILL break things, setting to Ultratech or higher makes the point of this mod worthless.", GameFont.Tiny);
            GUI.color = Color.white;
            listing.CheckboxLabeled("Show Tech and Counter", ref settings.showTechCounter, "If checked, this enables a note on the Vanilla research screen which says what tech level the selected research is as well as a counter of how many techs from that level are completed.");
            listing.AddLabeledSlider("Percentage of Research Needed: " + settings.percentageOfTechNeeded.ToStringPercent(), ref settings.percentageOfTechNeeded, 0f, 1f, "Min: 0%", "Max: 100%", 0.01f);
            GUI.color = Color.gray;
            listing.Note("This controls how many of the current tech level research needs to be completed for the tech level to be counted as 'complete'.", GameFont.Tiny);
            GUI.color = Color.white;
            listing.CheckboxLabeled("Only Count Vanilla/DLC Research", ref settings.onlyCountOfficialResearch, "If checked, this makes the counter completely ignore any research added by mods.");
            //listing.CheckboxLabeled("Adjust Tech Tier Buffs", ref settings.adjustTechTiers, "The vanilla costs are only adjusted for tech tiers lower than industrial, if this is checked then researching higher or lower techs will be buffed and debuffed as approptiate, increasing the further the tech is from your current level.");
        }
    }
}
