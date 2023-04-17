using HarmonyLib;
using RimWorld;
using Verse;
using System.Reflection;
using System.Collections.Generic;
using UnityEngine;
using RimWorld.Planet;
using System.Reflection.Emit;
using System;

namespace BetterInfoCard
{
    public static class StatDrawEntry_Patch
    {
        static FieldInfo field_value = typeof(StatDrawEntry).GetField("value", BindingFlags.NonPublic | BindingFlags.Instance);
        static FieldInfo field_labelInt = typeof(StatDrawEntry).GetField("labelInt", BindingFlags.NonPublic | BindingFlags.Instance);
        static FieldInfo field_overrideReportText = typeof(StatDrawEntry).GetField("overrideReportText", BindingFlags.NonPublic | BindingFlags.Instance);

        public static HashSet<string> FavoriteStatDrawIds => BetterInfoCard.singleton.settings.FavoriteStatDrawIds;
        //public struct StatDrawEntryId : IExposable
        //{
        //    // 每一项需要这两个字段来标识
        //    // Rimworld在加载mod时似乎还不会加载StatDef，所以不能直接存StatDef？
        //    public string stat_defName;
        //    public string labelId;

        //    public StatDrawEntryId(StatDef stat, string labelId)
        //    {
        //        stat_defName = stat?.defName??null;
        //        this.labelId = labelId;
        //    }

        //    public void ExposeData()
        //    {
                
        //        Scribe_Values.Look(ref stat_defName, nameof(stat_defName));
        //        Scribe_Values.Look(ref labelId, nameof(labelId));
        //    }



        //    public override bool Equals(object obj)
        //    {
        //        if(obj is StatDrawEntryId other)
        //        {
        //            return stat_defName == other.stat_defName && other.labelId == labelId;
        //        }
        //        return false;
        //    }

        //    public override int GetHashCode()
        //    {
        //        return stat_defName?.GetHashCode()??0 + labelId?.GetHashCode()??0;
        //    }
        //}

        
        public static Texture2D StartEmptyIcon => ContentFinder<Texture2D>.Get("UI/Widgets/Star_Empty");
        public static Texture2D StarFillIcon => ContentFinder<Texture2D>.Get("UI/Widgets/Star_Fill");


        private static System.Text.RegularExpressions.Regex matchNumberRegex = new System.Text.RegularExpressions.Regex(@"[+-]?([0-9]*[.])?[0-9]+");

