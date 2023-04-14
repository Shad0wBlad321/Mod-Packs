using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;
using RimWorld;

namespace VanillaPlantsExpandedMorePlants
{
    public class CompNoTransport : ThingComp
    {



        public CompProperties_NoTransport Props
        {
            get
            {
                return (CompProperties_NoTransport)this.props;
            }
        }

        public override void CompTick()
        {
            base.CompTick();

            if (this.parent.IsHashIntervalTick(Props.tickInterval))
            {
               if(this.parent.Map == null)
                {
                   
                        this.parent.Destroy();
                   
                }

            }


        }





    }
}

