using System.Collections.Generic;
using RimWorld;
using Verse;

namespace RitualRewards.Sinnamon_Ritual;

public class RitualAttachableOutcomeEffectWorker_Meteor : RitualAttachableOutcomeEffectWorker
{
    public override void Apply(Dictionary<Pawn, int> totalPresence, LordJob_Ritual jobRitual, OutcomeChance outcome, out string extraOutcomeDesc, ref LookTargets letterLookTargets)
    {
        IncidentParms parms = new()
        {
            target = jobRitual.Map
        };
        _ = IncidentDef.Named("MeteoriteImpact").Worker.TryExecute(parms);
        extraOutcomeDesc = def.letterInfoText;
    }
}
