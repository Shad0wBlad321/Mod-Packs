using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using Verse;
using Verse.AI;

namespace Brainwash
{
    public class CompProperties_ChangePersonality : CompProperties
    {
        public List<TraitDef> traitsToExclude;
        public CompProperties_ChangePersonality()
        {
            compClass = typeof(CompChangePersonality);
        }
    }
    public class CompChangePersonality : ThingComp
    {
        public List<TraitEntry> traitsToSet;
        public List<SkillEntry> skillsToSet;
        public List<string> backstoriesToSet;
        public Pawn pawn => parent as Pawn;
        public CompProperties_ChangePersonality Props => props as CompProperties_ChangePersonality;
        public override IEnumerable<FloatMenuOption> CompFloatMenuOptions(Pawn selPawn)
        {
            if (pawn.IsPrisonerOfColony && pawn.CanBeBrainwashedBy(selPawn) && TryGetNearbyTelevisionAndChair(selPawn, out var televisionAndChair))
            {
                yield return new FloatMenuOption("Brainwash_TakeForBrainwashPersonality".Translate(), delegate
                {
                    Find.WindowStack.Add(new Window_ChangePersonality(this, delegate
                    {
                        Job job = JobMaker.MakeJob(BrainwashDefOf.RedHorse_LeadToBrainwashChairForPersonalityChange, 
                            pawn, televisionAndChair.Item2, televisionAndChair.Item1);
                        job.count = 1;
                        selPawn.jobs.TryTakeOrderedJob(job);
                    }));
                });
            }
        }

        public bool TryGetNearbyTelevisionAndChair(Pawn pather, out (Thing, Thing) televisionAndChair)
        {
            televisionAndChair = default;
            foreach (ThingDef def in Core.allTelevisions)
            {
                Thing television = GenClosest.ClosestThingReachable(pawn.Position, pawn.Map, ThingRequest.ForDef(def),
                    PathEndMode.Touch, TraverseParms.For(pather, Danger.Deadly, TraverseMode.ByPawn), 9999f,
                    (Thing x) => SuitableForBrainwashing(x) && FindChair(pather, x) != null);
                if (television != null)
                {
                    var chair = FindChair(pather, television);
                    televisionAndChair.Item1 = television;
                    televisionAndChair.Item2 = chair;
                    return true;
                }
            }
            return false;
        }

        public Thing FindChair(Pawn pather, Thing television)
        {
            if (pawn.IsPrisonerOfColony)
            {
                return GenClosest.ClosestThingReachable(pawn.Position, pawn.Map,
                    ThingRequest.ForDef(BrainwashDefOf.RH2_BrainwashChair),
                    PathEndMode.Touch, TraverseParms.For(pather, Danger.Deadly, TraverseMode.ByPawn), 9999f,
                    (Thing x) => pather.CanReserveSittableOrSpot(x.Position)
                    && WatchBuildingUtility.CalculateWatchCells(television.def, television.Position,
                    television.Rotation, television.Map).Contains(x.Position));
            }
            return GenClosest.ClosestThingReachable(parent.Position, parent.Map, ThingRequest.ForGroup(ThingRequestGroup.BuildingArtificial),
                PathEndMode.Touch, TraverseParms.For(pather, Danger.Deadly, TraverseMode.ByPawn), 9999f,
                (Thing x) => (x.def.building?.isSittable ?? false) && pawn.CanReserveSittableOrSpot(x.Position)
                && WatchBuildingUtility.CalculateWatchCells(television.def, television.Position, television.Rotation,
                television.Map).Contains(x.Position));
        }

        public bool SuitableForBrainwashing(Thing television)
        {
            return (television.TryGetComp<CompPowerTrader>() is null || television.TryGetComp<CompPowerTrader>().PowerOn);
        }

        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Collections.Look(ref traitsToSet, "traitsToSet", LookMode.Deep);
            Scribe_Collections.Look(ref skillsToSet, "skillsToSet", LookMode.Deep);
            Scribe_Collections.Look(ref backstoriesToSet, "backstoriesToSet", LookMode.Value);
        }
    }
}
