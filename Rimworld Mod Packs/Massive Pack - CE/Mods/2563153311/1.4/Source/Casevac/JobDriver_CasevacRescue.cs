using RimWorld;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
using Verse.AI;

namespace Casevac
{
    public class JobDriver_CasevacRescue : JobDriver
    {
        private const TargetIndex TakeeIndex = TargetIndex.A;

        private const TargetIndex BedIndex = TargetIndex.B;
        protected Pawn Takee => (Pawn)job.GetTarget(TargetIndex.A).Thing;
        protected Building_Bed DropBed => (Building_Bed)job.GetTarget(TargetIndex.B).Thing;

        [TweakValue("0CASE", 0f, 2f)] public static float closestDistance = 0.5f;

        private List<Pawn> rescuers = new List<Pawn>();
        public Pawn MainRescuee
        {
            get
            {
                Pawn mainRescuee = (Takee.ParentHolder as Pawn_CarryTracker)?.pawn;
                if (mainRescuee is null)
                {
                    mainRescuee = Rescuers.OrderBy(x => x.Position.DistanceTo(Takee.Position)).FirstOrDefault();
                }
                return mainRescuee;
            }
        }
        public Pawn Taker => (Takee.ParentHolder as Pawn_CarryTracker)?.pawn;
        public List<Pawn> Rescuers
        {
            get
            {
                rescuers.RemoveAll(x => x.CurJobDef != CP_DefOf.CP_CasevacRescue);
                return rescuers;
            }
        }
        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
            if (rescuers is null)
            {
                rescuers = new List<Pawn>();
            }
            if (!Rescuers.Contains(pawn))
            {
                rescuers.Add(pawn);
            }
            IEnumerable<Pawn> otherRescuers = pawn.Map.mapPawns.SpawnedPawnsInFaction(Faction.OfPlayer).Where(x => x.CurJobDef == CP_DefOf.CP_CasevacRescue
                    && x.CurJob.targetA.Thing == TargetA.Thing && x.CurJob.targetB.Thing != null);

            foreach (Pawn otherPawn in otherRescuers)
            {
                if (otherPawn != null && otherPawn.jobs.curDriver is JobDriver_CasevacRescue casevacRescue)
                {
                    if (!casevacRescue.Rescuers.Contains(pawn))
                    {
                        casevacRescue.rescuers.Add(pawn);
                    }
                    if (!Rescuers.Contains(otherPawn))
                    {
                        rescuers.Add(otherPawn);
                    }
                }
            }

            return Rescuers.Any(x => pawn.Map.reservationManager.ReservedBy(DropBed, x))
|| pawn.Reserve(DropBed, job, DropBed.SleepingSlotsCount, 0, null, errorOnFailed);
        }

        private void ReleaseAndMakeOtherReserveBed()
        {
            if (pawn.Map.reservationManager.ReservedBy(DropBed, pawn, pawn.CurJob))
            {
                pawn.Map.reservationManager.Release(DropBed, pawn, pawn.CurJob);
            }
            rescuers.Remove(pawn);
            Pawn mainRescuee = MainRescuee;
            if (mainRescuee != null && Takee.CurrentBed() != DropBed && mainRescuee.CanReserve(DropBed))
            {
                mainRescuee.Reserve(DropBed, mainRescuee.CurJob, DropBed.SleepingSlotsCount, 0, null, true);
            }
        }

        public override void Notify_PatherFailed()
        {
            //Log.Error(pawn + " - failed to path END");
            //Find.TickManager.CurTimeSpeed = TimeSpeed.Paused;
            //EndJobWith(JobCondition.ErroredPather);
        }
        public bool ShouldEndJob()
        {
            Building_Bed curBed = Takee.CurrentBed();
            if (curBed == job.targetB.Thing)
            {
                return true;
            }
            if (!Rescuers.Any(x => pawn.Map.reservationManager.ReservedBy(DropBed, x)))
            {
                pawn.Reserve(DropBed, job, DropBed.SleepingSlotsCount, 0, null, true);
            }

            Pawn mainRescuee = MainRescuee;
            if (mainRescuee != null && pawn != mainRescuee)
            {
                int pawnTicksPerMove = pawn.TicksPerMove(false);
                int mainRescueeTicksPerMove = mainRescuee.TicksPerMove(false);
                if (pawnTicksPerMove > mainRescueeTicksPerMove)
                {
                    return true;
                }
            }
            return false;
        }

