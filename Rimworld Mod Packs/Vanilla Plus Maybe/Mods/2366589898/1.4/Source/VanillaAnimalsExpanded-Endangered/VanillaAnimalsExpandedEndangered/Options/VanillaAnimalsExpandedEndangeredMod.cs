using System.Collections.Generic;
using System.Linq;
using RimWorld;
using UnityEngine;
using Verse;

namespace VanillaAnimalsExpandedEndangered
{
    public class VanillaAnimalsExpandedEndangeredMod : Mod
    {
        public static VanillaAnimalsExpandedEndangeredSettings settings;
        public VanillaAnimalsExpandedEndangeredMod(ModContentPack pack) : base(pack)
        {
            settings = GetSettings<VanillaAnimalsExpandedEndangeredSettings>();
        }
        public override void DoSettingsWindowContents(Rect inRect)
        {
            base.DoSettingsWindowContents(inRect);
            settings.DoSettingsWindowContents(inRect);
        }

        public override string SettingsCategory()
        {
            return "Vanilla Animals Expanded - Endangered";
        }

        public override void WriteSettings()
        {
            base.WriteSettings();
            
        }
    }
    
}