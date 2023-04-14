using RimWorld;
using Verse;

namespace AOBAUtilities
{
    public class IngredientValueGetter_Mass : IngredientValueGetter
    {
		public override string BillRequirementsDescription(RecipeDef r, IngredientCount ing)
		{
			return "FT_BillRequires_Mass".Translate(ing.GetBaseCount()) + " (" + ing.filter.Summary + ")";
		}

		public override float ValuePerUnitOf(ThingDef t)
		{
			if (t.BaseMass <=0)
			{
				return 0f;
			}
			return t.GetStatValueAbstract(StatDefOf.Mass);
		}
	}
	public class IngredientValueGetter_MarketValue : IngredientValueGetter
	{
		public override string BillRequirementsDescription(RecipeDef r, IngredientCount ing)
		{
			return "FT_BillRequires_MarketValue".Translate(ing.GetBaseCount()) + " (" + ing.filter.Summary + ")";
		}

		public override float ValuePerUnitOf(ThingDef t)
		{
			if (t.BaseMarketValue <= 0)
			{
				return 0f;
			}
			return t.GetStatValueAbstract(StatDefOf.MarketValue);
		}
	}

}
