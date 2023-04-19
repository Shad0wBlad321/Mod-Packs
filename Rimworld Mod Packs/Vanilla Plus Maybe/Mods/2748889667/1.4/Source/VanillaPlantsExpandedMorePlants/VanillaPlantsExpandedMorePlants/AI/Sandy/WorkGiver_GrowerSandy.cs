using RimWorld;
using System.Collections.Generic;
using System.Diagnostics;
using Verse;
using Verse.AI;

namespace VanillaPlantsExpandedMorePlants
{
    public abstract class WorkGiver_GrowerSandy : WorkGiver_Scanner
    {
        protected static ThingDef wantedPlantDef;

        public override bool AllowUnreachable
        {
            get
            {
                return true;
            }
        }

        protected virtual bool ExtraRequirements(IPlantToGrowSettable settable, Pawn pawn)
        {
            return true;
        }

        [DebuggerHidden]
        public override IEnumerable<IntVec3> PotentialWorkCellsGlobal(Pawn pawn)
        {
            Danger maxDanger = pawn.NormalMaxDanger();
            /* List<Building> bList = pawn.Map.listerBuildings.allBuildingsColonist;
             for (int i = 0; i < bList.Count; i++)
             {
                 Building_PlantGrower b = bList[i] as Building_PlantGrower;
                 if ((b != null)&&b.TryGetComp<CompBotanyPlanter>().GetIsBotanyPlanter)
                 {
                     if (this.ExtraRequirements(b, pawn))
                     {
                         if (!b.IsForbidden(pawn))
                         {
                             if (pawn.CanReach(b, PathEndMode.OnCell, maxDanger, false, TraverseMode.ByPawn))
                             {
                                 if (!b.IsBurning())
                                 {

                                     foreach (IntVec3 intVec in b.OccupiedRect())
                                     {
                                         yield return intVec;
                                     }
                                     WorkGiver_GrowerBotany.wantedPlantDef = null;


                                 }
                             }
                         }
                     }
                 }
             }*/
            wantedPlantDef = null;
            List<Zone> zonesList = pawn.Map.zoneManager.AllZones;
            for (int j = 0; j < zonesList.Count; j++)
            {
                Zone_GrowingSandy growZone = zonesList[j] as Zone_GrowingSandy;
                if (growZone == null)
                {
                    continue;
                }
                if (growZone.cells.Count == 0)
                {
                    Log.ErrorOnce("Grow zone has 0 cells: " + growZone, -563487);
                }
                else if (ExtraRequirements(growZone, pawn) && !growZone.ContainsStaticFire && pawn.CanReach(growZone.Cells[0], PathEndMode.OnCell, maxDanger))
                {
                    for (int i = 0; i < growZone.cells.Count; i++)
                    {
                        yield return growZone.cells[i];
                    }
                    wantedPlantDef = null;
                }
            }
            wantedPlantDef = null;
        }

        public static ThingDef CalculateWantedPlantDef(IntVec3 c, Map map)
        {
            return c.GetPlantToGrowSettable(map)?.GetPlantDefToGrow();
        }
    }
}
