using RimWorld;
using System.Collections.Generic;
using Verse;
using Verse.AI;

namespace SanguophageFeedRelationship
{
    class JobDriver_ChillPawnBloodfeed : JobDriver
	{
		protected Pawn Victim
		{
			get
			{
				return (Pawn)this.job.targetA.Thing;
			}
		}

		public override bool TryMakePreToilReservations(bool errorOnFailed)
		{
			return this.pawn.Reserve(this.job.targetA, this.job, 1, -1, null, errorOnFailed);
		}

		protected override IEnumerable<Toil> MakeNewToils()
		{
			this.FailOnDespawnedOrNull(TargetIndex.A);
			this.FailOnMentalState(TargetIndex.A);
			this.FailOn(() => !Victim.IsColonist || !pawn.IsColonist || pawn.IsPrisoner);
			//Log.Message("passed toils failing checks");
			 
			//yield return Toils_General.WaitWith(TargetIndex.A, 120, true, false, false, TargetIndex.None).PlaySustainerOrSound(SoundDefOf.Bloodfeed_Cast, 1f); 

			yield return GotoBiteVictim(pawn, Victim);
			yield return Toils_General.WaitWith(TargetIndex.A, 120, useProgressBar: true).PlaySustainerOrSound(SoundDefOf.Bloodfeed_Cast);
			yield return Toils_General.Do(delegate
			{
				//Log.Message(pawn + " is trying to DoBite on " + Victim);
				SanguophageUtility.DoBite(this.pawn, this.Victim, 0.2f, 0.1f, BloodLoss, 1f, IntRange.one, ThoughtDefOf.FedOn, ThoughtDefOf.FedOn_Social);
			});
			yield return Toils_Interpersonal.SetLastInteractTime(TargetIndex.A);
		}

		private static Toil GotoBiteVictim(Pawn pawn, Pawn victim)
		{
			Toil toil = ToilMaker.MakeToil("GotoBiteVictim");
			toil.initAction = delegate
			{
				pawn.pather.StartPath(victim, PathEndMode.Touch);
			};
			toil.AddFailCondition(delegate
			{
				if (victim.DestroyedOrNull())
				{
					return true;
				}
				if (!victim.Awake())
				{
					return true;
				}
				if (!pawn.IsColonist)
				{
					return true;
				}
				if (pawn.IsPrisoner)
				{
					return true;
				}
				return (victim.guest == null) ? true : false;
			});
			toil.socialMode = RandomSocialMode.Off;
			toil.defaultCompleteMode = ToilCompleteMode.PatherArrival;
			return toil;
		}


		public const float BloodLoss = 0.4499f;

		public const int WaitTicks = 1200;

		private const float HemogenGain = 0.2f;

		private const float NutritionGain = 0.1f; 
	}
}
 