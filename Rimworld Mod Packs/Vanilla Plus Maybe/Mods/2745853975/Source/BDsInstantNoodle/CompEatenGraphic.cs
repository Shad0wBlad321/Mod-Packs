using Verse;

namespace BDsInstantNoodle;

public class CompEatenGraphic : ThingComp
{
	private Graphic graphic;

	public CompProperties_EatenGraphic Props => props as CompProperties_EatenGraphic;

	public Graphic Graphic => graphic ?? (graphic = Props.graphicData.Graphic);
}
