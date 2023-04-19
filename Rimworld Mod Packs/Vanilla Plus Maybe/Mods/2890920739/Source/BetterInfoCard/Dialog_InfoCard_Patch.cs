using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using RimWorld;
using Verse;
using System.Reflection;
using UnityEngine;
using RimWorld.Planet;
using System.Reflection.Emit;

namespace BetterInfoCard
{
    static class Dialog_InfoCard_Patch
    {
        public static Dictionary<Dialog_InfoCard, StatsReportUtility_Instanced> infoCardStatsDic = new Dictionary<Dialog_InfoCard, StatsReportUtility_Instanced>();
        public static Dictionary<Dialog_InfoCard, List<Dialog_InfoCard.Hyperlink>> infoCardHistoryDic = new Dictionary<Dialog_InfoCard, List<Dialog_InfoCard.Hyperlink>>();


        static MethodInfo DrawStatsReportMethod1 = typeof(StatsReportUtility).GetMethod("DrawStatsReport", new Type[] { typeof(Rect), typeof(Def), typeof(ThingDef) });
        static MethodInfo DrawStatsReportMethod2 = typeof(StatsReportUtility).GetMethod("DrawStatsReport", new Type[] { typeof(Rect), typeof(AbilityDef) });
        static MethodInfo DrawStatsReportMethod3 = typeof(StatsReportUtility).GetMethod("DrawStatsReport", new Type[] { typeof(Rect), typeof(Thing) });
        static MethodInfo DrawStatsReportMethod4 = typeof(StatsReportUtility).GetMethod("DrawStatsReport", new Type[] { typeof(Rect), typeof(Hediff) });
        static MethodInfo DrawStatsReportMethod5 = typeof(StatsReportUtility).GetMethod("DrawStatsReport", new Type[] { typeof(Rect), typeof(WorldObject) });
        static MethodInfo DrawStatsReportMethod6 = typeof(StatsReportUtility).GetMethod("DrawStatsReport", new Type[] { typeof(Rect), typeof(RoyalTitleDef), typeof(Faction), typeof(Pawn) });
        static MethodInfo DrawStatsReportMethod7 = typeof(StatsReportUtility).GetMethod("DrawStatsReport", new Type[] { typeof(Rect), typeof(Faction) });

        static MethodInfo Method_SelectEntry = typeof(StatsReportUtility).GetMethod("SelectEntry", new Type[] { typeof(int) });

        static MethodInfo Method_Notify_QuickSearchChanged = typeof(StatsReportUtility).GetMethod("Notify_QuickSearchChanged");

        static MethodInfo Method_get_QuickSearchWidget = typeof(StatsReportUtility).GetMethod("get_QuickSearchWidget");

        static FieldInfo Field_tab = typeof(Dialog_InfoCard).GetField("tab", BindingFlags.NonPublic | BindingFlags.Instance);

        static FieldInfo Field_thing = typeof(Dialog_InfoCard).GetField("thing", BindingFlags.NonPublic | BindingFlags.Instance);

        static MethodInfo Method_Setup = typeof(Dialog_InfoCard).GetMethod("Setup", BindingFlags.NonPublic | BindingFlags.Instance);

        static MethodInfo Method_get_ThingPawn = typeof(Dialog_InfoCard).GetMethod("get_ThingPawn", BindingFlags.NonPublic | BindingFlags.Instance);

