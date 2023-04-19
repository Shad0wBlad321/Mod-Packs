using System.Collections.Generic;
using RimWorld;
using Verse;

namespace RitualRewards.Sinnamon_Ritual;

public class RitualAttachableOutcomeEffectWorker_Dust : RitualAttachableOutcomeEffectWorker
{
    public override void Apply(Dictionary<Pawn, int> totalPresence, LordJob_Ritual jobRitual, OutcomeChance outcome, out string extraOutcomeDesc, ref LookTargets letterLookTargets)
    {
        List<Thing> list = jobRitual.Map.listerThings.ThingsInGroup(ThingRequestGroup.FoodSourceNotPlantOrTree);
        for (int num = list.Count - 1; num >= 0; num--)
        {
            Corpse corpse = (Corpse)list[num];
            if ((int)corpse.GetRotStage() >= 1)
            {
                corpse.DeSpawn();
                if (!corpse.Destroyed)
                    corpse.Destroy();

                if (!corpse.Discarded)
                    corpse.Discard();
            }
        }

        extraOutcomeDesc = def.letterInfoText;
    }
}
