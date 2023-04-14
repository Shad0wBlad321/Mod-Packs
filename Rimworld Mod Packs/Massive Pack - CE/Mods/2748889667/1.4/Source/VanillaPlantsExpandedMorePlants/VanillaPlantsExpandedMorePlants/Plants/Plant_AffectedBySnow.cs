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
    public class Plant_AffectedBySnow : Plant
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
                return this.GrowthRateFactor_Fertility * this.GrowthRateFactor_Temperature * this.GrowthRateFactor_Light * this.GrowthRateFactor_Snow;
            }
        }

        public float GrowthRateFactor_Snow
        {
            get
            {
                if (this.Map.weatherManager.SnowRate > 0)
                {
                    return 1.5f;
                }
                else return 1f;
            }
        }
       
    }
}