        [HarmonyPatch(typeof(StatDrawEntry), "Draw")]
        public static class Patch_StatDrawEntry_Draw
        {
            public static bool Prefix(ref StatDrawEntry __instance,ref float __result, float x, float y, float width, bool selected, bool highlightLabel,
                bool lowlightLabel, Action clickedCallback, Action mousedOverCallback, Vector2 scrollPosition, Rect scrollOutRect, string valueCached = null)
            {

                string statDrawEntryId = (__instance.stat?.defName) ?? __instance.Get_labelInt(); //信息卡中显示的属性可能没有对应的Stat

                //记录游戏中遭遇的label数据
                if(__instance.stat==null && statDrawEntryId!=null)
                {
                    if(!BetterInfoCard.singleton.recordedLableId.ContainsKey(statDrawEntryId))
                    {
                        
                        BetterInfoCard.singleton.recordedLableId.Add(statDrawEntryId, (statDrawEntryId, __instance.category,__instance.Get_overrideReportText()));
                    }
                }


                float width1 = width * 0.45f;
                string text = valueCached ?? __instance.ValueString;
                Rect rect = new Rect(8f, y, width, Text.CalcHeight(text, width1));
                if (!(y - scrollPosition.y + rect.height < 0f) && !(y - scrollPosition.y > scrollOutRect.height))
                {

                    StatDrawEntry compareTargetEntry = null;
                     
                    if(Dialog_InfoCard_Patch.allCompareTarget!=null)
                    {
                        var targetUti = Dialog_InfoCard_Patch.infoCardStatsDic[Dialog_InfoCard_Patch.allCompareTarget];
                        compareTargetEntry = targetUti.GetStatEntry(__instance.stat, __instance.Get_labelInt());
                    }
                    if(StatsReportUtility_Instanced.compareFocusEntry.HasValue &&
                        __instance.stat == StatsReportUtility_Instanced.compareFocusEntry.Value.Item2.stat && 
                        __instance.Get_labelInt() == StatsReportUtility_Instanced.compareFocusEntry.Value.Item3)
                    {
                        compareTargetEntry = StatsReportUtility_Instanced.compareFocusEntry.Value.Item2;
                    }


                    bool isFavorite = FavoriteStatDrawIds.Contains(statDrawEntryId);

                    GUI.color = Color.white;
                    if (selected)
                    {
                        Widgets.DrawHighlightSelected(rect);
                    }
                    else if (Mouse.IsOver(rect))
                    {
                        Widgets.DrawHighlight(rect);
                    }

                    if (highlightLabel || isFavorite)
                    {
                        Widgets.DrawTextHighlight(rect);
                    }

                    if (lowlightLabel)
                    {
                        GUI.color = Color.grey;
                    }

                    //收藏按钮
                    Rect rect1 = new Rect(rect.x,rect.y, Text.LineHeight, Text.LineHeight);
                    
                    if (isFavorite)
                    {
                        GUI.DrawTexture(rect1, StartEmptyIcon);
                        if (Widgets.ButtonImage(rect1, StarFillIcon))
                        {
                            FavoriteStatDrawIds.Remove(statDrawEntryId);
                            BetterInfoCard.singleton.WriteSettings();
                        }
                    }
                    else
                    {
                        if (Mouse.IsOver(rect))
                        {
                            GUI.DrawTexture(rect1, StartEmptyIcon);
                            if (Widgets.ButtonImage(rect1, StartEmptyIcon))
                            {
                                FavoriteStatDrawIds.Add(statDrawEntryId);
                                BetterInfoCard.singleton.WriteSettings();
                            }
                        }
                    }
                    



                    Rect rect2 = rect;
                    rect2.width -= width1;

                    rect2.x += rect1.width;
                    rect2.width -= rect1.width;

                    string label = __instance.LabelCap;

                    if (isFavorite) label = label.Colorize(Color.yellow);
                    if (compareTargetEntry != null)
                        label = "->".Colorize(compareTargetEntry==__instance? Color.cyan:Color.yellow) + label;

                    Widgets.Label(rect2, label);

                    if(compareTargetEntry!=null)
                    {
                        float? mValue = GetParseValue(__instance);

                        float? focusValue = GetParseValue(compareTargetEntry);

                        bool GreaterIsRed = BetterInfoCard.singleton.settings.lesserIsBetterDefNames.Contains(statDrawEntryId);
                        if(mValue!=null && focusValue!=null)
                        {
                            if (mValue > focusValue)
                            {
                                text = text.Colorize(GreaterIsRed ? Color.red : Color.green);
                            }
                            else if (mValue < focusValue)
                            {
                                text = text.Colorize(GreaterIsRed ? Color.green : Color.red);
                            }
                            else if (mValue == focusValue && compareTargetEntry != __instance)
                            {
                                text = text.Colorize(Color.yellow);
                            }
                        }

                        
                    }

                    Rect rect3 = rect;
                    rect3.x = rect2.xMax;
                    rect3.width = width1;
                    Widgets.Label(rect3, text);
                    GUI.color = Color.white;
                    if (__instance.stat != null && Mouse.IsOver(rect))
                    {
                        StatDef localStat = __instance.stat;
                        TooltipHandler.TipRegion(rect, new TipSignal(() => localStat.LabelCap + ": " + localStat.description, __instance.stat.GetHashCode()));
                    }

                    if (Widgets.ButtonInvisible(rect))
                    {
                        clickedCallback();
                    }

                    if (Mouse.IsOver(rect))
                    {
                        mousedOverCallback();
                    }
                }

                __result= rect.height;


                return false;
            }
        }
        


        //有些数据是不带数值的，所以要用正则去匹配
        private static float? GetParseValue(StatDrawEntry entry)
        {
            float res = (float)field_value.GetValue(entry);
            if (res != 0) return res;
            var match = matchNumberRegex.Match(entry.ValueString);
            if(match.Success)
            {
                if (!float.TryParse(match.Value, out res)) return null;
                return res;
            }
            else
            {
                return null;
            }
            


            //if (!float.TryParse(entry.ValueString, out res)) focusValue = (float)field_value.GetValue(entry);
            //return focusValue;
        }


        
        public static string Get_labelInt(this StatDrawEntry entry) => field_labelInt.GetValue(entry) as string;
        public static void Set_labelInt(this StatDrawEntry entry, string labelInt) => field_labelInt.SetValue(entry, labelInt);
        public static string Get_overrideReportText(this StatDrawEntry entry) => field_overrideReportText.GetValue(entry) as string;

    }
}
