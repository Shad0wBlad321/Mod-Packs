using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using Verse;

namespace BillDoorsFramework
{
    [DefOf]
    public static class StatDefOf
    {
        static StatDefOf()
        {
            DefOfHelper.EnsureInitializedInCtor(typeof(StatDefOf));
        }
        public static StatDef BDsKeyCardRequirmentStat;

        public static StatDef ArmorRating_Sharp;
    }
}
