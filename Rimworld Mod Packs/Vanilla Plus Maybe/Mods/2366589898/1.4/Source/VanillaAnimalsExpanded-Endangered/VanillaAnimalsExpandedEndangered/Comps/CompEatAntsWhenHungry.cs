using System;
using Verse;
using Verse.AI;
using RimWorld;

namespace VanillaAnimalsExpandedEndangered
{
    public class CompEatAntsWhenHungry : ThingComp
    {
      

        public CompProperties_EatAntsWhenHungry Props
        {
            get
            {
                return (CompProperties_EatAntsWhenHungry)this.props;
            }
        }


       

        public override void CompTick()
        {
            base.CompTick();
            
            if (this.parent.IsHashIntervalTick(250)) {
                Pawn pawn = this.parent as Pawn;
                //Log.Message(pawn.needs.food.CurLevelPercentage.ToString());
                if ((this.parent.Map != null) && (pawn.needs.food.CurLevelPercentage <0.5f) && (pawn.Awake()))
                {
                    //Log.Message("Conditions met");
                    Job job = JobMaker.MakeJob(DefDatabase<JobDef>.GetNamed("AEXP_IngestAnts", true),this.parent);
                    job.count = 1;
                    pawn.jobs.TryTakeOrderedJob(job, JobTag.Misc);

                }

            }

            
        }


    }
}

