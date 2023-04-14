using Verse;

namespace TribalMedicineRevamped
{
    public class HediffComp_SurviveHeartAttack : HediffComp
    {
        public override void CompPostTick(ref float severityAdjustment)
        {
            base.CompPostTick(ref severityAdjustment);
            if (Pawn.health.hediffSet.HasHediff(DefDatabase<HediffDef>.GetNamed("HeartAttack")))
            {
                Hediff heartAttack = Pawn.health.hediffSet.GetFirstHediffOfDef(DefDatabase<HediffDef>.GetNamed("HeartAttack"));

                heartAttack.Severity -= 0.00001f;
            }
        }
    }
}
