using RimWorld; 
using Verse;
using Verse.AI;


namespace SanguophageFeedRelationship
{
	// big thanks to 'these people' and their Vampire Thralls mod, their source code helped me get over a block I'd been dealing with for weeks

	class JobGiver_ChillPawnBloodfeed : JobGiver_GetHemogen
	{
		static SFR_AutoFeeders gc;
		public JobGiver_ChillPawnBloodfeed() {
			gc = new SFR_AutoFeeders(Current.Game);
		}

		protected override Job TryGiveJob(Pawn pawn)
		{
			if (!ModsConfig.BiotechActive) {
				return null;
			}
			Gene_Hemogen gene_Hemogen = pawn.genes?.GetFirstGeneOfType<Gene_Hemogen>();
			if (gene_Hemogen == null) {
				return null; }
			if (!gene_Hemogen.ShouldConsumeHemogenNow()) {
				return null; }
			
			if (pawn.IsBloodfeeder())
			{
				//Log.Message("Trying to find a victim for " + pawn.Name);
				Pawn victim = FindChillPawn(pawn);
				if (victim != null)
				{
					Log.Message(pawn.Name + " will auto-feed from " + victim.Name + " now.");
					return JobMaker.MakeJob(DefDatabase<JobDef>.GetNamed("ChillPawnBloodfeed"), victim);
				}
			}
			return null;
		}

		public static AcceptanceReport CanFeedOnChillPawn(Pawn bloodfeeder, Pawn victim)
		{
			//Log.Message("Acceptance Report for " + victim.Name);

			if (victim == null)
				return false;

			if (!gc.GetAutoFeed(victim.thingIDNumber))
				return false;

			if (victim.WouldDieFromAdditionalBloodLoss(SFR_ModSettings.minimumSeverity)) // 0.4499f default
				return false;

			if (!victim.Awake() || victim.IsForbidden(bloodfeeder) || victim.InAggroMentalState || !victim.IsColonist || victim.IsBloodfeeder())
				return false;

			if (!bloodfeeder.CanReserveAndReach(victim, PathEndMode.OnCell, bloodfeeder.NormalMaxDanger(), 1, -1, null, false) ||
				!bloodfeeder.IsColonist || bloodfeeder.IsPrisoner)
				return false;

			if (victim.story.traits.HasTrait(SFR_DefOf.SFR_Sanguofriend) || victim.story.traits.HasTrait(SFR_DefOf.SFR_Sanguophile) || victim.relations.DirectRelationExists(PawnRelationDefOf.Lover, bloodfeeder) ||
				victim.relations.DirectRelationExists(PawnRelationDefOf.Fiance, bloodfeeder) || victim.relations.DirectRelationExists(PawnRelationDefOf.Spouse, bloodfeeder) ||
				(victim.relations.OpinionOf(bloodfeeder) >= SFR_ModSettings.CloseFriend_minimumOpinion))
				return true;
			
			return false; 
		}

		private Pawn FindChillPawn(Pawn pawn)
		{
			return (Pawn)GenClosest.ClosestThing_Global_Reachable(pawn.Position, pawn.Map, pawn.Map.mapPawns.FreeColonistsAndPrisoners, PathEndMode.OnCell,
				 TraverseParms.For(pawn, Danger.Deadly, TraverseMode.ByPawn, false, false, false), 9999f, (Thing victim) => CanFeedOnChillPawn(pawn, (Pawn)victim).Accepted, null);

			/*return (Pawn)GenClosest.ClosestThing_Global_Reachable(pawn.Position, pawn.Map, pawn.Map.mapPawns.AllPawnsSpawned, PathEndMode.OnCell,
				TraverseParms.For(pawn, Danger.Deadly, TraverseMode.ByPawn, false, false, false), 9999f, validator: delegate (Thing t)
                {
					Pawn victim = t as Pawn;
					bool b = CanFeedOnChillPawn(pawn, victim).Accepted;
					Log.Message("Getting auto feeder in FindChillPawn " + pawn.Name + " " + b);

					return b;
				}, null); */
		}
	} 
}
