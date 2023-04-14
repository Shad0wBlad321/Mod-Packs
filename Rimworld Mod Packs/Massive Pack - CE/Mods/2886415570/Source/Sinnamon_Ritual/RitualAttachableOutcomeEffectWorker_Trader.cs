using System.Collections.Generic;
using RimWorld;
using Verse;

namespace RitualRewards.Sinnamon_Ritual;

public class RitualAttachableOutcomeEffectWorker_Trader : RitualAttachableOutcomeEffectWorker
{
    public override void Apply(Dictionary<Pawn, int> totalPresence, LordJob_Ritual jobRitual, OutcomeChance outcome, out string extraOutcomeDesc, ref LookTargets letterLookTargets)
    {
        IncidentParms parms = new()
        {
            target = jobRitual.Map,
            sendLetter = false
        };
        _ = IncidentDefOf.OrbitalTraderArrival.Worker.TryExecute(parms);
        extraOutcomeDesc = def.letterInfoText;
    }
}
