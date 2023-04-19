
using Verse;

namespace VanillaPlantsExpandedMorePlants
{
    public class CompProperties_NoTransport : CompProperties
    {
        public int tickInterval = 2000;

        public CompProperties_NoTransport()
        {
            this.compClass = typeof(CompNoTransport);
        }


    }
}