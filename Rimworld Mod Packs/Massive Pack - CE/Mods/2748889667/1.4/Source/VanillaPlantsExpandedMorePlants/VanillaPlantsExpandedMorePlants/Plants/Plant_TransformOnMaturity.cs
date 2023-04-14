using System;
using UnityEngine;
using Verse;
using RimWorld;

namespace VanillaPlantsExpandedMorePlants
{
    public class Plant_TransformOnMaturity : Plant
    {

        public int tickCounter = 0;      
        public const int tickInterval = 1;
        public bool checkOnlyOnce = false;


        public override void TickLong()
        {
            base.TickLong();

            if (!checkOnlyOnce) {
                tickCounter++;

                if (tickCounter >= tickInterval)
                {
                    if (HarvestableNow && this.Map!=null)
                    {
                        System.Random rand = new System.Random();

                        if (rand.NextDouble() < 0.25)
                        {
                            Thing thing = ThingMaker.MakeThing(InternalDefOf.VCE_YellowBellPepper);
                            IntVec3 pos = this.Position;
                            Map map = this.Map;
                            thing.stackCount = 1;
                            Plant plant = thing as Plant;
                            plant.Growth = 1;
                            this.Destroy();
                            GenPlace.TryPlaceThing(thing, pos, map, ThingPlaceMode.Direct);
                            
                        }

                        checkOnlyOnce = true;
                    }


                    tickCounter = 0;
                }

            }

           



        }

        public override void ExposeData()
        {
            base.ExposeData();
            
            Scribe_Values.Look(ref checkOnlyOnce, "checkOnlyOnce", false);
            Scribe_Values.Look(ref tickCounter, "tickCounter", 0);

        }

       

    }
}