        static FieldInfo Field_executeAfterFillCardOnce = typeof(Dialog_InfoCard).GetField("executeAfterFillCardOnce", BindingFlags.NonPublic | BindingFlags.Instance);
        public static void DrawStatsReportInstanced1(Rect rect, Def def, ThingDef stuff, Dialog_InfoCard dialog)
            => DialogToStatsReport(dialog)?.DrawStatsReport(rect, def, stuff);
        public static void DrawStatsReportInstanced2(Rect rect, AbilityDef def, Dialog_InfoCard dialog)
            => DialogToStatsReport(dialog)?.DrawStatsReport(rect, def);
        public static void DrawStatsReportInstanced3(Rect rect, Thing thing, Dialog_InfoCard dialog)
            => DialogToStatsReport(dialog)?.DrawStatsReport(rect, thing);
        public static void DrawStatsReportInstanced4(Rect rect, Hediff hediff, Dialog_InfoCard dialog)
            => DialogToStatsReport(dialog)?.DrawStatsReport(rect, hediff);
        public static void DrawStatsReportInstanced5(Rect rect, WorldObject worldObject, Dialog_InfoCard dialog)
            => DialogToStatsReport(dialog)?.DrawStatsReport(rect, worldObject);
        public static void DrawStatsReportInstanced6(Rect rect, RoyalTitleDef title, Faction faction, Pawn pawn, Dialog_InfoCard dialog)
            => DialogToStatsReport(dialog)?.DrawStatsReport(rect, title, faction, pawn);
        public static void DrawStatsReportInstanced7(Rect rect, Faction faction, Dialog_InfoCard dialog)
            => DialogToStatsReport(dialog)?.DrawStatsReport(rect, faction);

        public static void SelectEntryInstanced(int index, Dialog_InfoCard dialog)
            => DialogToStatsReport(dialog)?.SelectEntry(index);

        public static void Notify_QuickSearchChangedInstanced(Dialog_InfoCard dialog)
            => DialogToStatsReport(dialog)?.Notify_QuickSearchChanged();

        public static Dialog_InfoCard allCompareTarget;

        public static Dialog_InfoCard inspectCard;

        public static QuickSearchWidget get_QuickSearchWidgetInstanced(Dialog_InfoCard dialog)
        {
            return (DialogToStatsReport(dialog)?.minify??false)  ? null : infoCardStatsDic[dialog].QuickSearchWidget;
        }

        private static StatsReportUtility_Instanced DialogToStatsReport(Dialog_InfoCard dialog)
        {
            if (!infoCardStatsDic.ContainsKey(dialog)) return null;
            return infoCardStatsDic[dialog];
        }





        private static Dictionary<MethodInfo, CodeInstruction> DrawStatsReportTranspileCallDic = new Dictionary<MethodInfo, CodeInstruction>()
        {
            {DrawStatsReportMethod1,CodeInstruction.Call(typeof(Dialog_InfoCard_Patch),nameof(DrawStatsReportInstanced1)) },
            {DrawStatsReportMethod2,CodeInstruction.Call(typeof(Dialog_InfoCard_Patch),nameof(DrawStatsReportInstanced2)) },
            {DrawStatsReportMethod3,CodeInstruction.Call(typeof(Dialog_InfoCard_Patch),nameof(DrawStatsReportInstanced3)) },
            {DrawStatsReportMethod4,CodeInstruction.Call(typeof(Dialog_InfoCard_Patch),nameof(DrawStatsReportInstanced4)) },
            {DrawStatsReportMethod5,CodeInstruction.Call(typeof(Dialog_InfoCard_Patch),nameof(DrawStatsReportInstanced5)) },
            {DrawStatsReportMethod6,CodeInstruction.Call(typeof(Dialog_InfoCard_Patch),nameof(DrawStatsReportInstanced6)) },
            {DrawStatsReportMethod7,CodeInstruction.Call(typeof(Dialog_InfoCard_Patch),nameof(DrawStatsReportInstanced7)) },

            {Method_SelectEntry,CodeInstruction.Call(typeof(Dialog_InfoCard_Patch),nameof(SelectEntryInstanced)) },

            {Method_Notify_QuickSearchChanged,CodeInstruction.Call(typeof(Dialog_InfoCard_Patch),nameof(Notify_QuickSearchChangedInstanced)) },

            { Method_get_QuickSearchWidget,CodeInstruction.Call(typeof(Dialog_InfoCard_Patch),nameof(get_QuickSearchWidgetInstanced)) },
        };

