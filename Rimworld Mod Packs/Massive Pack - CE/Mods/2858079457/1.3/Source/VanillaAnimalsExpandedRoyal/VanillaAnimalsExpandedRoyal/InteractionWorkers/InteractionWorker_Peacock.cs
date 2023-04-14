using System;
using System.Collections.Generic;
using Verse;
using RimWorld;

namespace VanillaAnimalsExpandedRoyal
{
    public class InteractionWorker_Peacock : InteractionWorker
    {
        public override void Interacted(Pawn initiator, Pawn recipient, List<RulePackDef> extraSentencePacks, out string letterText, out string letterLabel, out LetterDef letterDef, out LookTargets lookTargets)
        {
            this.AddNuzzledThought(initiator, recipient);

            letterText = null;
            letterLabel = null;
            letterDef = null;
            lookTargets = null;
        }

        private void AddNuzzledThought(Pawn initiator, Pawn recipient)
        {
            if (recipient.health.hediffSet.HasHediff(InternalDefOf.VAERoy_PsychicSensitivity))
            {
                recipient.health.hediffSet.GetFirstHediffOfDef(InternalDefOf.VAERoy_PsychicSensitivity).Severity += 0.125f;
            }
            else { 
                recipient.health.AddHediff(InternalDefOf.VAERoy_PsychicSensitivity);
                recipient.health.hediffSet.GetFirstHediffOfDef(InternalDefOf.VAERoy_PsychicSensitivity).Severity = 0.125f;
            }

            

        }


    }
}
