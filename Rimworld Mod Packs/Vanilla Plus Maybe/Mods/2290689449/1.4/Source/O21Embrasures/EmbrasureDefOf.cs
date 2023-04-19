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
    [DefOf]
    public static class EmbrasureDefOf
    {
        static EmbrasureDefOf()
        {
            DefOfHelper.EnsureInitializedInCtor(typeof(EmbrasureDefOf));
        }

        public static ThingDef SeamlessEmbrasure_Letterbox;
        public static ThingDef SeamlessEmbrasure_Hole;
    }
}