        public static IEnumerable<CodeInstruction> TranspileToInstanceMethod(IEnumerable<CodeInstruction> instructions)
        {
            foreach (var instruction in instructions)
            {
                bool replaced = false;
                foreach (var dicMethod in DrawStatsReportTranspileCallDic.Keys)
                {
                    if (instruction.Calls(dicMethod))
                    {

                        var loadThis = new CodeInstruction(OpCodes.Ldarg_0);
                        instruction.MoveLabelsTo(loadThis);
                        yield return loadThis;

                        yield return DrawStatsReportTranspileCallDic[dicMethod].Clone();
                        replaced = true;
                    }
                }

                if (!replaced)
                {
                    yield return instruction;
                }
            }
        }

        [HarmonyPatch(typeof(Dialog_InfoCard), "Setup")]
        public static class Patch_Dialog_InfoCard_Setup
        {
            public static void Postfix(Dialog_InfoCard __instance)
            {
                __instance.forcePause = false;
                __instance.preventCameraMotion = false;
                __instance.resizeable = true;
                __instance.draggable = true;
                __instance.onlyOneOfTypeAllowed = false;
                __instance.closeOnClickedOutside = false;
                __instance.absorbInputAroundWindow = false;

                StatsReportUtility_Instanced uti = null;
                if (infoCardStatsDic.ContainsKey(__instance))
                {
                    uti = infoCardStatsDic[__instance];
                    uti.Reset();
                }
                else
                {
                    uti = new StatsReportUtility_Instanced();
                    infoCardStatsDic.Add(__instance, uti);
                    uti.Reset();
                }



                if(BetterInfoCard.singleton.settings.openInfoCardFocusSearch)
                {
                    var executeAfterFillCardOnce = Field_executeAfterFillCardOnce.GetValue(__instance) as System.Action;
                    System.Action focusAcion = () => {  uti.QuickSearchWidget.Focus(); };
                    Field_executeAfterFillCardOnce.SetValue(__instance, focusAcion);
                }
            }
        }

        [HarmonyPatch(typeof(Dialog_InfoCard), "Close")]
        public static class Patch_Dialog_InfoCard_Close
        {
            public static void Postfix(Dialog_InfoCard __instance)
            {
                //Log.Message($"当前信息卡数量{infoCardStatsDic.Keys.Count}");
                if (infoCardStatsDic.ContainsKey(__instance))
                {
                    infoCardStatsDic.Remove(__instance);
                }

                if (allCompareTarget == __instance) allCompareTarget = null;
                if (inspectCard == __instance) inspectCard = null;
            }
        }


        [HarmonyPatch(typeof(Window), "SetInitialSizeAndPosition")]
        public static class Patch_Window_SetInitialSizeAndPosition
        {
            public static void Postfix(Window __instance)
            {
                if (__instance is Dialog_InfoCard card)
                {
                    card.windowRect.width = BetterInfoCard.singleton.settings.initCardWidth;
                    card.windowRect.height = BetterInfoCard.singleton.settings.initCardHeight;


                    if (comparingInfoCards.Contains(card))
                    {
                        SetCollapse(true, card);
                        card.windowRect.position = comaprePivot;
                        comparingInfoCards.Remove(card);
                        comaprePivot.x += card.windowRect.width;
                    }
                    else
                    {
                        if (BetterInfoCard.singleton.settings.openInfoCardCollapsed)
                        {
                            SetCollapse(true, card);
                        }
                    }




                    
                }

            }
        }


        [HarmonyPatch(typeof(Widgets), "InfoCardButtonWorker", new Type[]{typeof(Rect)})]
        public static class Patch_Widgets_InfoCardButtonWorker
        {
            public static void Postfix(ref bool __result)
            {
                //IMGUI下，不点击框体的话，Event.current就不会捕捉点击事件。所以直接使用UnityEngine.Input获取。
                if (Event.current.type == EventType.Repaint && UnityEngine.Input.GetKeyDown(BetterInfoCard.singleton.settings.infoCardHotKey))
                {
                    __result = true;
                }

            }
        }
        
