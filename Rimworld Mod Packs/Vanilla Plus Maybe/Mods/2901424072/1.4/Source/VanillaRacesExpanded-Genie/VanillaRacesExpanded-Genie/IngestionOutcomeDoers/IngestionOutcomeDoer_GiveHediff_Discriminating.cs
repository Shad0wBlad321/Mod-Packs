
using RimWorld;
using System.Collections.Generic;
using Verse;
namespace VanillaRacesExpandedGenies
{
	public class IngestionOutcomeDoer_GiveHediff_Discriminating : IngestionOutcomeDoer
	{
		public HediffDef hediffDef;
		public HediffDef firstAlternateHediffDef;
		public HediffDef secondAlternateHediffDef;
		public HediffDef thirdAlternateHediffDef;



		public float severity = -1f;

		public ChemicalDef toleranceChemical;

		private bool divideByBodySize;

		public bool multiplyByGeneToleranceFactors;

		protected override void DoIngestionOutcomeSpecial(Pawn pawn, Thing ingested)
		{
			Hediff hediff;

			hediff = HediffMaker.MakeHediff(hediffDef, pawn);
			if (pawn.genes?.HasGene(InternalDefOf.VRE_Immunity_VeryWeak) == true && pawn.genes?.GetGene(InternalDefOf.VRE_Immunity_VeryWeak).Active==true)
			{
				hediff = HediffMaker.MakeHediff(firstAlternateHediffDef, pawn);
			}
			if (DefDatabase<GeneDef>.GetNamedSilentFail("AG_Gene_VeryWeakImmunity") !=null &&pawn.genes?.HasGene(DefDatabase<GeneDef>.GetNamed("AG_Gene_VeryWeakImmunity")) == true 
				&& pawn.genes?.GetGene(DefDatabase<GeneDef>.GetNamed("AG_Gene_VeryWeakImmunity")).Active == true)
			{
				hediff = HediffMaker.MakeHediff(secondAlternateHediffDef, pawn);
			}
			if (pawn.genes?.HasGene(InternalDefOf.Immunity_Weak) == true && pawn.genes?.GetGene(InternalDefOf.Immunity_Weak).Active == true)
			{
				hediff = HediffMaker.MakeHediff(thirdAlternateHediffDef, pawn);
			}


			float effect = (!(severity > 0f)) ? hediffDef.initialSeverity : severity;
			if (divideByBodySize)
			{
				effect /= pawn.BodySize;
			}
			AddictionUtility.ModifyChemicalEffectForToleranceAndBodySize_NewTemp(pawn, toleranceChemical, ref effect, multiplyByGeneToleranceFactors);
			hediff.Severity = effect;
			pawn.health.AddHediff(hediff);
		}

		public override IEnumerable<StatDrawEntry> SpecialDisplayStats(ThingDef parentDef)
		{
			if (parentDef.IsDrug && chance >= 1f)
			{
				foreach (StatDrawEntry item in hediffDef.SpecialDisplayStats(StatRequest.ForEmpty()))
				{
					yield return item;
				}
			}
		}
	}
}