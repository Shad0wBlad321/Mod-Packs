using RimWorld;
using Verse;

namespace BDsInstantNoodle;

public class InstantNoodle : ThingWithComps
{
	private Graphic graphic;

	public override Graphic Graphic
	{
		get
		{
			if (base.ParentHolder is Pawn_CarryTracker pawn_CarryTracker && !pawn_CarryTracker.pawn.pather.Moving && pawn_CarryTracker.pawn.CurJobDef == JobDefOf.Ingest)
			{
				return graphic ?? (graphic = GetComp<CompEatenGraphic>().Graphic);
			}
			return base.Graphic;
		}
	}
}
