using System.Collections.Generic;
using System.Linq;
using RimWorld;
using UnityEngine;
using Verse;
using VanillaPsycastsExpanded;
using RimWorld.Planet;

namespace Psyker
{
    public class PsycastsAbility : AbilityExtension_Psycast
    {
        public HediffDef mustHaveHediff;

        public override bool ValidateTarget(LocalTargetInfo target, VFECore.Abilities.Ability ability, bool throwMessages = false)
        {
            if (psychic)
            {
                Pawn pawn = target.Pawn;
                if (pawn != null && pawn.GetStatValue(StatDefOf.PsychicSensitivity) < float.Epsilon)
                {
                    if (throwMessages)
                    {
                        Messages.Message("Ineffective".Translate(), MessageTypeDefOf.RejectInput, historical: false);
                    }
                    return false;
                }
                HediffDef nurglesRot = mustHaveHediff;
                Log.Message("" + nurglesRot);
                if (pawn != null && !pawn.health.hediffSet.HasHediff(nurglesRot))
                {
                    if (throwMessages)
                    {
                        Messages.Message("Doesn't have nurgles rot".Translate(), MessageTypeDefOf.RejectInput, historical: false);
                    }
                    return false;
                }
            }
            return true;
        }
    }
}