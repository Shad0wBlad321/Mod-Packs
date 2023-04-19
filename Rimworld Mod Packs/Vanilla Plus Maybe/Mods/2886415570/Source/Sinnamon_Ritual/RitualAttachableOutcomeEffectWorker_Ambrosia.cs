using System.Collections.Generic;
using RimWorld;
using Verse;

namespace RitualRewards.Sinnamon_Ritual;

public class RitualAttachableOutcomeEffectWorker_Ambrosia : RitualAttachableOutcomeEffectWorker
{
    public override void Apply(Dictionary<Pawn, int> totalPresence, LordJob_Ritual jobRitual, OutcomeChance outcome, out string extraOutcomeDesc, ref LookTargets letterLookTargets)
    {
        IncidentParms parms = new()
        {
            target = jobRitual.Map,
            points = outcome.BestPositiveOutcome(jobRitual) ? 1510 : 1005
        };
        IncidentDef incidentDef = IncidentDef.Named("Sinnamon_SmallAmbrosiaSprout");
        if (!incidentDef.Worker.CanFireNow(parms))
        {
            extraOutcomeDesc = "Sinnamon_AmbrosiaFailed";
            return;
        }

        _ = incidentDef.Worker.TryExecute(parms);
        extraOutcomeDesc = def.letterInfoText;
    }
}
