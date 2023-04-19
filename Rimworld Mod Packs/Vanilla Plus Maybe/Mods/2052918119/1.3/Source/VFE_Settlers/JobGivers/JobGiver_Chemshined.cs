using RimWorld;
using Verse;
using Verse.AI;

namespace VFE_Settlers.JobGivers
{
    public class JobGiver_Chemshined : ThinkNode_JobGiver
    {
        protected override Job TryGiveJob(Pawn pawn)
        {
            if (Rand.Value < 0.1f && VFESMod.settings.Chemsined)
            {
                Thing thing = Utilities.UtilityAction.GetDrunkenTarget(pawn, pawn.Position);
                if (thing != null)
                {
                    Job job = new Job(JobDefOf.AttackMelee, thing)
                    {
                        maxNumMeleeAttacks = Rand.Range(4, 12)
                    };
                    return job;
                }
            }
            return null;
        }
    }
}