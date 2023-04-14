using Verse;

namespace TribalMedicineRevamped
{
    public class HediffComp_EndureGutWorms : HediffComp
    {
        public override void CompPostTick(ref float severityAdjustment)
        {
            base.CompPostTick(ref severityAdjustment);
            {
                if (parent.pawn.health.hediffSet.HasHediff(DefDatabase<HediffDef>.GetNamed("TM_HiddenHediff_GutWormsEndurance")) && !Pawn.health.hediffSet.HasHediff(DefDatabase<HediffDef>.GetNamed("GutWorms")))
                {
                    Hediff endurance = parent.pawn.health.hediffSet.GetFirstHediffOfDef(DefDatabase<HediffDef>.GetNamed("TM_HiddenHediff_GutWormsEndurance"));
                    Pawn.health.RemoveHediff(endurance);
                }
            }
        }

        public override void CompPostPostAdd(DamageInfo? dinfo)
        {
            base.CompPostPostAdd(dinfo);
            if (Pawn.health.hediffSet.HasHediff(DefDatabase<HediffDef>.GetNamed("GutWorms")))
            {
                Pawn.health.AddHediff(DefDatabase<HediffDef>.GetNamed("TM_HiddenHediff_GutWormsEndurance"));
            }
        }
    }
}
