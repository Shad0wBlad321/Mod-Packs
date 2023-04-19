using System;
using System.Collections.Generic;
using System.Diagnostics;
using Verse;
using Verse.AI;
using RimWorld;

namespace VanillaAnimalsExpandedEndangered
{
    public class JobDriver_EatAnts : JobDriver
    {
        private const int EatingDuration = 1000;

        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
            return true;
        }

        [DebuggerHidden]
        protected override IEnumerable<Toil> MakeNewToils()
        {
            //Log.Message("Starting job");
            Toil wait = new Toil();
            wait.defaultCompleteMode = ToilCompleteMode.Delay;
            wait.defaultDuration = EatingDuration;
            wait.socialMode = RandomSocialMode.Off;
            wait.FailOnCannotTouch(TargetIndex.A, PathEndMode.Touch);
            yield return wait.WithProgressBarToilDelay(TargetIndex.A, true);
            
            yield return Toils_General.Do(delegate
            {
                this.pawn.needs.food.CurLevel += 1;

            });
        }
    }
}