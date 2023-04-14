using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using RimWorld;
using Verse;

namespace TribalMedicineRevamped
{
    public class IngestionOutcomeDoer_CureFoodPoisoning : IngestionOutcomeDoer
    {
        protected override void DoIngestionOutcomeSpecial(Pawn pawn, Thing ingested) // Need to add checking.
        {
            Hediff foodPoisoning = pawn.health.hediffSet.GetFirstHediffOfDef(HediffDefOf.FoodPoisoning);

            if (foodPoisoning != null && foodPoisoning.Severity < 0.15f)
            {
                foodPoisoning.Severity = 0.15f;
            }
        }
    }
}
