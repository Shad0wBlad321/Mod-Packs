using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace GeneRipper
{
    public class GeneRipperSettings : ModSettings
    {

        public int ExtractionHours = 24;
        public int ExtractionTicks => (int) (ExtractionHours * 2500);

        public float BlendingChance = 0.5f;

        public override void ExposeData()
        {

            Scribe_Values.Look(ref ExtractionHours, nameof(ExtractionHours), 24);
            Scribe_Values.Look(ref BlendingChance, nameof(BlendingChance), 0.5f);

            base.ExposeData();
        }
    }
}
