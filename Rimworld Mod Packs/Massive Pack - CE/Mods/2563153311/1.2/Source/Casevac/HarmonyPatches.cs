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

namespace Casevac
{
	[StaticConstructorOnStartup]
	internal static class HarmonyPathces
	{
		static HarmonyPathces()
		{
			new Harmony("CP.Casevac").PatchAll();
		}
		public static bool TryGetNewMovement(Pawn pawn, out int result, bool diagonal)
        {
			if (pawn.jobs?.curDriver is JobDriver_CasevacRescue casevacRescue)
			{
				var taker = casevacRescue.Taker;
				if (taker != null)
				{
					var closestDistance = JobDriver_CasevacRescue.closestDistance;
					if (diagonal)
					{
						closestDistance *= GenMath.Sqrt2;
					}
					if (taker != pawn)
					{
						if (pawn.Position.DistanceTo(taker.Position) <= closestDistance)
						{
							if (diagonal)
                            {
								result = taker.TicksPerMoveDiagonal;
                            }
							else 
							{
								result = taker.TicksPerMoveCardinal;
							}
							return true;
						}
					}
					else
					{
						result = pawn.TicksPerMove(diagonal: diagonal);
						var bonus = casevacRescue.Rescuers.Where(x => x != pawn && x.Position.DistanceTo(pawn.Position) <= closestDistance).Sum(x => x.GetStatValue(StatDefOf.MoveSpeed));
						if (bonus != 0)
						{
							result -= (int)bonus;
							return true;
						}
					}
				}
			}
			result = 0;
			return false;
		}
	}
	[HarmonyPatch(typeof(Pawn), "TicksPerMoveCardinal", MethodType.Getter)]
	public static class TicksPerMoveCardinal_Patch
	{
		public static bool Prefix(Pawn __instance, ref int __result)
		{
			if (HarmonyPathces.TryGetNewMovement(__instance, out int result, false))
			{
				__result = result;
				return false;
			}
			return true;
		}
	}

	[HarmonyPatch(typeof(Pawn), "TicksPerMoveDiagonal", MethodType.Getter)]
	public static class TicksPerMoveDiagonal_Patch
	{
		public static bool Prefix(Pawn __instance, ref int __result)
		{
			if (HarmonyPathces.TryGetNewMovement(__instance, out int result, true))
			{
				__result = result;
				return false;
			}
			return true;
		}
	}
	[HarmonyPatch(typeof(FloatMenuMakerMap), "AddHumanlikeOrders")]
	public static class FloatMenuMakerCarryAdder
	{
		public static void Postfix(Vector3 clickPos, Pawn pawn, ref List<FloatMenuOption> opts)
		{
			if (pawn.health.capacities.CapableOf(PawnCapacityDefOf.Manipulation))
			{
				foreach (LocalTargetInfo item11 in GenUI.TargetsAt_NewTemp(clickPos, TargetingParameters.ForRescue(pawn), thingsOnly: true))
				{
					Pawn victim3 = (Pawn)item11.Thing;
					if (!victim3.InBed() && pawn.CanReserveAndReach(victim3, PathEndMode.OnCell, Danger.Deadly, 1, -1, null, ignoreOtherReservations: true) 
						&& !victim3.mindState.WillJoinColonyIfRescued)
					{
						if (!victim3.IsPrisonerOfColony && (!victim3.InMentalState || victim3.health.hediffSet.HasHediff(HediffDefOf.Scaria)) 
							&& (victim3.Faction == Faction.OfPlayer || victim3.Faction == null || !victim3.Faction.HostileTo(Faction.OfPlayer)))
						{
							opts.Add(FloatMenuUtility.DecoratePrioritizedTask(new FloatMenuOption("RH.CASEVAC".Translate(victim3.Named("PAWN")), delegate
							{
								Building_Bed building_Bed2 = RestUtility.FindBedFor(victim3, pawn, sleeperWillBePrisoner: false, checkSocialProperness: false);
								if (building_Bed2 == null)
								{
									building_Bed2 = RestUtility.FindBedFor(victim3, pawn, sleeperWillBePrisoner: false, checkSocialProperness: false, ignoreOtherReservations: true);
								}
								if (building_Bed2 == null)
								{
									string t3 = (!victim3.RaceProps.Animal) ? ((string)"NoNonPrisonerBed".Translate()) : ((string)"NoAnimalBed".Translate());
									Messages.Message("CannotRescue".Translate() + ": " + t3, victim3, MessageTypeDefOf.RejectInput, historical: false);
								}
								else
								{
									Job job = JobMaker.MakeJob(CP_DefOf.CP_CasevacRescue, victim3, building_Bed2);
									job.count = 1;
									pawn.jobs.TryTakeOrderedJob(job);
									int num = 1;
									foreach (var otherPawn in GenRadial.RadialDistinctThingsAround(pawn.Position, pawn.Map, 10f, true).OfType<Pawn>().OrderByDescending(x => x.GetStatValue(StatDefOf.MoveSpeed)))
                                    {
										if (otherPawn != pawn && otherPawn.mindState?.lastJobTag == JobTag.Idle && !otherPawn.InMentalState && !otherPawn.Drafted 
											&& !otherPawn.Downed && otherPawn.IsColonistPlayerControlled && GenSight.LineOfSight(otherPawn.Position, pawn.Position, pawn.Map))
                                        {
											Job job2 = JobMaker.MakeJob(CP_DefOf.CP_CasevacRescue, victim3, building_Bed2);
											job2.count = 1;
											otherPawn.jobs.TryTakeOrderedJob(job2);
											num++;
											if (num > 4)
                                            {
												break;
                                            }
										}
                                    }
								}
							}, MenuOptionPriority.RescueOrCapture, null, victim3), pawn, victim3));
						}
					}
				}
			}
		}
	}
	[HarmonyPatch(typeof(Pawn_PathFollower))]
	[HarmonyPatch("StopDead")]
	public class Pawn_PathFollower_StopDead
	{
		public static Dictionary<Pawn, int> lastPathTicks = new Dictionary<Pawn, int>();
		private static bool Prefix(Pawn_PathFollower __instance)
		{
			if (__instance.pawn.jobs?.curDriver is JobDriver_CasevacRescue jobDriver_CasevacRescue)
			{
				if (jobDriver_CasevacRescue.TargetA.Thing.ParentHolder is Pawn_CarryTracker pawn_CarryTracker
					&& pawn_CarryTracker.pawn != __instance.pawn && (!lastPathTicks.TryGetValue(__instance.pawn, out var value) || Find.TickManager.TicksGame != value))
				{
					if (__instance.curPath != null)
					{
						__instance.curPath.ReleaseToPool();
					}
					__instance.curPath = null;
					__instance.moving = false;
					__instance.nextCell = __instance.pawn.Position;
					var nodeLeftCount = pawn_CarryTracker.pawn.pather.curPath.NodesLeftCount;
					var pos = nodeLeftCount - 1 > 0 ? pawn_CarryTracker.pawn.pather.curPath.nodes[nodeLeftCount - 2] : pawn_CarryTracker.pawn.Position;
					lastPathTicks[__instance.pawn] = Find.TickManager.TicksGame;
					__instance.pawn.pather.StartPath(pos, PathEndMode.OnCell);
					return false;
				}
			}
			return true;
		}
	}
}
