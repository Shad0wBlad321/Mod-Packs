using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;

namespace RitualRewards.Sinnamon_Ritual;

public class RitualAttachableOutcomeEffectWorker_Strip : RitualAttachableOutcomeEffectWorker
{
    public override void Apply(Dictionary<Pawn, int> totalPresence, LordJob_Ritual jobRitual, OutcomeChance outcome, out string extraOutcomeDesc, ref LookTargets letterLookTargets)
    {
        extraOutcomeDesc = "";
        bool flag = false;
        float chance = outcome.BestPositiveOutcome(jobRitual) ? 0.33f : 0.75f;
        IEnumerable<Pawn> enumerable = jobRitual.Map.mapPawns.AllPawnsSpawned.Where((x) => x.Faction.HostileTo(Faction.OfPlayer));
        if (!enumerable.Any())
            return;

        foreach (Pawn item in enumerable)
        {
            if (Rand.Chance(chance))
            {
                item.apparel.DropAll(item.PositionHeld, true, true);
                flag = true;
            }
        }

        if (flag)
            extraOutcomeDesc = def.letterInfoText + (outcome.BestPositiveOutcome(jobRitual) ? "Sinnamon_stripGood".Translate() : "Sinnamon_stripGreat".Translate());
    }
}
