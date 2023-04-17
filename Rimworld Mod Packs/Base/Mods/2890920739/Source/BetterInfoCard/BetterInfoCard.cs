using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using RimWorld;
using Verse;
using UnityEngine;
using Verse.Steam;



/// <summary>
/// Mod开发计划
/// 1.允许多个InfoCard同时显示，而且要正常显示，每帧更新数据
/// 2.在InfoCard上显示更多的数据，比如身体部位，攻击手段
/// 3.InfoCard之间可以对比，而且InfoCard需要能够折叠
/// 
/// 
/// 11.21
/// 4.可以将属性标记为“收藏”，并且标记数据会保存在存档中
/// 
/// 11.22
/// 5.实现快捷键
/// 6.打开信息卡时焦点默认为搜索框
/// 7.允许在Mod设置中配置（初始折叠，快捷键，初始宽高）
/// 8.在同时选中多个物体时，一键比较
/// 
/// 
/// 
/// 11.24
/// 检查模式似乎点击角色时会无效
/// 搜索的选项点击后，取消搜索时并不会聚焦于搜索项
/// </summary>



namespace BetterInfoCard
{
    public class BetterInfoCard : Mod
    {
        
        public BetterInfoCardModSetting settings;
        public static BetterInfoCard singleton;

        //记录游戏中出现的labelId，因为没有统一的结构获取，而且会被翻译，所以无法预先配置，只能开始游戏后用户自己存储
        public Dictionary<string, (string,StatCategoryDef,string)> recordedLableId = new Dictionary<string, (string, StatCategoryDef, string)>(); 
        public BetterInfoCard(ModContentPack content) : base(content)
        {
            settings = GetSettings<BetterInfoCardModSetting>();
            new Harmony(nameof(BetterInfoCard)).PatchAll();

            if (singleton == null) singleton = this;
        }


        public override string SettingsCategory() => "Better Info Card";


        public override void WriteSettings()
        {
            base.WriteSettings();
        }

        public override void DoSettingsWindowContents(Rect inRect)
        {
            settings.DrawSettings(inRect);
        }
    }


    public class BetterInfoCardModSetting : ModSettings
    {
        private Vector2 scrollPosition = Vector2.zero;

        

        public HashSet<string> FavoriteStatDrawIds = new HashSet<string>();
        public HashSet<string> lesserIsBetterDefNames = new HashSet<string>();
        public KeyCode infoCardHotKey;
        public bool openInfoCardCollapsed;
        public bool openInfoCardFocusSearch;
        public bool hideFilteredStat;
        public float initCardWidth;
        public float initCardHeight;

        public void Reset()
        {
            FavoriteStatDrawIds = new HashSet<string>();
            infoCardHotKey = KeyCode.I;
            openInfoCardCollapsed = false;
            openInfoCardFocusSearch = true;
            hideFilteredStat = true;
            initCardWidth = 950f;
            initCardHeight = 760f;
            lesserIsBetterDefNames = new HashSet<string>(
                new string[] 
                {
                    "Ability_CastingTime",
                    "Ability_RequiredPsylink",
                    "Ability_EntropyGain",
                    "Ability_PsyfocusCost",
                    "Ability_GoodwillImpact",
                    "Ability_DetectChancePerEntropy",
                    "Mass",
                    "Flammability",
                    "WorkToMake",
                    "DeteriorationRate",
                    "FoodPoisonChanceFixedHuman",
                    "FilthMultiplier",
                    "WorkToBuild",
                    "AimingDelayFactor",
                    "RangedCooldownFactor",
                    "MentalBreakThreshold",
                    "ComfyTemperatureMin",
                    "MinimumHandlingSkill",
                    "FilthRate",
                    "MaxNutrition",
                    "PsychicEntropyGain",
                    "CertaintyLossFactor",
                    "CancerRate",
                    "FoodPoisonChance",
                    "MeleeWeapon_CooldownMultiplier",
                    "RangedWeapon_Cooldown",
                    "BandwidthCost",
                    "ControlTakingTime",
                    "MechEnergyUsageFactor",
                    "WastepacksPerRecharge",
                    "MechEnergyLossPerHP",
                });
            
        }

