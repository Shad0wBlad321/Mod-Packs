using System;
using RimWorld;
using Verse;
using UnityEngine;
using System.Collections.Generic;


namespace VanillaAnimalsExpandedRoyal
{
    public class MapComponent_Cranes : MapComponent
    {



        public int tickCounter = 0;
        public int tickInterval = 10000;
        public int ownedCranesInMap_backup = 0;
      




        public MapComponent_Cranes(Map map) : base(map)
        {

        }

        public override void FinalizeInit()
        {
            if (map.IsPlayerHome)
            {
                StaticCollectionsClass.ownedCranesInMap = ownedCranesInMap_backup;
               
            }
            base.FinalizeInit();
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look<int>(ref this.ownedCranesInMap_backup, "ownedCranesInMap_backup", 0, true);
           
            Scribe_Values.Look<int>(ref this.tickCounter, "tickCounterCranes", 0, true);

        }
        public override void MapComponentTick()
        {


            tickCounter++;
            if ((tickCounter > tickInterval))
            {


                if (map.IsPlayerHome)
                {
                    ownedCranesInMap_backup = 0;
                    List<Thing> cranesInMap = map.listerThings.ThingsOfDef(InternalDefOf.VAERoy_Crane);

                    foreach (Thing crane in cranesInMap)
                    {
                        if (crane.Faction == Faction.OfPlayer)
                        {
                            ownedCranesInMap_backup += 1;
                        }

                    }
                            
                    StaticCollectionsClass.ownedCranesInMap = ownedCranesInMap_backup;
                   
                }

                


                tickCounter = 0;
            }



        }

      




    }


}



