using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace Rimdeed
{
    public class RimdeedSettings : ModSettings
    {
        public static int woodlogCost = 1250;
        public static int silverCost = 5000;
        public static int goldCost = 8000;

        public static bool resetBan;
        public static bool disallowNeolithicFaction = true;
        public static bool disallowUltraFaction = true;
        public static bool disallowSpacerFaction = false;
        public static IntRange ageRestriction = new IntRange(16, 60);
        public static List<string> forbiddenRaces = new List<string>();
        public string raceInput;

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref woodlogCost, "woodlogCost");
            Scribe_Values.Look(ref silverCost, "silverCost");
            Scribe_Values.Look(ref goldCost, "goldCost");

            Scribe_Values.Look(ref resetBan, "resetBan");
            Scribe_Values.Look(ref disallowNeolithicFaction, "disallowNeolithicFaction", true);
            Scribe_Values.Look(ref disallowUltraFaction, "disallowUltraFaction", true);
            Scribe_Values.Look(ref disallowSpacerFaction, "disallowSpacerFaction", false);

            int ageRestrictionMin = ageRestriction.min;
            int ageRestrictionMax = ageRestriction.max;
            Scribe_Values.Look(ref ageRestrictionMin, "ageRestrictionMin", 16);
            Scribe_Values.Look(ref ageRestrictionMax, "ageRestrictionMax", 60);
            ageRestriction = new IntRange(ageRestrictionMin, ageRestrictionMax);

            Scribe_Collections.Look(ref forbiddenRaces, "forbiddenRaces", LookMode.Value, Array.Empty<string>());
        }

        public void DoSettingsWindowContents(Rect inRect)
        {
            Rect rect = new Rect(inRect.x, inRect.y, inRect.width, inRect.height);
            Listing_Standard listingStandard = new Listing_Standard();
            listingStandard.Begin(rect);
            if (listingStandard.ButtonText("Reset".Translate()))
            {
                woodlogCost = 1250;
                silverCost = 5000;
                goldCost = 8000;
                disallowNeolithicFaction = true;
                disallowUltraFaction = true;
                disallowSpacerFaction = false;
                ageRestriction = new IntRange(16, 60);
                forbiddenRaces = new List<string>();
            }

            listingStandard.Label("RimDeed.AdjustWoodlogCost".Translate().ToString() + woodlogCost.ToString("c0"));
            woodlogCost = (int)listingStandard.Slider(woodlogCost, 0f, 10000f);
            listingStandard.Label("RimDeed.AdjustSilverCost".Translate().ToString() + silverCost.ToString("c0"));
            silverCost = (int)listingStandard.Slider(silverCost, 0f, 10000f);
            listingStandard.Label("RimDeed.AdjustGoldCost".Translate().ToString() + goldCost.ToString("c0"));
            goldCost = (int)listingStandard.Slider(goldCost, 0f, 10000f);

            if (listingStandard.ButtonText("RimDeed.ResetBan".Translate()))
            {
                resetBan = true;
            }

            listingStandard.Label("RimDeed.AdjustAgeRestrictionForApplicants".Translate() + $"({ageRestriction.min}-{ageRestriction.max})");
            listingStandard.IntRange(ref ageRestriction, 0, 200);

            listingStandard.CheckboxLabeled("RimDeed.DisallowNeolithicFaction".Translate(), ref disallowNeolithicFaction);
            listingStandard.CheckboxLabeled("RimDeed.DisallowSpacerFaction".Translate(), ref disallowSpacerFaction);
            listingStandard.CheckboxLabeled("RimDeed.DisallowUltraFaction".Translate(), ref disallowUltraFaction);
            listingStandard.Label("RimDeed.RestrictRacesOfApplicants".Translate());

            var humanlikeRaces = DefDatabase<ThingDef>.AllDefs.Where(x => (x.race?.Humanlike ?? false)).ToList();
            if (humanlikeRaces.Any()) 
            {
                if (forbiddenRaces is null)
                    forbiddenRaces = new List<string>();

                raceInput = listingStandard.TextEntry(raceInput, 1);
                if (!raceInput.NullOrEmpty())
                {
                    humanlikeRaces = humanlikeRaces.Where(x => x.LabelCap.ToLower().RawText.Contains(raceInput)).ToList();
                }
                float listHeight = humanlikeRaces.Count() * 30f;
                Rect viewRect = new Rect(inRect.x, 385, 300f - 26f, 200);
                Rect scrollRect = new Rect(inRect.x, 385, 300f - 43f, listHeight);

                Widgets.BeginScrollView(viewRect, ref scrollPosition, scrollRect);
                GUI.BeginGroup(scrollRect);

                for (var i = 0; i < humanlikeRaces.Count; i++)
                {
                    var race = humanlikeRaces[i];
                    var forbidden = forbiddenRaces.Contains(race.defName);

                    Widgets.CheckboxLabeled(new Rect(viewRect.x + 20, 30 * i, 230f, 30f), race.LabelCap, ref forbidden);

                    if (forbidden && !forbiddenRaces.Contains(race.defName))
                    {
                        forbiddenRaces.Add(race.defName);
                    }
                    else if (!forbidden && forbiddenRaces.Contains(race.defName))
                    {
                        forbiddenRaces.Remove(race.defName);
                    }
                }
                GUI.EndGroup();
                Widgets.EndScrollView();
            }
            listingStandard.End();
            base.Write();
        }

        private Vector2 scrollPosition = Vector2.zero;
    }
}
