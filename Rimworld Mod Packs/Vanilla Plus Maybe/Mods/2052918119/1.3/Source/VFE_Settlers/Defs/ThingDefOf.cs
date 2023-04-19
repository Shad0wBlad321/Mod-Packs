using RimWorld;
using Verse;

namespace VFE_Settlers.Defs
{
    [DefOf]
    public static class ThingDefOf
    {
        public static ThingDef Chemshine;
        public static ThingDef Building_ChemshineBarrel;

        static ThingDefOf()
        {
            DefOfHelper.EnsureInitializedInCtor(typeof(ThingDefOf));
        }
    }
}