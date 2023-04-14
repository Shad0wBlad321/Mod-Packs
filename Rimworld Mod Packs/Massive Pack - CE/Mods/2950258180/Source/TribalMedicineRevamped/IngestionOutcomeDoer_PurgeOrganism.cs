using System.Collections.Generic;

using RimWorld;
using Verse;

namespace TribalMedicineRevamped
{
    internal class IngestionOutcomeDoer_PurgeOrganism : IngestionOutcomeDoer
    {
        private readonly float purgeChance = 0.5f;
        protected override void DoIngestionOutcomeSpecial(Pawn pawn, Thing ingested)
        {
            List<Hediff> pawnHediffs = pawn.health.hediffSet.hediffs;
            List<Hediff> affectedHediffs = new List<Hediff>();

            foreach (Hediff hediff in pawnHediffs)
            {
                if (hediff.CurStage.lifeThreatening && hediff.def.defName != "TM_BrootPoisoning")
                {
                    if (Rand.Chance(purgeChance + (hediff.Severity * 0.3f)))
                    {
                        affectedHediffs.Add(hediff);
                    }
                }
            }

            foreach (Hediff hediff in affectedHediffs)
            {
                pawn.health.RemoveHediff(hediff);
                Messages.Message("TM_MessageHediffCleansed".Translate(pawn.LabelShort, hediff.Label, pawn.Named("PAWN"), hediff.Named("HEDIFF")), pawn, MessageTypeDefOf.PositiveEvent, true);
            }

        }
    }
}
