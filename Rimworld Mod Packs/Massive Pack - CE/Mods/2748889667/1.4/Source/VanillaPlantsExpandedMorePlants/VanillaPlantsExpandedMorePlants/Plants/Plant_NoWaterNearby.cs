using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using UnityEngine;

using Verse;

namespace VanillaPlantsExpandedMorePlants
{
    public class Plant_NoWaterNearby : Plant
    {

        const int radius = 10;
        bool waterFound = false;


        public override void SpawnSetup(Map map, bool respawningAfterLoad)
        {
            base.SpawnSetup(map, respawningAfterLoad);
            int num = GenRadial.NumCellsInRadius(radius);
            for (int i = 0; i < num; i++)
            {
                IntVec3 c = this.Position + GenRadial.RadialPattern[i];
                if (c.InBounds(map))
                {
                    TerrainDef terrain = c.GetTerrain(map);

                    if (terrain != null && terrain.IsWater)
                    {
                        waterFound=true;
                        break;
                    }

                }




            }

        }

        public override float GrowthRate
        {
            get
            {
                if (this.Blighted)
                {
                    return 0f;
                }
                if (base.Spawned && !PlantUtility.GrowthSeasonNow(base.Position, base.Map, false))
                {
                    return 0f;
                }
                return this.GrowthRateFactor_Fertility * this.GrowthRateFactor_Temperature * this.GrowthRateFactor_Light * this.GrowthRateFactor_Water;
            }
        }

        public float GrowthRateFactor_Water
        {
            get
            {
                if (waterFound)
                {
                    return 0.3f;
                }
                else return 1f;
            }
        }

        public override void ExposeData()
        {
            base.ExposeData();

            Scribe_Values.Look<bool>(ref this.waterFound, "waterFound", false, false);

        }

        public override string GetInspectString()
        {
            if (GrowthRateFactor_Water == 0.3f)
            {
                return base.GetInspectString() + "\n" + "VCE_WaterNearby".Translate();

            }
            else 
            {
                return base.GetInspectString();

            }
           


        }


    }
}
