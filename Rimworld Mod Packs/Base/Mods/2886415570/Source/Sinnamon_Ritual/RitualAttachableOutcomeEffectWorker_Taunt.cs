using System.Collections.Generic;
using RimWorld;
using Verse;

namespace RitualRewards.Sinnamon_Ritual;

public class RitualAttachableOutcomeEffectWorker_Taunt : RitualAttachableOutcomeEffectWorker
{
    public override void Apply(Dictionary<Pawn, int> totalPresence, LordJob_Ritual jobRitual, OutcomeChance outcome, out string extraOutcomeDesc, ref LookTargets letterLookTargets)
    {
        IncidentParms parms = new()
        {
            target = jobRitual.Map,
            points = jobRitual.Map.PlayerWealthForStoryteller / 500f
        };
        _ = IncidentDefOf.RaidEnemy.Worker.TryExecute(parms);
        extraOutcomeDesc = def.letterInfoText;
    }
}
