using HarmonyLib;
using RimWorld;
using UnityEngine;
using Verse; 

namespace SanguophageFeedRelationship
{
    [StaticConstructorOnStartup]
    static class Patches_DoBite
    { 
        static Patches_DoBite()
        {
            Harmony harmony = new Harmony("SanguophageFeedRelationship.Patches_DoBite");
            harmony.PatchAll();
        }
    }

    [HarmonyPatch(typeof(SanguophageUtility))]
    [HarmonyPatch("DoBite")]
    public class Patch_DoBite
    {
        public static bool Prefix(Pawn biter, Pawn victim, float targetHemogenGain, ref float nutritionGain, ref float targetBloodLoss, float victimResistanceGain,
            IntRange bloodFilthToSpawnRange, ref ThoughtDef thoughtDefToGiveTarget, ref ThoughtDef opinionThoughtToGiveTarget)
        {
            if (victim.story.traits.HasTrait(SFR_DefOf.SFR_Sanguophobe)) // Through trait
            {
                victim.needs?.mood?.thoughts?.memories?.TryGainMemory((Thought_Memory)ThoughtMaker.MakeThought(DefDatabase<ThoughtDef>.GetNamed("FedOn_Sanguophobe")), biter);
                victim.needs?.mood?.thoughts?.memories?.TryGainMemory((Thought_Memory)ThoughtMaker.MakeThought(DefDatabase<ThoughtDef>.GetNamed("FedOn_SocialSanguophobe")), biter);
                thoughtDefToGiveTarget = null;
                opinionThoughtToGiveTarget = null;
            }
            else if (victim.story.traits.HasTrait(SFR_DefOf.SFR_Sanguophile)) // Through trait
            {
                victim.needs?.mood?.thoughts?.memories?.TryGainMemory((Thought_Memory)ThoughtMaker.MakeThought(DefDatabase<ThoughtDef>.GetNamed("FedOn_Sanguophile")), biter);
                victim.needs?.mood?.thoughts?.memories?.TryGainMemory((Thought_Memory)ThoughtMaker.MakeThought(DefDatabase<ThoughtDef>.GetNamed("FedOn_SocialSanguophile")), biter);
                thoughtDefToGiveTarget = null;
                opinionThoughtToGiveTarget = null;
            }
            else if (victim.relations.DirectRelationExists(PawnRelationDefOf.Lover, biter) || victim.relations.DirectRelationExists(PawnRelationDefOf.Fiance, biter) ||
             victim.relations.DirectRelationExists(PawnRelationDefOf.Spouse, biter)) // Through relationship
            {
                victim.needs?.mood?.thoughts?.memories?.TryGainMemory((Thought_Memory)ThoughtMaker.MakeThought(DefDatabase<ThoughtDef>.GetNamed("FedOn_Lover")), biter);
                victim.needs?.mood?.thoughts?.memories?.TryGainMemory((Thought_Memory)ThoughtMaker.MakeThought(DefDatabase<ThoughtDef>.GetNamed("FedOn_SocialLover")), biter);
                thoughtDefToGiveTarget = null;
                opinionThoughtToGiveTarget = null;
            }
            else if (victim.story.traits.HasTrait(SFR_DefOf.SFR_Sanguofriend)) // Through trait
            {
                victim.needs?.mood?.thoughts?.memories?.TryGainMemory((Thought_Memory)ThoughtMaker.MakeThought(DefDatabase<ThoughtDef>.GetNamed("FedOn_Sanguofriend")), biter);
                victim.needs?.mood?.thoughts?.memories?.TryGainMemory((Thought_Memory)ThoughtMaker.MakeThought(DefDatabase<ThoughtDef>.GetNamed("FedOn_SocialSanguofriend")), biter);
                thoughtDefToGiveTarget = null;
                opinionThoughtToGiveTarget = null;
            }
            else if (victim.relations.OpinionOf(biter) >= SFR_ModSettings.CloseFriend_minimumOpinion) // Through relationship
            {
                victim.needs?.mood?.thoughts?.memories?.TryGainMemory((Thought_Memory)ThoughtMaker.MakeThought(DefDatabase<ThoughtDef>.GetNamed("FedOn_CloseFriend")), biter);
                victim.needs?.mood?.thoughts?.memories?.TryGainMemory((Thought_Memory)ThoughtMaker.MakeThought(DefDatabase<ThoughtDef>.GetNamed("FedOn_SocialCloseFriend")), biter);
                thoughtDefToGiveTarget = null;
                opinionThoughtToGiveTarget = null;
            }

            if (biter.story.traits.HasTrait(DefDatabase<TraitDef>.GetNamed("Bloodlust")))
            {
                targetBloodLoss += targetBloodLoss * 0.1f;
                nutritionGain += nutritionGain * 0.1f;
            }

            nutritionGain = SFR_ModSettings.HemogenNutrition;

            return true;
        }

    }