        [HarmonyPatch(typeof(Dialog_InfoCard), "DoWindowContents")]
        public static class Patch_Dialog_InfoCard_DoWindowContents
        {
            public static bool Prefix(Dialog_InfoCard __instance, Rect inRect)
            {
                DrawAppendButtons(__instance, inRect);
                if (!infoCardStatsDic.ContainsKey(__instance)) return true;
                var stats = infoCardStatsDic[__instance];
                if (!stats.minify)
                {
                    return true;
                }
                else
                {
                    MethodInfo method_GetTitle = typeof(Dialog_InfoCard).GetMethod("GetTitle", BindingFlags.Instance | BindingFlags.NonPublic);
                    


                    Rect rect1 = new Rect(inRect).ContractedBy(18f);
                    rect1.height = 34f;
                    rect1.x += 34f;
                    Text.Font = GameFont.Medium;
                    Widgets.Label(rect1, method_GetTitle.Invoke(__instance,null).ToString() );
                    //Rect rect2 = new Rect(inRect.x + 9f, rect1.y, 34f, 34f);
                    //if (this.thing != null)
                    //    Widgets.ThingIcon(rect2, this.thing);
                    //else
                    //    Widgets.DefIcon(rect2, this.def, this.stuff, drawPlaceholder: true);
                    Rect rect3 = new Rect(inRect);
                    rect3.yMin = rect1.yMax;
                    rect3.yMax -= 38f;
                    Rect rect4 = rect3;


                    FieldInfo resizerField = typeof(Window).GetField("resizer", BindingFlags.Instance | BindingFlags.NonPublic);
                    WindowResizer resizer = resizerField.GetValue(__instance) as WindowResizer;
                    resizer.minWindowSize.y = 40;

                    return false;
                }
            }
        }


        [HarmonyPatch(typeof(Dialog_InfoCard), "FillCard")]
        public static class Patch_Dialog_InfoCard_FillCard
        {
            static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
            {
                return TranspileToInstanceMethod(instructions);
            }

            public static void Postfix(Dialog_InfoCard __instance, Rect cardRect)
            { 
            }
        }

        private static void SetCollapse(bool collapse, Dialog_InfoCard card)
        {
            var stats = infoCardStatsDic[card];
            if (collapse && !stats.collapse) card.windowRect.width = 0.5f * card.windowRect.width;
            if(!collapse && stats.collapse) card.windowRect.width = 2 * card.windowRect.width;
            stats.collapse = collapse;
            Widgets.mouseOverScrollViewStack.Clear(); //窗口尺寸变化的时候似乎需要加上这一句？？？
        }

        private static void SetMinify(bool minify, Dialog_InfoCard card)
        {
            var stats = infoCardStatsDic[card];
            if (minify && !stats.minify) card.windowRect.height = 60;
            if (!minify && stats.minify) card.windowRect.height = card.InitialSize.y;
            stats.minify = minify;
            card.doCloseButton = !minify;
            Widgets.mouseOverScrollViewStack.Clear(); //窗口尺寸变化的时候似乎需要加上这一句？？？
        }

        public static Texture2D CollapseIcon => ContentFinder<Texture2D>.Get("UI/Widgets/Collapse");
        public static Texture2D MinifyIcon => ContentFinder<Texture2D>.Get("UI/Widgets/Minify");
        public static Texture2D CompareAllIcon => ContentFinder<Texture2D>.Get("UI/Widgets/ApplyAll");

