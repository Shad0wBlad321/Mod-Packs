using System.Collections.Generic;
using RimWorld;
using Verse;

namespace RitualRewards.Sinnamon_Ritual;

public class RitualAttachableOutcomeEffectWorker_Aurora : RitualAttachableOutcomeEffectWorker
{
    public override void Apply(Dictionary<Pawn, int> totalPresence, LordJob_Ritual jobRitual, OutcomeChance outcome, out string extraOutcomeDesc, ref LookTargets letterLookTargets)
    {
        int num = outcome.BestPositiveOutcome(jobRitual) ? 4 : 2;
        Sinnamon_GameCondition_Aurora cond = (Sinnamon_GameCondition_Aurora)GameConditionMaker.MakeCondition(GameConditionDefOf.Aurora, num * 60000);
        jobRitual.Map.GameConditionManager.RegisterCondition(cond);
        extraOutcomeDesc = def.letterInfoText + $" {num} days.";
    }
}