        private static BetterInfoCardModSetting defaultInfoCardSettings;
        public BetterInfoCardModSetting()
        {
            Reset();
        }

        public override void ExposeData()
        {
            if (defaultInfoCardSettings == null)
            {
                defaultInfoCardSettings = new BetterInfoCardModSetting();
                defaultInfoCardSettings.Reset();
            }

            Scribe_Collections.Look(ref FavoriteStatDrawIds, nameof(FavoriteStatDrawIds), LookMode.Value);
            if (Scribe.mode == LoadSaveMode.LoadingVars && FavoriteStatDrawIds == null) FavoriteStatDrawIds = defaultInfoCardSettings.FavoriteStatDrawIds;
            Scribe_Collections.Look(ref lesserIsBetterDefNames, nameof(lesserIsBetterDefNames), LookMode.Value);
            if (Scribe.mode == LoadSaveMode.LoadingVars && lesserIsBetterDefNames == null) lesserIsBetterDefNames = defaultInfoCardSettings.lesserIsBetterDefNames;
            Scribe_Values.Look(ref infoCardHotKey, nameof(infoCardHotKey), defaultInfoCardSettings.infoCardHotKey);
            Scribe_Values.Look(ref openInfoCardCollapsed, nameof(openInfoCardCollapsed), defaultInfoCardSettings.openInfoCardCollapsed);
            Scribe_Values.Look(ref openInfoCardFocusSearch, nameof(openInfoCardFocusSearch), defaultInfoCardSettings.openInfoCardFocusSearch);
            Scribe_Values.Look(ref hideFilteredStat, nameof(hideFilteredStat), defaultInfoCardSettings.hideFilteredStat);
            Scribe_Values.Look(ref initCardWidth, nameof(initCardWidth), defaultInfoCardSettings.initCardWidth);
            Scribe_Values.Look(ref initCardHeight, nameof(initCardHeight), defaultInfoCardSettings.initCardHeight);
            base.ExposeData();
        }

        public void DrawSettings(Rect rect)
        {
            rect.xMin += 20f;
            rect.yMax -= 20f;
            Listing_Standard listingStandard = new Listing_Standard(GameFont.Small);
            Rect rect1 = new Rect(rect.x, rect.y, rect.width, rect.height);
            Rect rect2 = new Rect(0.0f, 0.0f, rect.width - 30f, 15000f);
            Widgets.BeginScrollView(rect, ref scrollPosition, rect2);
            listingStandard.Begin(rect2);
            listingStandard.CheckboxLabeled("Collapsed_by_default".Translate(), ref openInfoCardCollapsed);
            listingStandard.CheckboxLabeled("Focus_Search_by_default".Translate(), ref openInfoCardFocusSearch);
            listingStandard.CheckboxLabeled("Hide_Filtered_Stat".Translate(), ref hideFilteredStat);
            if (listingStandard.ButtonTextLabeled("Open_Info_Card_Hotkey".Translate(), $"[ {infoCardHotKey.ToString()} ]"))
            {
                Find.WindowStack.Add(new Dialog_DefineBindingLightWeight((k)=>infoCardHotKey = k));
            }
            string bufferWidth= initCardWidth.ToString(), bufferHeight = initCardHeight.ToString();
            listingStandard.GapLine();
            listingStandard.TextFieldNumericLabeled<float>("Init_width".Translate(), ref initCardWidth, ref bufferWidth);
            listingStandard.TextFieldNumericLabeled<float>("Init_height".Translate(), ref initCardHeight, ref bufferHeight);
            Rect dropdownRect= listingStandard.GetRect(100);

            if (listingStandard.ButtonText("Reset".Translate()))
            {
                Reset();
            }

            DrawCompareStatList(listingStandard);







            listingStandard.End();
            
            Widgets.EndScrollView();
        }


