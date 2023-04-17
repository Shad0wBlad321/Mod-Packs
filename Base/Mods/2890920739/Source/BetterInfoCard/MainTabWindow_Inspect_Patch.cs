using HarmonyLib;
using RimWorld;
using Verse;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using RimWorld.Planet;
using System.Reflection.Emit;

namespace BetterInfoCard
{
    static class  MainTabWindow_Inspect_Patch
    {
        public static Texture2D CompareCardIcon => ContentFinder<Texture2D>.Get("UI/Widgets/CompareCard");

        [HarmonyPatch(typeof(MainTabWindow_Inspect), "DoInspectPaneButtons")]
        public static class Patch_MainTabWindow_Inspect_DoInspectPaneButtons
        {
            public static bool Prefix(MainTabWindow_Inspect __instance, Rect rect, ref float lineEndWidth)
            {
                var selected = Find.Selector.NumSelected;
                if (selected == 1)
                {
                    if (Dialog_InfoCard_Patch.inspectCard != null)
                    {
                        Thing selectThing = Find.Selector.SelectedObjects[0] as Thing;
                        if (selectThing != null && Dialog_InfoCard_Patch.inspectCard.Get_Thing()!=selectThing)
                        {
                            Dialog_InfoCard_Patch.inspectCard.Set_Thing(selectThing);
                            Dialog_InfoCard_Patch.inspectCard.SetTab(Dialog_InfoCard.InfoCardTab.Stats);
                            Dialog_InfoCard_Patch.inspectCard.Invoke_Setup();
                        }
                    }
                    return true;
                }

                List<Thing> selectedThings = new List<Thing>();
                foreach (var obj in Find.Selector.SelectedObjects)
                {
                    if (obj is Thing thing) selectedThings.Add(thing);
                }

                float num = rect.width - 48f;
                if (selectedThings.Count>1)
                {
                    Rect buttonRect = new Rect(num, 0f, 48, 48);
                    if(Widgets.ButtonImage(buttonRect, CompareCardIcon))
                    {
                        List<Dialog_InfoCard> toCompareCards = new List<Dialog_InfoCard>();
                        foreach (var thing in selectedThings)
                        {
                            var card = new Dialog_InfoCard(thing);
                            toCompareCards.Add(card);
                        }
                        Dialog_InfoCard_Patch.SetComparingInfoCards(toCompareCards);
                        foreach (var card in toCompareCards)
                        {
                            Find.WindowStack.Add(card);
                        }
                        
                    }
                    lineEndWidth += 24f;
                }
                return true;
            }
        }

    }
}
