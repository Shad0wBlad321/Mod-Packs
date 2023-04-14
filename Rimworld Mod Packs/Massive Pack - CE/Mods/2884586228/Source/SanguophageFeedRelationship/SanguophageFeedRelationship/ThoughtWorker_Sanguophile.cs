using System; 
using RimWorld;
using Verse;

namespace SanguophageFeedRelationship
{
    public class ThoughtWorker_Sanguophile: ThoughtWorker
    {
		protected override ThoughtState CurrentSocialStateInternal(Pawn p, Pawn other)
		{
			if (!p.RaceProps.Humanlike)
			{
				return false;
			}
			if (!p.story.traits.HasTrait(SFR_DefOf.SFR_Sanguophile))
			{
				return false;
			}
			if (!RelationsUtility.PawnsKnowEachOther(p, other))
			{
				return false;
			}
			if (other.def != p.def)
			{
				return false;
			}
			if (!other.genes.HasGene(DefDatabase<GeneDef>.GetNamed("Bloodfeeder")))
			{
				return false;
			}
			return true;
		}
	}
}