        private void DrawCompareStatList(Listing_Standard listingStandard)
        {
            listingStandard.Label("Stats_Compare_Detail".Translate());
            listingStandard.GapLine();
            void DrawStatCheckButton(string name, string label, string description)
            {
                if (lesserIsBetterDefNames.Contains(name))
                {
                    if (listingStandard.ButtonTextLabeled(label, "Lesser_is_Better".Translate().Colorize(Color.red), tooltip: description))
                    {
                        lesserIsBetterDefNames.Remove(name);
                    }
                }
                else
                {
                    if (listingStandard.ButtonTextLabeled(label, "Greater_is_Better".Translate().Colorize(Color.green), tooltip: description))
                    {
                        lesserIsBetterDefNames.Add(name);
                    }
                }
            }

            Dictionary<StatCategoryDef, List<(string, string, string)>> categoryedAllStatDraws = new Dictionary<StatCategoryDef, List<(string, string, string)>>();
            foreach (var stat in DefDatabase<StatDef>.AllDefs)
            {
                if (stat == null || stat.category == null) continue;
                if (!categoryedAllStatDraws.ContainsKey(stat.category))
                {
                    categoryedAllStatDraws.Add(stat.category, new List<(string, string, string)>());
                }
                categoryedAllStatDraws[stat.category].Add((stat.defName, stat.label, stat.description));
            }

            foreach (var labelRecord in BetterInfoCard.singleton.recordedLableId.Values)
            {
                if (labelRecord.Item2 == null) continue;
                if (!categoryedAllStatDraws.ContainsKey(labelRecord.Item2))
                {
                    categoryedAllStatDraws.Add(labelRecord.Item2, new List<(string, string, string)>());
                }
                categoryedAllStatDraws[labelRecord.Item2].Add((labelRecord.Item1, $"\"{labelRecord.Item1}\"", labelRecord.Item3));
            }





            foreach (var cata in categoryedAllStatDraws.Keys)
            {
                listingStandard.Label(cata.label);
                listingStandard.GapLine();
                foreach (var statDrawInfo in categoryedAllStatDraws[cata])
                {
                    DrawStatCheckButton(statDrawInfo.Item1, statDrawInfo.Item2, statDrawInfo.Item3);
                }
            }

            listingStandard.GapLine();
            foreach (var stat in DefDatabase<StatDef>.AllDefs)
            {
                if (stat.category == null)
                {
                    DrawStatCheckButton(stat.defName, stat.label, stat.description);
                }
            }
            foreach (var labelRecord in BetterInfoCard.singleton.recordedLableId.Values)
            {
                if (labelRecord.Item2 == null)
                {
                    DrawStatCheckButton(labelRecord.Item1, $"\"{labelRecord.Item1}\"", labelRecord.Item3);
                }
            }
        }






        public class Dialog_DefineBindingLightWeight : Window
        {
            protected Vector2 windowSize = new Vector2(400f, 200f);
            private System.Action<KeyCode> setter;

            public Dialog_DefineBindingLightWeight(System.Action<KeyCode> setter)
            {
                this.setter = setter;
                closeOnAccept = false;
                closeOnCancel = false;
                forcePause = true;
                onlyOneOfTypeAllowed = true;
                absorbInputAroundWindow = true;
            }

            public override void DoWindowContents(Rect inRect)
            {
                Text.Anchor = TextAnchor.MiddleCenter;
                if (SteamDeck.IsSteamDeckInNonKeyboardMode)
                {
                    Widgets.Label(inRect, "PressAnyKeyOrEscController".Translate().Resolve().AdjustedForKeys());
                }
                else
                {
                    Widgets.Label(inRect, "PressAnyKeyOrEsc".Translate());
                }

                Text.Anchor = TextAnchor.UpperLeft;
                if (Event.current.isKey && Event.current.type == EventType.KeyDown && Event.current.keyCode != 0)
                {
                    if (Event.current.keyCode != KeyCode.Escape)
                    {
                        setter.Invoke(Event.current.keyCode);
                    }

                    Close();
                    Event.current.Use();
                }
            }
        }
    }
}
