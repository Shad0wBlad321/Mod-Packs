﻿using RimWorld;
using Verse;

namespace Brainwash
{
    public class JobDriver_StartBrainwashTelevision : JobDriver_WatchBrainwashBase
    {
        public override void BrainwashEffect()
        {
            CompChangePersonality comp = pawn.GetComp<CompChangePersonality>();

            System.Collections.Generic.List<string> backstoriesToSet = comp.backstoriesToSet;
            for (int i = 0; i < 2; i++)
            {
                string backstoryID = backstoriesToSet.Count > i ? backstoriesToSet[i] : null;
                if (i == 0)
                {
                    pawn.story.childhood = DefDatabase<BackstoryDef>.GetNamed(backstoryID);
                }
                else
                {
                    pawn.story.adulthood = DefDatabase<BackstoryDef>.GetNamed(backstoryID);
                }
            }

            System.Collections.Generic.List<TraitEntry> traitsToSet = comp.traitsToSet;
            for (int i = pawn.story.traits.allTraits.Count - 1; i >= 0; i--)
            {
                Trait trait = pawn.story.traits.allTraits[i];
                pawn.story.traits.RemoveTrait(trait);
            }

            for (int i = 0; i < 4; i++)
            {
                TraitEntry traitToAdd = traitsToSet.Count > i ? traitsToSet[i] : null;
                if (traitToAdd != null)
                {
                    pawn.story.traits.GainTrait(new Trait(traitToAdd.traitDef, traitToAdd.degree));
                }
            }

            System.Collections.Generic.List<SkillEntry> skillsToSet = comp.skillsToSet;
            foreach (SkillEntry skill in skillsToSet)
            {
                SkillRecord skillRecord = pawn.skills.GetSkill(skill.skillDef);
                skillRecord.Level = skill.level;
                skillRecord.passion = skill.passion;
                skillRecord.xpSinceLastLevel = skillRecord.XpRequiredForLevelUp / 2f;
            }
            pawn.health.AddHediff(HediffDefOf.CatatonicBreakdown);
            Messages.Message("Brainwash_PawnHavingBreakdown".Translate(pawn.Named("PAWN")), pawn, MessageTypeDefOf.CautionInput);
        }
    }
}
