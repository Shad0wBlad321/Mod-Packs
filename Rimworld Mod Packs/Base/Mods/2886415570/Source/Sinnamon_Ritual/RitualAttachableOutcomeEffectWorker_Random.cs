using System.Collections.Generic;
using RimWorld;
using Verse;

namespace RitualRewards.Sinnamon_Ritual;

public class RitualAttachableOutcomeEffectWorker_Random : RitualAttachableOutcomeEffectWorker
{
    public override void Apply(Dictionary<Pawn, int> totalPresence, LordJob_Ritual jobRitual, OutcomeChance outcome, out string extraOutcomeDesc, ref LookTargets letterLookTargets)
    {
        List<RitualAttachableOutcomeEffectWorker> list = new();
        foreach (RitualAttachableOutcomeEffectDef allDef in DefDatabase<RitualAttachableOutcomeEffectDef>.AllDefs)
        {
            if (allDef.defName is not "Sinnamon_Random" and not "Sinnamon_Aurora")
                list.Add(allDef.Worker);
        }

        RitualAttachableOutcomeEffectWorker ritualAttachableOutcomeEffectWorker = list.RandomElement();
        ritualAttachableOutcomeEffectWorker.Apply(totalPresence, jobRitual, outcome, out string extraOutcomeDesc2, ref letterLookTargets);
        extraOutcomeDesc = "Sinnamon_Random".Translate(ritualAttachableOutcomeEffectWorker.def.label) + extraOutcomeDesc2;
    }
}
