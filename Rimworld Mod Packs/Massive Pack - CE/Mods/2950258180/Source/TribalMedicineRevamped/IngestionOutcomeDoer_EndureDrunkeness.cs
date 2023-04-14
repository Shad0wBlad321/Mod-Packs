using RimWorld;
using Verse;

namespace TribalMedicineRevamped
{
    public class IngestionOutcomeDoer_EndureDrunkeness : IngestionOutcomeDoer
    {
        protected override void DoIngestionOutcomeSpecial(Pawn pawn, Thing ingested)
        {
            if (pawn.health.hediffSet.HasHediff(HediffDefOf.AlcoholHigh))
            {
                Hediff drunkeness = pawn.health.hediffSet.GetFirstHediffOfDef(HediffDefOf.AlcoholHigh);
                drunkeness.Severity -= 0.08f;
            }
        }
    }
}
