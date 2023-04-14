using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using Verse;
using Verse.AI;
using Verse.Sound;
using static UnityEngine.GraphicsBuffer;

namespace BillDoorsFramework
{
    public class CompChangeableProjectileForLGIS : CompChangeableProjectile
    {
        public CompProperties_ChangeableProjectileForLGIS Props => (CompProperties_ChangeableProjectileForLGIS)props;
        public bool Loaded => loadedCount < Props.magazineSize;
    }

    public class CompProperties_ChangeableProjectileForLGIS : CompProperties
    {
        public CompProperties_ChangeableProjectileForLGIS()
        {
            compClass = typeof(CompChangeableProjectileForLGIS);
        }

        public int magazineSize = 1;
    }

    public class WorkGiver_ReloadLGIS : WorkGiver_Scanner
    {
        public override ThingRequest PotentialWorkThingRequest => ThingRequest.ForGroup(ThingRequestGroup.BuildingArtificial);

        public override bool HasJobOnThing(Pawn pawn, Thing t, bool forced = false)
        {
            if (!t.Spawned || t.IsForbidden(pawn))
            {
                return false;
            }
            if (!pawn.CanReserve(t, 1, -1, null, forced))
            {
                return false;
            }
            if (t is Building_TurretGun turret)
            {
                CompChangeableProjectileForLGIS comp = turret.gun.TryGetComp<CompChangeableProjectileForLGIS>();
                if (comp != null && !comp.Loaded)
                {
                    return true;
                }
            }
            return false;
        }

        public override Job JobOnThing(Pawn pawn, Thing t, bool forced = false)
        {
            if (t is Building_TurretGun turret)
            {
                CompChangeableProjectileForLGIS comp = turret.gun.TryGetComp<CompChangeableProjectileForLGIS>();
                if (comp != null && !comp.Loaded)
                {
                    Job job = JobMaker.MakeJob(JobDefOf.BDF_RearmTurretWithMagazine, turret.gun);
                    return job;
                }
            }
            return null;
        }
    }

    public class JobDriver_ReloadLGIS : JobDriver
    {
        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
            pawn.Reserve(job.GetTarget(TargetIndex.A).Thing, job);
            return true;
        }

        public static Thing FindAmmoForTurret(Pawn pawn, Building_TurretGun gun)
        {
            StorageSettings allowedShellsSettings = (pawn.IsColonist ? gun.gun.TryGetComp<CompChangeableProjectile>().allowedShellsSettings : null);
            Predicate<Thing> validator = delegate (Thing t)
            {
                if (t.IsForbidden(pawn))
                {
                    return false;
                }
                if (!pawn.CanReserve(t, 10, 1))
                {
                    return false;
                }
                return (allowedShellsSettings == null || allowedShellsSettings.AllowedToAccept(t)) ? true : false;
            };
            return GenClosest.ClosestThingReachable(gun.Position, gun.Map, ThingRequest.ForGroup(ThingRequestGroup.Shell), PathEndMode.OnCell, TraverseParms.For(pawn), 40f, validator);
        }

        protected override IEnumerable<Toil> MakeNewToils()
        {
            CompChangeableProjectileForLGIS comp = job.GetTarget(TargetIndex.A).Thing.TryGetComp<CompChangeableProjectileForLGIS>();
            Toil loadIfNeeded = ToilMaker.MakeToil("MakeNewToils");
            loadIfNeeded.initAction = delegate
            {
                Pawn actor3 = loadIfNeeded.actor;
                Building obj = (Building)actor3.CurJob.targetA.Thing;
                Building_TurretGun building_TurretGun2 = obj as Building_TurretGun;
                Thing thing = FindAmmoForTurret(pawn, building_TurretGun2);
                if (thing == null)
                {
                    if (actor3.Faction == Faction.OfPlayer)
                    {
                        Messages.Message("MessageOutOfNearbyShellsFor".Translate(actor3.LabelShort, building_TurretGun2.Label, actor3.Named("PAWN"), building_TurretGun2.Named("GUN")).CapitalizeFirst(), building_TurretGun2, MessageTypeDefOf.NegativeEvent);
                    }
                    actor3.jobs.EndCurrentJob(JobCondition.Incompletable);
                }
                actor3.CurJob.targetB = thing;
                actor3.CurJob.count = 1;
            };
            yield return loadIfNeeded;
            yield return Toils_Reserve.Reserve(TargetIndex.B, 10, (comp.Props.magazineSize - comp.loadedCount));
            yield return Toils_Goto.GotoThing(TargetIndex.B, PathEndMode.OnCell).FailOnSomeonePhysicallyInteracting(TargetIndex.B);
            yield return Toils_Haul.StartCarryThing(TargetIndex.B);
            yield return Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.Touch);
            Toil loadShell = ToilMaker.MakeToil("MakeNewToils");
            loadShell.initAction = delegate
            {
                Pawn actor2 = loadShell.actor;
                Building_TurretGun building_TurretGun = ((Building)actor2.CurJob.targetA.Thing) as Building_TurretGun;
                SoundDefOf.Artillery_ShellLoaded.PlayOneShot(new TargetInfo(building_TurretGun.Position, building_TurretGun.Map));
                building_TurretGun.gun.TryGetComp<CompChangeableProjectile>().LoadShell(actor2.CurJob.targetB.Thing.def, actor2.CurJob.targetB.Thing.stackCount);
                actor2.carryTracker.innerContainer.ClearAndDestroyContents();
            };
            yield return loadShell;
        }
    }
}
