using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;
using Verse.AI;
using Verse.AI.Group;

namespace Casevac
{
	public class WorkGiver_Casevac : WorkGiver_Scanner
	{
        public override ThingRequest PotentialWorkThingRequest => ThingRequest.ForGroup(ThingRequestGroup.Undefined);
		public override PathEndMode PathEndMode => PathEndMode.ClosestTouch;
		public override Danger MaxPathDanger(Pawn pawn)
		{
			return Danger.Deadly;
		}
		public override IEnumerable<Thing> PotentialWorkThingsGlobal(Pawn pawn)
		{
            return pawn.Map.mapPawns.SpawnedPawnsInFaction(Faction.OfPlayer).Where(x => x.CurJobDef == CP_DefOf.CP_CasevacRescue && x.CurJob.targetA.Pawn.CurrentBed() != x.CurJob.targetB.Thing);
        }
        public override bool HasJobOnThing(Pawn pawn, Thing t, bool forced = false)
        {
            Pawn pawn2 = t as Pawn;
            if (pawn2 == null || pawn2 == pawn)
            {
                return false;
            }

            if (pawn2.CurJobDef == CP_DefOf.CP_CasevacRescue && pawn2.jobs.curDriver is JobDriver_CasevacRescue casevacRescue)
            {
                if (casevacRescue.Rescuers.Count >= 4)
                {
                    return false;
                }
                var mainRescuee = casevacRescue.MainRescuee;
                if (mainRescuee != null && pawn != mainRescuee)
                {
                    var pawnTicksPerMove = pawn.TicksPerMove(false);
                    var mainRescueeTicksPerMove = mainRescuee.TicksPerMove(false);
                    if (pawnTicksPerMove > mainRescueeTicksPerMove)
                    {
                        return false;
                    }
                }
            }
            return true;
        }
		public override Job JobOnThing(Pawn pawn, Thing t, bool forced = false)
		{
			Pawn rescuee = (Pawn)t;
			if (rescuee != null)
            {
				Job job = JobMaker.MakeJob(CP_DefOf.CP_CasevacRescue, rescuee.CurJob.targetA, rescuee.CurJob.targetB);
                job.count = 1;
				return job;
			}
			return null;
		}
	}
}