        protected override IEnumerable<Toil> MakeNewToils()
        {
            this.FailOnDestroyedOrNull(TargetIndex.A);
            this.FailOnDestroyedOrNull(TargetIndex.B);
            AddEndCondition(() => ShouldEndJob() ? JobCondition.Succeeded : JobCondition.Ongoing);
            AddFinishAction(() => ReleaseAndMakeOtherReserveBed());
            yield return Toils_Bed.ClaimBedIfNonMedical(TargetIndex.B, TargetIndex.A);
            Toil toil = new Toil
            {
                tickAction = delegate
            {
                if (TargetA.Thing.ParentHolder is Pawn_CarryTracker pawn_CarryTracker)
                {
                    if (!pawn.pather.MovingNow)
                    {
                        if (pawn_CarryTracker.pawn != pawn)
                        {
                            if (!Pawn_PathFollower_StopDead.lastPathTicks.TryGetValue(pawn, out int value) || value != Find.TickManager.TicksGame)
                            {
                                pawn.pather.StartPath(pawn_CarryTracker.pawn, PathEndMode.OnCell);
                            }
                        }
                        else
                        {
                            pawn.pather.StartPath(DropBed, PathEndMode.ClosestTouch);
                        }
                    }
                }
                else if (TargetA.Thing != pawn.pather.Destination.Thing || !pawn.pather.MovingNow)
                {
                    pawn.pather.StartPath(TargetA, PathEndMode.OnCell);
                }
            },
                socialMode = RandomSocialMode.Off,
                defaultCompleteMode = ToilCompleteMode.PatherArrival
            };
            yield return toil;

            yield return Toils_Jump.JumpIf(toil, () => TargetA.Thing.ParentHolder is Pawn_CarryTracker pawn_CarryTracker && pawn_CarryTracker.pawn != pawn);

            Toil toil2 = Toils_Haul.StartCarryThing(TargetIndex.A);
            toil2.FailOnBedNoLongerUsable(TargetIndex.B, TargetIndex.A);
            toil2.AddPreInitAction(CheckMakeTakeeGuest);
            yield return toil2;

            yield return Toils_Goto.GotoThing(TargetIndex.B, PathEndMode.Touch);
            Toil toil3 = new Toil
            {
                initAction = delegate
            {
                CheckMakeTakeePrisoner();
                if (Takee.playerSettings == null)
                {
                    Takee.playerSettings = new Pawn_PlayerSettings(Takee);
                }
            }
            };
            yield return toil3;
            yield return new Toil
            {
                initAction = delegate ()
                {
                    foreach (Pawn rescuee in Rescuers)
                    {
                        if (rescuee.Map.reservationManager.ReservedBy(DropBed, rescuee))
                        {
                            rescuee.Map.reservationManager.Release(DropBed, rescuee, rescuee.CurJob);
                        }
                    }
                }
            };
            Toil toil4 = new Toil
            {
                initAction = delegate
            {
                foreach (Pawn rescuee in Rescuers)
                {
                    if (rescuee.Map.reservationManager.ReservedBy(DropBed, rescuee))
                    {
                        rescuee.Map.reservationManager.Release(DropBed, rescuee, rescuee.CurJob);
                    }
                }

                foreach (Pawn rescuee in Rescuers)
                {
                    Takee.needs?.mood?.thoughts?.memories?.TryGainMemory(CP_DefOf.CP_RescuedMe, rescuee);
                    foreach (Pawn rescuee2 in Rescuers)
                    {
                        if (rescuee != rescuee2)
                        {
                            rescuee.needs?.mood?.thoughts?.memories?.TryGainMemory(CP_DefOf.CP_RescuedTogether, rescuee2);
                        }
                    }
                }

                IntVec3 position = DropBed.Position;
                pawn.carryTracker.TryDropCarriedThing(position, ThingPlaceMode.Direct, out Thing _);
                if (!DropBed.Destroyed && (DropBed.OwnersForReading.Contains(Takee) || (DropBed.Medical && DropBed.AnyUnoccupiedSleepingSlot) || Takee.ownership == null))
                {
                    Takee.jobs.Notify_TuckedIntoBed(DropBed);
                    if (Takee.RaceProps.Humanlike && job.def != JobDefOf.Arrest && !Takee.IsPrisonerOfColony)
                    {
                        Takee.relations.Notify_RescuedBy(pawn);
                    }
                    Takee.mindState.Notify_TuckedIntoBed();
                }
            },
                defaultCompleteMode = ToilCompleteMode.Instant
            };
            yield return toil4;
        }
        private void CheckMakeTakeePrisoner()
        {
            if (Takee.HostileTo(Faction.OfPlayer) && !Takee.IsPrisonerOfColony)
            {
                if (Takee.guest.Released)
                {
                    Takee.guest.Released = false;
                    Takee.guest.interactionMode = PrisonerInteractionModeDefOf.NoInteraction;
                    GenGuest.RemoveHealthyPrisonerReleasedThoughts(Takee);
                }
                if (!Takee.IsPrisonerOfColony)
                {
                    Takee.guest.CapturedBy(Faction.OfPlayer, pawn);
                }
            }
        }

        private void CheckMakeTakeeGuest()
        {
            if (!Takee.HostileTo(Faction.OfPlayer) && Takee.Faction != Faction.OfPlayer && Takee.HostFaction != Faction.OfPlayer && Takee.guest != null && !Takee.IsWildMan())
            {
                Takee.guest.SetGuestStatus(Faction.OfPlayer);
            }
        }

        [TweakValue("0CASEVAC", -1f, 1f)] public static float vectorX;
        [TweakValue("0CASEVAC", -1f, 1f)] public static float vectorZ;
        public override bool ModifyCarriedThingDrawPos(ref Vector3 drawPos, ref bool behind, ref bool flip)
        {
            drawPos += new Vector3(vectorX, 0, vectorZ);
            return true;
        }
    }
}