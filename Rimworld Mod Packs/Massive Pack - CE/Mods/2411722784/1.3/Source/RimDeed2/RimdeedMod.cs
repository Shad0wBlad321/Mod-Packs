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
    class RimdeedMod : Mod
    {
        public static RimdeedSettings settings;
        public RimdeedMod(ModContentPack pack) : base(pack)
        {
            settings = GetSettings<RimdeedSettings>();
        }
        public override void DoSettingsWindowContents(Rect inRect)
        {
            base.DoSettingsWindowContents(inRect);
            settings.DoSettingsWindowContents(inRect);
        }
        public override string SettingsCategory()
        {
            return "[RH2] Rimdeed® - Pawn Recruitment";
        }
    }
}
