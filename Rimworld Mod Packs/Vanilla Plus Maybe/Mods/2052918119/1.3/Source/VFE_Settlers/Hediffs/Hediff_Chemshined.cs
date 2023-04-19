using RimWorld;
using Verse;

namespace VFE_Settlers.Hediffs
{
    public class Hediff_Chemshined : HediffWithComps
    {
        public override void Tick()
        {
            base.Tick();
            if (this.pawn.IsHashIntervalTick(300))
            {
                if (this.Severity >= 0.75f)
                {
                    Hediff hediff = this.pawn.health.hediffSet.GetFirstHediffOfDef(HediffDefOf.Hangover, false);
                    if (hediff != null)
                    {
                        hediff.Severity = 1f;
                        return;
                    }
                    else
                    {
                        hediff = HediffMaker.MakeHediff(HediffDefOf.Hangover, this.pawn, null);
                        hediff.Severity = 1f;
                        this.pawn.health.AddHediff(hediff, null, null, null);
                        return;
                    }
                }
                if (this.Severity < 0.05f)
                {
                    this.pawn.health.RemoveHediff(this);
                }
            }
        }
    }
}