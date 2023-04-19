﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using Verse;

namespace BillDoorsFramework
{
    [DefOf]
    public static class EffecterDefOf
    {
        static EffecterDefOf()
        {
            DefOfHelper.EnsureInitializedInCtor(typeof(EffecterDefOf));
        }
        public static EffecterDef Effecter_3HST_RGhitTreeTrunk;
        public static EffecterDef Effecter_3HST_RGhitTreeLeafSpring;
    }
}
