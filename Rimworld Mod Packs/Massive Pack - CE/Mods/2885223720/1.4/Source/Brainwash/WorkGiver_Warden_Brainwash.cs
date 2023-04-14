using RimWorld;
using Verse;
using Verse.AI;

namespace Brainwash
{
	public class WorkGiver_Warden_Brainwash : WorkGiver_Warden
	{
		public override Job JobOnThing(Pawn pawn, Thing t, bool forced = false)
		{
			if (!ShouldTakeCareOfPrisoner(pawn, t))
			{
				return null;
			}
			Pawn prisoner = (Pawn)t;
			PrisonerInteractionModeDef interactionMode = prisoner.guest.interactionMode;
			if (prisoner.CanBeBrainwashedBy(pawn) && interactionMode == BrainwashDefOf.RedHorse_Brainwash
				&& prisoner.CurJobDef != BrainwashDefOf.RedHorse_WatchBrainwashTelevision 
				&& prisoner.CurJobDef != BrainwashDefOf.RedHorse_StartBrainwashTelevision
                && (prisoner.guest.will > 0 || prisoner.guest.resistance > 0))
			{
				CompChangePersonality comp = prisoner.TryGetComp<CompChangePersonality>();
				if (comp.TryGetNearbyTelevisionAndChair(pawn, out var televisionAndChair))
				{
                    Job job = JobMaker.MakeJob(BrainwashDefOf.RedHorse_LeadToBrainwashChair, t, televisionAndChair.Item2,
                        televisionAndChair.Item1);
                    job.count = 1;
                    return job;
                }
			}
			return null;
		}
	}
}
