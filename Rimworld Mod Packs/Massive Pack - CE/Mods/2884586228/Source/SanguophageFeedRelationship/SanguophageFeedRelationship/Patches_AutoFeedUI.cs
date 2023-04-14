using HarmonyLib;
using RimWorld;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using UnityEngine;
using Verse;

namespace SanguophageFeedRelationship
{
    [HarmonyPatch(typeof(SocialCardUtility))]
    [HarmonyPatch("DrawSocialCard")]
    public class Patch_DrawSocialCard
    {   
        public Patch_DrawSocialCard() {
        }


        /*public static void Postfix(Rect rect, Pawn pawn)
        {
            if (ModsConfig.BiotechActive && !pawn.Dead && !pawn.IsBloodfeeder() && (pawn.IsColonist || pawn.IsPrisoner || pawn.IsSlaveOfColony))
            {
                Widgets.BeginGroup(rect);

                Text.Font = GameFont.Small;

                //GUI.color = new Color(1f, 1f, 1f, 0.5f);
                //GUI.color = Color.white;

                //Widgets.DrawLineHorizontal(0f, rect.height - num, rect.width);
                Rect SFR_rect = new Rect(30f, 40f, rect.width, 40f).ContractedBy(10f);

                bool boo = SFR_GameComponent.GetAutoFeed(pawn);
                Widgets.CheckboxLabeled(SFR_rect, "Auto feed sanguophiles", ref boo, false, null, null, true);
                SFR_GameComponent.SetAutoFeed(pawn, boo);

                Widgets.EndGroup();
            }
        }*/

        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            Rect temp = new Rect(0f, 0f, 0f, 0f);
            MethodInfo m_AddAutoFeedToggle = SymbolExtensions.GetMethodInfo(() => AddAutoFeedToggle(temp, temp, null));

            foreach (CodeInstruction instruction in instructions)
            {
                yield return instruction;
                if (instruction.opcode.Equals(OpCodes.Mul))
                { 
                    yield return new CodeInstruction(OpCodes.Ldloc_S, 4);
                    yield return new CodeInstruction(OpCodes.Ldarg_0);
                    yield return new CodeInstruction(OpCodes.Ldarg_1); 
                    yield return new CodeInstruction(OpCodes.Call, m_AddAutoFeedToggle); 
                }
            }
        }

        public static void AddAutoFeedToggle (Rect rect5, Rect rect, Pawn pawn)
        {
            if (SFR_ModSettings.autoFeedOptionsOff)
                return;

            if (ModsConfig.BiotechActive && !pawn.Dead && !pawn.IsBloodfeeder() && (pawn.IsColonist || pawn.IsPrisoner || pawn.IsSlaveOfColony))
            {
                Widgets.BeginGroup(rect);

                Text.Font = GameFont.Small;
                Rect SFR_rect = new Rect(rect.width/2, rect5.yMin - 30f, rect.width, 22f);

                SFR_AutoFeeders gc = Current.Game.GetComponent<SFR_AutoFeeders>();
                bool boo = gc.GetAutoFeed(pawn.thingIDNumber); 
                Widgets.CheckboxLabeled(SFR_rect, "Auto feed bloodfeeders?", ref boo, false, null, null, true);
                gc.SetAutoFeed(pawn, boo);
                  
                Widgets.EndGroup();
            }
        }

    }
     


        /*
        [HarmonyPatch(typeof(ITab_Pawn_Social))]
        [HarmonyPatch(MethodType.Constructor)]
        public class Patch_FillTab
        {
            public static void Postfix(ref Vector2 ___size)
            {
                Log.Message("Sizeeeeee: " + ___size);
                ___size += new Vector2(0f, 380f);
                Log.Message("Size new: " + ___size);
            }

        }*/

}