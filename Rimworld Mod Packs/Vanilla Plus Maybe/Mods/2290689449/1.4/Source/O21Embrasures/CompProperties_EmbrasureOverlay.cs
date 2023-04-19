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
    public class CompProperties_EmbrasureOverlay : CompProperties
    {
        public CompProperties_EmbrasureOverlay()
        {
            compClass = typeof(Comp_EmbrasureOverlay);
        }

        public GraphicData embrasureGraphics;
    }
}
