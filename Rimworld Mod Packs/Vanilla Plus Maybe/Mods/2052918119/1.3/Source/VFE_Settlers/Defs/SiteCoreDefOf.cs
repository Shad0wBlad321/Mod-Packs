using RimWorld;

namespace VFE_Settlers.Defs
{
    [DefOf]
    public static class SitePartDefOf
    {
        public static SitePartDef CaravanRaid;
        public static SitePartDef WantedHideout;

        static SitePartDefOf()
        {
            DefOfHelper.EnsureInitializedInCtor(typeof(SitePartDefOf));
        }
    }
}