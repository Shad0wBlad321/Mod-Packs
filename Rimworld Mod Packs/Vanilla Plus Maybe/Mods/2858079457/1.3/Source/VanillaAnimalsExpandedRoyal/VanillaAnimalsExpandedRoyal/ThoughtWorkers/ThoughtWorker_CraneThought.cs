
using RimWorld;
using Verse;

namespace VanillaAnimalsExpandedRoyal
{
	public class ThoughtWorker_CraneThought : ThoughtWorker
	{
		protected override ThoughtState CurrentStateInternal(Pawn p)
		{

			if (!p.royalty.HasAnyTitleIn(Faction.OfEmpire))
			{
				return false;
			}
			if (StaticCollectionsClass.ownedCranesInMap<5)
			{
				return false;
			}
			return true;
		}
	}
}
