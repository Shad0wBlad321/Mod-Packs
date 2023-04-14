using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using RimWorld;
using Verse;

namespace TribalMedicineRevamped
{
    public class IngestionOutcomeDoer_ReduceIllnessPercentage : IngestionOutcomeDoer
    {
        protected override void DoIngestionOutcomeSpecial(Pawn pawn, Thing ingested)
        {
            Hediff hediffToAffect = pawn.health.hediffSet.hediffs.Find(h => h.def.HasComp(typeof(HediffComp_Immunizable)));

            if (hediffToAffect != null)
            {
                hediffToAffect.Severity -= hediffToAffect.Severity * 0.12f;
            }
        }
    }
}
