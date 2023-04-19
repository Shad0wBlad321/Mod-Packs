using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;

using Verse;

namespace VanillaPlantsExpandedMorePlants
{
    public class Plant_AffectedByFog : Plant
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
                return this.GrowthRateFactor_Fertility * this.GrowthRateFactor_Temperature * this.GrowthRateFactor_Light * this.GrowthRateFactor_Fog;
            }
        }

        public float GrowthRateFactor_Fog
        {
            get
            {
                if (this.Map.weatherManager.curWeather.defName.Contains("fog")|| this.Map.weatherManager.curWeather.defName.Contains("Fog"))
                {
                    return 1.3f;
                }
                else return 1f;
            }
        }
    }
}
