using RimWorld;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace VFE_Settlers.Utilities
{
    public static class UtilityThing
    {
        public static IEnumerable<Thing> GetSilverInHome(Map map)
        {
            HashSet<Thing> yieldedThings = new HashSet<Thing>();
            IEnumerable<IntVec3> home = map.areaManager.Home.ActiveCells;
            foreach (IntVec3 cell in home)
            {
                List<Thing> thingList = cell.GetThingList(map);
                for (int i = 0; i < thingList.Count; i++)
                {
                    Thing t = thingList[i];
                    if (t.def == ThingDefOf.Silver && !yieldedThings.Contains(t))
                    {
                        yieldedThings.Add(t);
                        yield return t;
                    }
                }
            }
        }

        public static int GetAmountSilverInHome(IEnumerable<Thing> silver)
        {
            return silver.Sum(x => x.stackCount);
        }

        public static void RemoveSilverFromHome(IEnumerable<Thing> silver, int debt)
        {
            while (debt > 0 && silver.Count() > 0)
            {
                int count = silver.First().stackCount;
                if (count >= debt)
                {
                    silver.First().SplitOff(debt);
                    break;
                }
                else
                {
                    debt = -count;
                    silver.First().Destroy();
                }
            }
        }
    }
}