        public static Texture2D InspectAllIcon => ContentFinder<Texture2D>.Get("UI/Widgets/Inspect");
        private static void DrawAppendButtons(Dialog_InfoCard __instance, Rect cardRect)
        {
            if (infoCardStatsDic.ContainsKey(__instance))
            {
                var stats = infoCardStatsDic[__instance];
                Rect minifyButtonRect = new Rect(cardRect.xMax - 50, cardRect.yMin + 4f, 18f, 18f);
                Rect collapseButtonRect = new Rect(cardRect.xMax - 80, cardRect.yMin + 4f, 18f, 18f);
                Rect applyAllButtonRect = new Rect(cardRect.xMax - 110, cardRect.yMin + 4f, 18f, 18f);
                Rect InspectButtonRect = new Rect(cardRect.xMax - 140, cardRect.yMin + 4f, 18f, 18f);

                if (Widgets.ButtonImage(minifyButtonRect,  MinifyIcon))
                {
                    SetMinify(!stats.minify, __instance);
                }

                if (Widgets.ButtonImage(collapseButtonRect,CollapseIcon))
                {
                    SetCollapse(!stats.collapse, __instance);
                    
                }


                Color applyAllColor = allCompareTarget == __instance ? Color.white : Color.grey;
                if (Widgets.ButtonImage(applyAllButtonRect, CompareAllIcon,applyAllColor))
                {
                    if (allCompareTarget == __instance)
                    {
                        allCompareTarget = null;
                    }
                    else
                    {
                        allCompareTarget = __instance;
                    }

                    Vector2 pivot = __instance.windowRect.position;
                    pivot.x += __instance.windowRect.width;
                    foreach (var card in infoCardStatsDic.Keys)
                    {
                        if (card == __instance) continue;

                        SetCollapse(stats.collapse, card);
                        SetMinify(stats.minify, card);
                        card.windowRect.size = __instance.windowRect.size;
                        card.windowRect.position = pivot;
                        pivot.x += card.windowRect.width;

                        Pawn mPawn = Method_get_ThingPawn.Invoke(__instance, null) as Pawn;
                        Pawn cPawn = Method_get_ThingPawn.Invoke(card, null) as Pawn;
                        if (mPawn != null && cPawn != null && mPawn.def == cPawn.def)
                        {
                            card.SetTab((Dialog_InfoCard.InfoCardTab)Field_tab.GetValue(__instance));
                        }
                    }
                }

                Color inspectColor = inspectCard == __instance ? Color.white : Color.grey;
                if (Widgets.ButtonImage(InspectButtonRect,InspectAllIcon,inspectColor))
                {
                    if(inspectCard == __instance)
                    {
                        inspectCard = null;
                    }
                    else
                    {
                        inspectCard = __instance;
                    }
                }

            }
        }



        [HarmonyPatch(typeof(Dialog_InfoCard), "Notify_CommonSearchChanged")]
        public static class Patch_Dialog_InfoCard_Notify_CommonSearchChanged
        {
            static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
            {
                return TranspileToInstanceMethod(instructions);
            }
        }

        [HarmonyPatch(typeof(Dialog_InfoCard), "CommonSearchWidget", MethodType.Getter)]
        public static class Patch_Dialog_InfoCard_get_CommonSearchWidget
        {
            static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
            {
                return TranspileToInstanceMethod(instructions);
            }
        }



        [HarmonyPatch(typeof(Dialog_InfoCard), "PushCurrentToHistoryAndClose")]
        public static class Patch_Dialog_InfoCard_PushCurrentToHistoryAndClose
        {
            public static bool Prefix()
            {
                var historyField= typeof(Dialog_InfoCard).GetField("history", BindingFlags.NonPublic | BindingFlags.Static);
                var history= historyField.GetValue(null) as List<Dialog_InfoCard.Hyperlink>;
                history.Clear();
                return false;
            }
        }


        private static List<Dialog_InfoCard> comparingInfoCards = new List<Dialog_InfoCard>();
        private static Vector2 comaprePivot;

        public static void SetComparingInfoCards(IList<Dialog_InfoCard> cards)
        {
            comparingInfoCards = new List<Dialog_InfoCard>( cards);
            comaprePivot = new Vector2(0, (UI.screenHeight - BetterInfoCard.singleton.settings.initCardHeight) / 2);
        }



        public static Thing Get_Thing(this Dialog_InfoCard card) => Field_thing.GetValue(card) as Thing;

        public static void Set_Thing(this Dialog_InfoCard card, Thing thing) => Field_thing.SetValue(card, thing);


        public static void Invoke_Setup(this Dialog_InfoCard card) => Method_Setup.Invoke(card,null);



    }
}
