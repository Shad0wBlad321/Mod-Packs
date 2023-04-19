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
            return "Vanilla Animals Expanded — Endangered";
        }

        public override void WriteSettings()
        {
            base.WriteSettings();
            DefsAlterer.DoDefsAlter();
        }
    }
    [StaticConstructorOnStartup]
    public static class DefsAlterer
    {
        static DefsAlterer()
        {
            DoDefsAlter();
        }
        public static void DoDefsAlter()
        {
            if (VanillaAnimalsExpandedEndangeredMod.settings.addBanishToAllAnimals)
            {
                foreach (var animal in DefDatabase<PawnKindDef>.AllDefs.Where(x => x.race.race.Animal))
                {
                    Pawn_GetGizmos_Patch.animalsToBanish.Add(animal);
                }
            }
        }
    }
}