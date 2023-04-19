using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using Verse;

namespace VanillaPlantsExpandedMorePlants
{
    public class Plant_PrefersRocky : Plant
    {

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
                return this.GrowthRateFactor_Fertility_Inverse * this.GrowthRateFactor_Temperature * this.GrowthRateFactor_Light;
            }
        }

        public float GrowthRateFactor_Fertility_Inverse
        {
            get
            {
                float fertilityAtCell = this.Map.fertilityGrid.FertilityAt(base.Position);

                if (fertilityAtCell<=0.7) {
                    return 1f;
                }else if (fertilityAtCell>0.7 && fertilityAtCell <= 1)
                {
                    return 0.6f;
                }
                else
                {
                    return 0;
                }

               
            }
        }

        public override string GetInspectString()
        {
            if (GrowthRateFactor_Fertility_Inverse==0.6)
            {
                return base.GetInspectString() + "\n" + "VCE_StuntedGrowthFertility".Translate();

            }
            else if (GrowthRateFactor_Fertility_Inverse==0)
            {
                return base.GetInspectString() + "\n" + "VCE_StoppedGrowthFertility".Translate();

            }
            else return base.GetInspectString();


        }

    }
}
