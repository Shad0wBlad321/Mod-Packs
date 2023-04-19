using RimWorld;
using Verse;

namespace VFE_Settlers.Defs
{
    [DefOf]
    public static class SettlerEffecterDefOf
    {
        static SettlerEffecterDefOf()
        {
            DefOfHelper.EnsureInitializedInCtor(typeof(EffecterDefOf));
        }

        public static EffecterDef Joy_HoldKnife;
    }
}