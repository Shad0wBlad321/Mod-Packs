
using RimWorld;
using System.Linq;
using Verse;
using Verse.AI;

namespace VanillaAnimalsExpandedRoyal
{

	public class JobGiver_Peacock : ThinkNode_JobGiver
	{
		private const float MaxNuzzleDistance = 40f;

		protected override Job TryGiveJob(Pawn pawn)
		{
			
			if (!(from p in pawn.Map.mapPawns.SpawnedPawnsInFaction(pawn.Faction)
				  where !p.NonHumanlikeOrWildMan() && p != pawn && p.Position.InHorDistOf(pawn.Position, MaxNuzzleDistance) && pawn.GetRoom() == p.GetRoom() && !p.Position.IsForbidden(pawn) && p.CanCasuallyInteractNow()
				  select p).TryRandomElement(out Pawn result))
			{
				return null;
			}
			Job job = JobMaker.MakeJob(InternalDefOf.VAERoy_PeacockNuzzle, result);
			job.locomotionUrgency = LocomotionUrgency.Walk;
			job.expiryInterval = 3000;
			return job;
		}
	}
}
