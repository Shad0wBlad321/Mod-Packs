using System;
using UnityEngine;
using Verse;
using RimWorld;

namespace VanillaPlantsExpandedMorePlants
{
    public class Plant_GrowsInTandem : Plant
    {

        public int tickCounter = 0;
        public int carrotsInMap = 0;
        public int beansInMap = 0;
        public const int tickInterval = 4;


        public override void TickLong()
        {
            base.TickLong();

            tickCounter++;

            if (tickCounter >= tickInterval)
            {
                if (this.Map != null)
                {
                    if (this.def == InternalDefOf.VCE_Beans)
                    {
                        carrotsInMap = this.Map.listerThings.ThingsOfDef(InternalDefOf.VCE_Carrot).Count;

                }
                    else if (this.def == InternalDefOf.VCE_Carrot)
                    {
                        beansInMap = this.Map.listerThings.ThingsOfDef(InternalDefOf.VCE_Beans).Count;

                    }
                }


                tickCounter = 0;
            }



        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref carrotsInMap, "carrotsInMap", 0);
            Scribe_Values.Look(ref beansInMap, "beansInMap", 0);
            Scribe_Values.Look(ref tickCounter, "tickCounter", 0);

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
                return GrowthRateFactor_Fertility * GrowthRateFactor_Temperature * GrowthRateFactor_Light * GrowthRateFactor_Proximity;
            }
        }

        public float GrowthRateFactor_Proximity
        {
            get
            {
                if (this.def == InternalDefOf.VCE_Beans)
                {
                    if (carrotsInMap <= 100)
                    {
                        return 1f + (0.003f * carrotsInMap);
                    }
                    else return 1.3f;
                    
                }
                else if (this.def == InternalDefOf.VCE_Carrot)
                {
                    if (beansInMap <= 100)
                    {
                        return 1f + (0.003f * beansInMap);
                    }
                    else return 1.3f;
                }
                else return 0;
            }
        }

        public override string GetInspectString()
        {
            if (this.def == InternalDefOf.VCE_Beans)
            {
                return base.GetInspectString() + "\n" + "VCE_CarrotsNearby".Translate(carrotsInMap);

            }
            else if (this.def == InternalDefOf.VCE_Carrot)
            {
                return base.GetInspectString() + "\n" + "VCE_BeansNearby".Translate(beansInMap);

            }
            else return base.GetInspectString();


        }



    }
}
