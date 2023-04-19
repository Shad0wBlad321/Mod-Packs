using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;
using RimWorld;
using Verse;

namespace O21Embrasures
{
    [DefOf]
    public static class EmbrasureDefOf
    {
        static EmbrasureDefOf()
        {
            DefOfHelper.EnsureInitializedInCtor(typeof(EmbrasureDefOf));
        }

        public static ThingDef O21_Embrasure_Letterbox;
        public static ThingDef O21_Embrasure_Hole;
    }
}
