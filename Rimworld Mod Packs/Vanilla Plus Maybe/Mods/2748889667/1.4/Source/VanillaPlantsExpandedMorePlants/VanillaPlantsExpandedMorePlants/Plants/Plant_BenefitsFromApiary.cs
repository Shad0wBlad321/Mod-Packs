using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;
using RimWorld;

namespace VanillaPlantsExpandedMorePlants
{
    class Plant_BenefitsFromApiary : Plant
    {

        public bool beehouseNearby = false;

        public override void ExposeData()
        {
            base.ExposeData();

            Scribe_Values.Look(ref beehouseNearby, "beehouseNearby", false);
           
        }

        public override void SpawnSetup(Map map, bool respawningAfterLoad)
        {
            base.SpawnSetup(map, respawningAfterLoad);
            int num = GenRadial.NumCellsInRadius(6);
            for (int i = 0; i < num; i++)
            {
                IntVec3 current = this.Position + GenRadial.RadialPattern[i];
                if (current.InBounds(this.Map))
                {
                    Building getbeehouse = current.GetEdifice(this.Map);
                    if ((getbeehouse != null) && (getbeehouse.def.defName == "VFEV_Apiary") )
                    {
                        beehouseNearby = true;
                    }
                }
            }


        }

        public override float GrowthRate
        {
            get
            {
                if (Blighted)
                {
                    return 0f;
                }
                if (base.Spawned && !PlantUtility.GrowthSeasonNow(base.Position, base.Map))
                {
                    return 0f;
                }
                return GrowthRateFactor_Fertility * GrowthRateFactor_Temperature * GrowthRateFactor_Light * GrowthRateFactor_Beehouse;
            }
        }

        public float GrowthRateFactor_Beehouse
        {
            get
            {
                if (beehouseNearby)
                {
                     return 1f;

                }
                else 
                {
                    return 0.75f;
                }
               
            }
        }

        public override string GetInspectString()
        {
            if (beehouseNearby)
            {
                return base.GetInspectString() + "\n" + "VCE_Pollinated".Translate();
            }
            else
            {
                return base.GetInspectString() + "\n" + "VCE_NotPollinated".Translate();
            }
        }

    }
}
