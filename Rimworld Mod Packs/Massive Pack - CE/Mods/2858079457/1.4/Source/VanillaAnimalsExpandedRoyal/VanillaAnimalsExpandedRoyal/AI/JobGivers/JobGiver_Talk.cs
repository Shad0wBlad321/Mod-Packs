
using System.Linq;
using RimWorld;
using Verse;
using Verse.AI;

namespace VanillaAnimalsExpandedRoyal
{
	public class JobGiver_Talk : ThinkNode_JobGiver
	{
		private const float MaxTalkingDistance = 10f;

		protected override Job TryGiveJob(Pawn pawn)
		{

			if (pawn.Faction == null)
			{
				return null;
			}

			if (!(from p in pawn.Map.mapPawns.SpawnedPawnsInFaction(pawn.Faction)
				  where !p.NonHumanlikeOrWildMan() && p != pawn && p.Position.InHorDistOf(pawn.Position, MaxTalkingDistance) && pawn.GetRoom() == p.GetRoom() && !p.Position.IsForbidden(pawn) && p.CanCasuallyInteractNow()
				  select p).TryRandomElement(out var result))
			{
				return null;
			}
			Job job = JobMaker.MakeJob(InternalDefOf.VAERoy_OrangutanTalk, result);
			job.locomotionUrgency = LocomotionUrgency.Sprint;
			job.expiryInterval = 3000;
			return job;
		}
	}
}
