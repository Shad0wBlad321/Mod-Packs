using RimWorld;
using Verse;
using Verse.AI;

namespace VFE_Settlers.ThinkNodes
{
    public class ThinkNode_ConditionalChemshinedMuffalo : ThinkNode_Conditional
    {
        protected override bool Satisfied(Pawn pawn)
        {
            return pawn.kindDef == PawnKindDefOf.Muffalo && pawn.health.hediffSet.GetFirstHediffOfDef(Defs.HediffDefOf.Chemshined, false) != null && pawn.health.hediffSet.GetFirstHediffOfDef(Defs.HediffDefOf.Chemshined, false).Severity > 60f;
        }
    }
}