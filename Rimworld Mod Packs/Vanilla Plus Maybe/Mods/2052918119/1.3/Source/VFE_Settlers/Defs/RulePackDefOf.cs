using RimWorld;
using Verse;

namespace VFE_Settlers.Defs
{
    [DefOf]
    public static class RulePackDefOf
    {
        public static RulePackDef NamerWantedNickname;

        static RulePackDefOf()
        {
            DefOfHelper.EnsureInitializedInCtor(typeof(RulePackDefOf));
        }
    }
}