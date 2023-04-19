using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using HarmonyLib;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.AI;

namespace VanillaAnimalsExpandedEndangered
{
    public class VanillaAnimalsExpandedEndangeredSettings : ModSettings
    {
        public bool addBanishToAllAnimals;
        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref addBanishToAllAnimals, "addBanishToAllAnimals");
        }
        public void DoSettingsWindowContents(Rect inRect)
        {
            Listing_Standard listingStandard = new Listing_Standard();
            listingStandard.Begin(inRect);
            listingStandard.CheckboxLabeled("VAEE.AddBanishGizmoToAllAnimalsRepopulate".Translate(), ref addBanishToAllAnimals);
            listingStandard.End();
            base.Write();
        }
    }
}