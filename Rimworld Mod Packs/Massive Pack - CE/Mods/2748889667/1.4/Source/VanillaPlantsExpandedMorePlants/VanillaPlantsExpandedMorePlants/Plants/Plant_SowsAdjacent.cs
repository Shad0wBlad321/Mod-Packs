using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;
using RimWorld;

namespace VanillaPlantsExpandedMorePlants
{
    class Plant_SowsAdjacent: Plant
    {

        public override void SpawnSetup(Map map, bool respawningAfterLoad)
        {
            base.SpawnSetup(map, respawningAfterLoad);
            if (!respawningAfterLoad) {
                Random random = new Random();
                for (int i = 0; i < 8; i++)
                {
                    IntVec3 c2 = this.Position + GenAdj.AdjacentCells[i];
                    if (c2.InBounds(map))
                    {
                        
                        if (random.NextDouble() < 0.25f)
                        {
                            Plant plant = c2.GetPlant(map);
                            if (plant == null)
                            {
                                Zone_Growing zone_Growing = c2.GetZone(map) as Zone_Growing;
                                if (zone_Growing != null)
                                {
                                    GenSpawn.Spawn(InternalDefOf.VCE_PeanutSecondary, c2, this.Map);
                                }
                            }
                        }
                    }


                }

            }
            
                
        }

    }
}
