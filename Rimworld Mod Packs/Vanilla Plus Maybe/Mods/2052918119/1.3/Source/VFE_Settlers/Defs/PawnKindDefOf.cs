using RimWorld;
using Verse;

namespace VFE_Settlers.Defs
{
    [DefOf]
    public static class PawnKindDefOf
    {
        public static PawnKindDef Outlaw;

        static PawnKindDefOf()
        {
            DefOfHelper.EnsureInitializedInCtor(typeof(PawnKindDefOf));
        }
    }
}