    // <== Auto-feed ==>

    /*
    [HarmonyPatch(typeof(SocialCardUtility))]
    [HarmonyPatch("DrawSocialCard")]
    public class Patch_DrawSocialCard
    {
        public static void Postfix (Rect rect, Pawn pawn)
        {
            if (ModsConfig.BiotechActive && !pawn.Dead && !pawn.IsBloodfeeder() && (pawn.IsColonist || pawn.IsPrisoner || pawn.IsSlaveOfColony))
            {
                Widgets.BeginGroup(rect);

                Text.Font = GameFont.Small;
                 
                //GUI.color = new Color(1f, 1f, 1f, 0.5f);
                //GUI.color = Color.white;

                float num = Prefs.DevMode ? 20f : 15f;
                //Widgets.DrawLineHorizontal(0f, rect.height - num, rect.width);
                Rect SFR_rect = new Rect(0f, rect.height/3*2 - num, rect.width, rect.height - num).ContractedBy(10f);
                SFR_rect.height *= 0.63f;

                bool boo = SFR_GameComponent.GetAutoFeed(pawn);
                Widgets.CheckboxLabeled(SFR_rect, "Auto feed sanguophiles", ref boo, false, null, null, false);
                SFR_GameComponent.SetAutoFeed(pawn, boo);
                 
                Widgets.EndGroup();
            }
        }
    }  

    [HarmonyPatch(typeof(Gene_Hemogen))]
    [HarmonyPatch(nameof(Gene_Hemogen.ShouldConsumeHemogenNow))]
    public class Patch_TryBiteChillPawn
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        { 
            MethodInfo m_GetChillPawn = SymbolExtensions.GetMethodInfo(() => GetChillPawn(null));

            yield return new CodeInstruction(OpCodes.Ldarg_0);
            yield return new CodeInstruction(OpCodes.Call, m_GetChillPawn);
            //yield return new CodeInstruction(OpCodes.Ret);


            foreach (CodeInstruction instruction in instructions)
            {
                yield return instruction;
            } 
        }
        /*
        public static void GetChillPawn (Gene_Hemogen gene)
        {
            Pawn pawn = gene.pawn;
            Pawn_JobTracker jobReport = pawn.jobs;

            if (!pawn.Awake())
                return;
            foreach (Job job in jobReport.AllJobs())
            {
                if (job.def == SFR_DefOf.ChillPawnBloodfeed)
                {
                    return;
                }
            } 

            float newTarget = gene.targetValue - (gene.targetValue / 4);

            if (gene.Value < newTarget)
            {
                Pawn victim = FindChillPawn(pawn);

                if (victim != null)
                {
                    pawn.jobs.TryTakeOrderedJob(JobMaker.MakeJob(SFR_DefOf.ChillPawnBloodfeed, victim));
                }
            }

            return;
        }*/




    // <!== Auto-feed ==!>



    // when becoming a sanguophage lose the sanguophile and sanguofriend traits//
    [HarmonyPatch(typeof(PawnGenerator))]
    [HarmonyPatch("GenerateGenes")]
    public class Patch_GenerateGenes
    {
        public static bool Prefix(Pawn pawn, XenotypeDef xenotype, PawnGenerationRequest request)
        {
            if (xenotype == XenotypeDefOf.Sanguophage)
            {
                if (pawn.story.traits.HasTrait(DefDatabase<TraitDef>.GetNamed("SFR_Sanguophile")))
                {
                    pawn.story.traits.RemoveTrait(pawn.story.traits.GetTrait(SFR_DefOf.SFR_Sanguophile));
                }

                if (pawn.story.traits.HasTrait(SFR_DefOf.SFR_Sanguofriend))
                {
                    pawn.story.traits.RemoveTrait(pawn.story.traits.GetTrait(SFR_DefOf.SFR_Sanguofriend));
                }
            }
            return true;
        }
    }

}
