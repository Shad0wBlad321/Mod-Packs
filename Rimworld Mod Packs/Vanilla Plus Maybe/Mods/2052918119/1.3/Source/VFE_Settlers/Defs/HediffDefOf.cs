using RimWorld;
using Verse;

namespace VFE_Settlers.Defs
{
    [DefOf]
    public static class HediffDefOf
    {
        public static HediffDef Chemshined;
        public static HediffDef Wanted;

        static HediffDefOf()
        {
            DefOfHelper.EnsureInitializedInCtor(typeof(HediffDefOf));
        }
    }
}