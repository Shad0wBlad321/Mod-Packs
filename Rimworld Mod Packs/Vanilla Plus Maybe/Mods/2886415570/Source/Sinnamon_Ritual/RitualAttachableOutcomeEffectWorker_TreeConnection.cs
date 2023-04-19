using System.Collections.Generic;
using RimWorld;
using Verse;

namespace RitualRewards.Sinnamon_Ritual;

public class RitualAttachableOutcomeEffectWorker_TreeConnection : RitualAttachableOutcomeEffectWorker
{
    public override void Apply(Dictionary<Pawn, int> totalPresence, LordJob_Ritual jobRitual, OutcomeChance outcome, out string extraOutcomeDesc, ref LookTargets letterLookTargets)
    {
        foreach (Pawn key in totalPresence.Keys)
        {
            foreach (Thing connectedThing in key.connections.ConnectedThings)
            {
                if (connectedThing.def.defName == "Plant_TreeGauranlen")
                    connectedThing.TryGetComp<CompTreeConnection>().ConnectionStrength = 1f;
            }
        }

        extraOutcomeDesc = def.letterInfoText;
    }
}
