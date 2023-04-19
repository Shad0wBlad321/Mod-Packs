using System.Collections.Generic;
using RimWorld;
using Verse;

namespace RitualRewards.Sinnamon_Ritual;

public class RitualAttachableOutcomeEffectWorker_Eclipse : RitualAttachableOutcomeEffectWorker
{
    public override void Apply(Dictionary<Pawn, int> totalPresence, LordJob_Ritual jobRitual, OutcomeChance outcome, out string extraOutcomeDesc, ref LookTargets letterLookTargets)
    {
        jobRitual.Map.gameConditionManager.GetActiveCondition<GameCondition_DisableElectricity>()?.End();
        GameCondition_NoSunlight cond = (GameCondition_NoSunlight)GameConditionMaker.MakeCondition(GameConditionDefOf.Eclipse, 120000);
        jobRitual.Map.GameConditionManager.RegisterCondition(cond);
        if (outcome.BestPositiveOutcome(jobRitual))
        {
            GameCondition_Aurora cond2 = (GameCondition_Aurora)GameConditionMaker.MakeCondition(GameConditionDefOf.Aurora, 30000);
            jobRitual.Map.GameConditionManager.RegisterCondition(cond2);
            extraOutcomeDesc = "Sinnamon_EclipseWithAurora".Translate();
        }
        else
        {
            extraOutcomeDesc = def.letterInfoText;
        }
    }
}
