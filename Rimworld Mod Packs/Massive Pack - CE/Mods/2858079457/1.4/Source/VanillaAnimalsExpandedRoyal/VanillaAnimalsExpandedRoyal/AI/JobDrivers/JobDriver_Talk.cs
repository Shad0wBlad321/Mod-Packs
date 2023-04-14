
using System.Collections.Generic;
using RimWorld;
using Verse;
using Verse.AI;
using System.Linq;
using UnityEngine;

namespace VanillaAnimalsExpandedRoyal
{
    public class JobDriver_Talk : JobDriver
    {
        private const int TalkDuration = 2000;

        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
            return true;
        }

        protected override IEnumerable<Toil> MakeNewToils()
        {
            this.FailOnDespawnedNullOrForbidden(TargetIndex.A);
            this.FailOnNotCasualInterruptible(TargetIndex.A);
            yield return Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.Touch);
            yield return Toils_Interpersonal.WaitToBeAbleToInteract(pawn);
            Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.Touch).socialMode = RandomSocialMode.Off;
            Toils_General.WaitWith(TargetIndex.A, TalkDuration, useProgressBar: false, maintainPosture: true).socialMode = RandomSocialMode.Off;
            yield return Toils_General.Do(delegate
            {
                Pawn recipient = (Pawn)pawn.CurJob.targetA.Thing;

                
                pawn.interactions.TryInteractWith(recipient, InternalDefOf.VAERoy_OrangutanInteraction);
                if(!pawn.relations.DirectRelationExists(InternalDefOf.VAERoy_TutorRelation, recipient))
                {
                    pawn.relations.AddDirectRelation(InternalDefOf.VAERoy_TutorRelation, recipient);
                }

                if (!recipient.relations.DirectRelationExists(InternalDefOf.VAERoy_TutorRelation, pawn))
                {
                    recipient.relations.AddDirectRelation(InternalDefOf.VAERoy_TutorRelation, pawn);
                }

                
                SkillDef skillDef = DefDatabase<SkillDef>.AllDefs.Where((SkillDef x) => !recipient.skills.GetSkill(x).TotallyDisabled).RandomElement();
                int level = recipient.skills.GetSkill(skillDef).Level;
                IntRange skillGain = new IntRange(160, 240);
                int realSkillRange = skillGain.RandomInRange;
                recipient.skills.Learn(skillDef, realSkillRange, direct: true);
                MoteMaker.ThrowText(recipient.DrawPos, recipient.Map, skillDef.LabelCap + ": "+ realSkillRange + " xp", Color.white, 3.65f);
                if (pawn.Name == null || pawn.Name.Numerical)
                {
                    pawn.Name = PawnBioAndNameGenerator.GeneratePawnName(pawn);
                }
                if (!recipient.Dead && recipient.needs.joy != null)
                {
                    JoyKindDef joyKind = JoyKindDefOf.Social;
                    recipient.needs.joy.GainJoy(0.05f, joyKind);
                }


            });
        }
    }
}
