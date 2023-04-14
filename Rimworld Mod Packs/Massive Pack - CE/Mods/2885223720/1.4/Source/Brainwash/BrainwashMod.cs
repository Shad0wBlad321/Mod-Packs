using HarmonyLib;
using UnityEngine;
using Verse;

namespace Brainwash
{
    public class BrainwashMod : Mod
    {
        public static BrainwashSettings settings;
        public BrainwashMod(ModContentPack pack) : base(pack)
        {
            settings = GetSettings<BrainwashSettings>();
            new Harmony("BrainwashMod").PatchAll();
        }
        public override void DoSettingsWindowContents(Rect inRect)
        {
            base.DoSettingsWindowContents(inRect);
            settings.DoSettingsWindowContents(inRect);
        }

        public override string SettingsCategory()
        {
            return Content.Name;
        }
    }

    public class BrainwashSettings : ModSettings
    {
        public static bool modifySkillsForBrainwashing;
        public static bool modifyBackstoriesForBrainwashing;
        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref modifySkillsForBrainwashing, "modifySkillsForBrainwashing");
            Scribe_Values.Look(ref modifyBackstoriesForBrainwashing, "modifyBackstoriesForBrainwashing");
        }
        public void DoSettingsWindowContents(Rect inRect)
        {
            Rect rect = new(inRect.x, inRect.y, inRect.width, inRect.height);
            Listing_Standard listingStandard = new();
            listingStandard.Begin(rect);
            listingStandard.CheckboxLabeled("Brainwash_AllowModifyingSkillsDuringBrainwashing".Translate(), ref modifySkillsForBrainwashing);
            listingStandard.CheckboxLabeled("Brainwash_AllowModifyingBackstoriesDuringBrainwashing".Translate(), ref modifyBackstoriesForBrainwashing);
            listingStandard.End();
        }
    }
}
