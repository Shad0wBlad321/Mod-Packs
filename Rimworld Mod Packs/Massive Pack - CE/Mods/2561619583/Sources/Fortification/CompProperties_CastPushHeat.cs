using Verse;
using RimWorld;

namespace Fortification
{
    public class CompProperties_CastPushHeat : CompProperties
    {
        public CompProperties_CastPushHeat()
        {
            this.compClass = typeof(CompCastPushHeat);
        }
        public float energyPerCast;
    }
}
