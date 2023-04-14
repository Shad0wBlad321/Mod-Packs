using System;
using System.Collections.Generic;
using System.Reflection;
using System.Diagnostics;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;
using Verse.AI;
using Verse.AI.Group;
using Verse.Sound;
using Verse.Noise;
using Verse.Grammar;
using RimWorld;
using RimWorld.Planet;

namespace N1UiExp
{
    public class Dialog_RenameBill : Dialog_Rename
    {
        private Bill bill;
        private readonly RecipeDef _currentName;

        private bool focusedRenameField;

        private int startAcceptingInputAtFrame;

        private bool AcceptsInput => startAcceptingInputAtFrame <= Time.frameCount;

        public override Vector2 InitialSize => new Vector2(280f, 185f);

        public Dialog_RenameBill(Bill bill, Building_WorkTable selectedTable) : base()
        {
            this.bill = bill;
            this._currentName = selectedTable.def.AllRecipes.FirstOrDefault(recipe => recipe.defName == bill.recipe.defName);
        }

        public override void DoWindowContents(Rect inRect)
        {
            Text.Font = GameFont.Medium;
            Widgets.Label(new Rect(0f, 0f, inRect.width, 30f), "N1.Rename".Translate());
            Text.Font = GameFont.Small;
            bool flag = false;
            if (Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.Return)
            {
                flag = true;
                Event.current.Use();
            }
            GUI.SetNextControlName("RenameField");
            string text = Widgets.TextField(new Rect(0f, 35f, inRect.width, 35f), curName);
            if (AcceptsInput && text.Length < MaxNameLength)
            {
                curName = text;
            }
            else if (!AcceptsInput)
            {
                ((TextEditor)GUIUtility.GetStateObject(typeof(TextEditor), GUIUtility.keyboardControl)).SelectAll();
            }
            if (!focusedRenameField)
            {
                UI.FocusControl("RenameField", this);
                focusedRenameField = true;
            }
            Text.Font = GameFont.Tiny;
            Widgets.Label(new Rect(0f, 78f, inRect.width, GUI.skin.label.CalcHeight(new GUIContent("N1.RenameDesc".Translate()), inRect.width)), "N1.RenameDesc".Translate());
            Text.Font = GameFont.Small;
            if (!(Widgets.ButtonText(new Rect(0f, 115f, inRect.width, 35f), "OK") || flag))
            {
                return;
            }
            AcceptanceReport acceptanceReport = NameIsValid(curName);
            if (!acceptanceReport.Accepted)
            {
                if (acceptanceReport.Reason.NullOrEmpty())
                {
                    Messages.Message("NameIsInvalid".Translate(), MessageTypeDefOf.RejectInput, historical: false);
                }
                else
                {
                    Messages.Message(acceptanceReport.Reason, MessageTypeDefOf.RejectInput, historical: false);
                }
            }
            else
            {
                SetName(curName);
                Find.WindowStack.TryRemove(this);
            }
        }

        protected override void SetName(string name)
        {
            if (!string.IsNullOrEmpty(name))
            {
                RecipeDef newRecipe = new RecipeDef();
                newRecipe.defName = bill.recipe.defName;
                newRecipe.label = name;
                newRecipe.jobString = bill.recipe.jobString;
                newRecipe.workerClass = bill.recipe.workerClass;
                newRecipe.workAmount = bill.recipe.workAmount;
                newRecipe.ingredients = bill.recipe.ingredients;
                newRecipe.products = bill.recipe.products;
                newRecipe.skillRequirements = bill.recipe.skillRequirements;
                newRecipe.researchPrerequisite = bill.recipe.researchPrerequisite;
                newRecipe.workSkill = bill.recipe.workSkill;
                newRecipe.workSkillLearnFactor = bill.recipe.workSkillLearnFactor;
                newRecipe.description = bill.recipe.description;
                newRecipe.fixedIngredientFilter = bill.recipe.fixedIngredientFilter;
                newRecipe.workSpeedStat = bill.recipe.workSpeedStat;
                newRecipe.efficiencyStat = bill.recipe.efficiencyStat;
                newRecipe.recipeUsers = bill.recipe.recipeUsers;
                newRecipe.modExtensions = bill.recipe.modExtensions;
    
                // Задаем новый рецепт для счета
                bill.recipe = newRecipe;
            }
            else {
                bill.recipe = _currentName;
            }
        }
        protected override AcceptanceReport NameIsValid(string name)
        {
            return true;
        }
    }
    public class RecipeInfo
    {
        public RecipeDef Recipe { get; private set; }
        public bool IsExpanded { get; set; }
        public float Height { get; set; }
        public RecipeInfo(RecipeDef recipe, bool isExpanded = false, float height = 30f)
        {
            Recipe = recipe;
            IsExpanded = isExpanded;
            Height = height;
        }
    }
    public class WorkTableInfo
    {
        public string name;
        public int id;
        
        public WorkTableInfo(string name, int id)
        {
            this.name = name;
            this.id = id;
        }
    }

    [StaticConstructorOnStartup]
    public class BillTabButton : MainTabWindow
    {
        private int curTab = 0;
        private int curTabRecipe = 0;
        private string curRecipe = null;
        private int curRect = 0;
        // private List<TabRecord> tabs = new List<TabRecord>();
        private Dictionary<int, string> tabs = new Dictionary<int, string>();
        
        private Dictionary<int, Dictionary<int, bool>> expandedStatesByTab = new Dictionary<int, Dictionary<int, bool>>();
        private Dictionary<int, Dictionary<int, float>> expandedStatesByHeight = new Dictionary<int, Dictionary<int, float>>();
        private BillRepeatModeDef repeatMode = BillRepeatModeDefOf.RepeatCount;
        private int repeatCount = 1;
        private int targetCount = 10;
        private bool pauseWhenSatisfied;
	    private int unpauseWhenYouHave = 5;

        private bool isClipboardAdd = false;


        private float scrollSumTables;
        private float scrollSumRecipes;
        private float scrollSumBills;
        private Texture2D icnDeleteBtn = ContentFinder<Texture2D>.Get("UI/icn_delete");
        private Texture2D icnDetailsBtn = ContentFinder<Texture2D>.Get("UI/icn_details");
        private Texture2D icnPauseBtn = ContentFinder<Texture2D>.Get("UI/icn_pause");
        private Texture2D icnUnpauseBtn = ContentFinder<Texture2D>.Get("UI/icn_unpause");
        private Texture2D icnArrowUpBtn = ContentFinder<Texture2D>.Get("UI/icn_arrow_up");
        private Texture2D icnArrowDownBtn = ContentFinder<Texture2D>.Get("UI/icn_arrow_down");
        private Texture2D icnBillManager = ContentFinder<Texture2D>.Get("UI/icn_billmanager");
        private Texture2D icnPlus = ContentFinder<Texture2D>.Get("UI/icn_plus");
        private Texture2D icnMinus = ContentFinder<Texture2D>.Get("UI/icn_minus");
        private Texture2D icnTabTables = ContentFinder<Texture2D>.Get("UI/icn_tab_tables");
        private Texture2D icnTabRecipes = ContentFinder<Texture2D>.Get("UI/icn_tab_recipes");
        private Texture2D icnTabQueues = ContentFinder<Texture2D>.Get("UI/icn_tab_queues");
        private Texture2D icnTabBills = ContentFinder<Texture2D>.Get("UI/icn_tab_bills");


        private List<Building_WorkTable> workTables;
        Vector2 scrollPositionTables = Vector2.zero;
        Vector2 scrollPositionRecipes = Vector2.zero;
        Vector2 scrollPositionBills = Vector2.zero;

        private bool focusedRenameField;
        private int startAcceptingInputAtFrame;
        private bool AcceptsInput => startAcceptingInputAtFrame <= Time.frameCount;

        public override void PreOpen()
        {
            base.PreOpen();
            tabs.Clear();
            curRecipe = null;
            expandedStatesByTab.Clear();
            expandedStatesByHeight.Clear();

            workTables = Find.CurrentMap.listerBuildings.allBuildingsColonist.FindAll(b => b is Building_WorkTable).Cast<Building_WorkTable>().ToList();
            

            if (workTables.Count == 0)
            {
                
            }
            else
            {
                foreach (Building_WorkTable table in workTables)
                {   
                    var tabInfo = new WorkTableInfo(table.LabelCap.Truncate(300), table.thingIDNumber);
                    tabs.Add(tabInfo.id, tabInfo.name);
                    expandedStatesByTab[tabInfo.id] = new Dictionary<int, bool>();
                    expandedStatesByHeight[tabInfo.id] = new Dictionary<int, float>();
                }
                curTab = workTables[0].thingIDNumber;
            }
        }

        private void DrawSearchInput(Rect rect)
        {
            string searchText = "";

            Widgets.TextField(new Rect(rect.xMax - 188f, rect.yMin, 188f, rect.height), searchText);
            if (Event.current.type == EventType.KeyDown)
            {
                // Здесь можно выполнить нужный код при нажатии Enter
                Log.Message("Введенный текст: " + searchText);
            }
        }

        public override void DoWindowContents(Rect rect)
        {
            Rect tabWindowRect = rect;
            
            Rect windowHeaderRect = new Rect(rect.xMin, rect.yMin, rect.width, 30f);
            Widgets.DrawTextureFitted(new Rect(windowHeaderRect.xMin, windowHeaderRect.yMin, 30f, 30f), icnBillManager, 1f);
            Text.Font = GameFont.Medium;
            Widgets.Label(new Rect(windowHeaderRect.xMin + 40f, windowHeaderRect.yMin, windowHeaderRect.width, windowHeaderRect.height), "Bill Manager");
            Text.Font = GameFont.Small;
            // DrawSearchInput(windowHeaderRect);
            

            Widgets.DrawBoxSolid(new Rect(windowHeaderRect.xMin, windowHeaderRect.yMax + 18f, windowHeaderRect.width, 1f), new Color(1f, 1f, 1f, 0.1f));

            Rect managerContentRect = new Rect(windowHeaderRect.xMin, windowHeaderRect.yMax + 18f + 19f, windowHeaderRect.width, tabWindowRect.height - 30f - 18f - 1f - 18f);
            Color blockBGColor = new Color(0.165f, 0.169f, 0.173f);
            Color blockOutlineColor = new Color(0.529f, 0.529f, 0.529f);

            // Work Tables
            Rect workTablesBlockRect = new Rect(managerContentRect.xMin, managerContentRect.yMin, (managerContentRect.width - 8f) / 3f, managerContentRect.height);
            Widgets.DrawBoxSolidWithOutline(workTablesBlockRect, blockBGColor, blockOutlineColor);
            Rect workTablesBlockHeaderRect = new Rect(workTablesBlockRect.xMin, workTablesBlockRect.yMin, workTablesBlockRect.width, 30f);
            DrawBlockHeader(workTablesBlockHeaderRect, "N1.blockHeader_workTables", icnTabTables);
            Rect tablesContentRect = new Rect(workTablesBlockRect.xMin, workTablesBlockHeaderRect.yMax, workTablesBlockRect.width, workTablesBlockRect.height - workTablesBlockHeaderRect.height);
            DrawTablesContent(tablesContentRect);

            // Recipes
            Rect recipesBlockRect = new Rect(managerContentRect.xMin + ((managerContentRect.width - 8f) / 3f) + 4f, managerContentRect.yMin, (managerContentRect.width - 8f) / 3f, managerContentRect.height);
            Widgets.DrawBoxSolidWithOutline(recipesBlockRect, blockBGColor, blockOutlineColor);
            Rect recipesBlockHeaderRect = new Rect(recipesBlockRect.xMin, recipesBlockRect.yMin, recipesBlockRect.width, 30f);
            DrawBlockHeaderTabs(recipesBlockHeaderRect, "N1.blockHeader_recipesManage", "N1.blockHeader_queuesManage");
            Rect recipesContentRect = new Rect(recipesBlockRect.xMin, recipesBlockHeaderRect.yMax, recipesBlockRect.width, recipesBlockRect.height - recipesBlockHeaderRect.height);
            DrawRecipesContent(recipesContentRect);

            //Bills
            Rect billsBlockRect = new Rect(managerContentRect.xMin + (((managerContentRect.width - 8f) / 3f) * 2f) + 8f, managerContentRect.yMin, (managerContentRect.width - 8f) / 3f, managerContentRect.height);
            Widgets.DrawBoxSolidWithOutline(billsBlockRect, blockBGColor, blockOutlineColor);
            Rect billssBlockHeaderRect = new Rect(billsBlockRect.xMin, billsBlockRect.yMin, billsBlockRect.width, 30f);
            DrawBlockHeader(billssBlockHeaderRect, "N1.blockHeader_bills", icnTabBills);
            Rect billsContentRect = new Rect(billsBlockRect.xMin, billssBlockHeaderRect.yMax, billsBlockRect.width, billsBlockRect.height - billssBlockHeaderRect.height);
            DrawBillsContent(billsContentRect);


            if (Mouse.IsOver(workTablesBlockRect)) {
                curRect = 1;
            }
            else if (Mouse.IsOver(recipesBlockRect))
            {
                curRect = 2;
            }
            else if (Mouse.IsOver(billsBlockRect))
            {
                curRect = 3;
            }
            else {
                curRect = 0;
            }
        }
        
        private void DrawBillsContent(Rect rect)
        {
            if (curRecipe != null)
            {
                Building_WorkTable selectedTable = workTables.Find(t => t.thingIDNumber == curTab);
                List<Bill> bills = selectedTable.BillStack.Bills.Where(bill => bill.recipe.defName == curRecipe).ToList();
                List<RecipeInfo> recipeInfos = selectedTable.def.AllRecipes
                            .Where(recipe => recipe.AvailableNow)
                            .Select(recipe => new RecipeInfo(recipe))
                            .ToList();
                RecipeInfo recipeDef = recipeInfos.Find(recipe => recipe.Recipe.defName == curRecipe);
                Rect scrollRect = new Rect(rect.xMin, rect.yMin, rect.width, rect.height);
                Rect scrollViewRect = new Rect(0f, 0f, rect.width - 16f, scrollSumBills);
                if (scrollSumBills < scrollRect.height)
                {
                    scrollViewRect = new Rect(0f, 0f, rect.width, scrollSumBills);
                }
                
                
                Widgets.BeginScrollView(scrollRect, ref scrollPositionBills, scrollViewRect);

                Listing_Standard list = new Listing_Standard();
                list.Begin(scrollViewRect);
                float itemHeight = 72f;
                float addBtnHeight = 40f;
                float contentHeight = bills.Count * itemHeight + addBtnHeight;
                Rect rectItem = list.GetRect(addBtnHeight);
                Rect rectContent = new Rect(rectItem.xMin, rectItem.yMin, rectItem.width, contentHeight);

                Rect createBillButtonRect = new Rect(rectContent.xMin, rectContent.yMin, rectContent.width, addBtnHeight);
                Widgets.DrawHighlightIfMouseover(createBillButtonRect);
                GUI.Label(new Rect(createBillButtonRect.xMin + 8f, createBillButtonRect.yMin + 9f, createBillButtonRect.width, createBillButtonRect.height), "AddBill".Translate());
                if (Widgets.ButtonInvisible(createBillButtonRect))
                {
                    selectedTable.billStack.AddBill(recipeDef.Recipe.MakeNewBill());
                }
                Widgets.DrawBoxSolid(new Rect(createBillButtonRect.xMin, createBillButtonRect.yMax - 1f, createBillButtonRect.width, 1f), new Color(1f, 1f, 1f, 0.1f));
                
                if (bills.Count > 0)
                {
                    foreach (Bill bill in bills)
                    {
                        Bill_Production productionBill = bill as Bill_Production;
                        BillRepeatModeDef repeatMode = productionBill.repeatMode;

                        Rect billRect = new Rect(rectContent.xMin, rectContent.yMin + addBtnHeight, rectContent.width, itemHeight);
                        float iconSize = 24f;
                        Rect closeBtnRect = new Rect(billRect.xMax - iconSize, billRect.yMin + 8f, iconSize, iconSize);
                        Rect infoBtnRect = new Rect(billRect.xMax - iconSize * 2f, billRect.yMin + 8f, iconSize, iconSize);
                        Rect pauseBtnRect = new Rect(billRect.xMax - iconSize * 3f, billRect.yMin + 8f, iconSize, iconSize);
                        Rect copyBtnRect = new Rect(billRect.xMax - iconSize * 4f, billRect.yMin + 8f, iconSize, iconSize);

                        int repeatCount = productionBill.repeatCount;
                        int targetCount = productionBill.targetCount;
                        bool paused = productionBill.paused;
                        int unpauseWhenYouHave = productionBill.unpauseWhenYouHave;

                        string billText = $"#{bill.billStack.IndexOf(bill) + 1} | {bill.LabelCap.Truncate(200)}";

                        
                        float textHeight = Text.CalcHeight(billText, billRect.width);
                        string RepeatInfoText = "TEST";
                        if (repeatMode == BillRepeatModeDefOf.Forever)
                        {
                            RepeatInfoText =  "Forever".Translate();
                        }
                        if (repeatMode == BillRepeatModeDefOf.RepeatCount)
                        {
                            RepeatInfoText = repeatCount + "x";
                        }
                        if (repeatMode == BillRepeatModeDefOf.TargetCount)
                        {
                            RepeatInfoText = recipeDef.Recipe.WorkerCounter.CountProducts(bill as Bill_Production) + "/" + targetCount;
                        }
                        Rect billTextRect = new Rect(billRect.xMin + 8f, billRect.yMin + 9f, billRect.width, billRect.height);
                        GUI.Label(billTextRect, billText);
                        Rect billTextRawRect = new Rect(billTextRect.xMin - 4f, billTextRect.yMin, Text.CalcSize(billText).x + 8f, Text.CalcSize(billText).y);

                        if (Mouse.IsOver(billTextRawRect))
                        {
                            Widgets.DrawHighlight(billTextRawRect);
                            TooltipHandler.TipRegionByKey(billTextRect, "N1.Rename".Translate());
                        }

                        if (Widgets.ButtonInvisible(billTextRawRect))
                        {
                            Find.WindowStack.Add(new Dialog_RenameBill(bill, selectedTable));
                        }

                        Rect repeatModeRect = new Rect(billRect.xMin + 8f, billRect.yMax - 32f, billRect.width - 16f - 50f, 24f);
                        Rect minusBtnRect = new Rect(repeatModeRect.xMax + 1f, repeatModeRect.yMin, 24f, 24f);
                        Rect plusBtnRect = new Rect(minusBtnRect.xMax + 1f, minusBtnRect.yMin, 24f, 24f);
                        Widgets.DrawTextureFitted(minusBtnRect, icnMinus, 0.75f);
                        Widgets.DrawTextureFitted(plusBtnRect, icnPlus, 0.75f);

                        if (Mouse.IsOver(repeatModeRect))
                        {
                            Widgets.DrawBoxSolidWithOutline(repeatModeRect, new Color(1f, 1f, 1f, 0.1f), new Color(1f, 1f, 1f, 0.1f));
                        }
                        else
                        {
                            Widgets.DrawBoxSolidWithOutline(repeatModeRect, new Color(1f, 1f, 1f, 0.05f), new Color(1f, 1f, 1f, 0.05f));
                        }

                        Widgets.Label(new Rect(repeatModeRect.xMin + 4f, repeatModeRect.yMin + 2f, repeatModeRect.width, repeatModeRect.height), repeatMode.LabelCap.Resolve().PadRight(20));
                        
                        if (Widgets.ButtonInvisible(repeatModeRect))
                        {
                            BillRepeatModeUtility.MakeConfigFloatMenu(productionBill);
                        }

                        if (Mouse.IsOver(minusBtnRect))
                        {
                            Widgets.DrawBoxSolidWithOutline(minusBtnRect, new Color(1f, 1f, 1f, 0.1f), new Color(1f, 1f, 1f, 0.1f));
                        }
                        else
                        {
                            Widgets.DrawBoxSolidWithOutline(minusBtnRect, new Color(1f, 1f, 1f, 0.05f), new Color(1f, 1f, 1f, 0.05f));
                        }

                        if (Mouse.IsOver(plusBtnRect))
                        {
                            Widgets.DrawBoxSolidWithOutline(plusBtnRect, new Color(1f, 1f, 1f, 0.1f), new Color(1f, 1f, 1f, 0.1f));
                        }
                        else
                        {
                            Widgets.DrawBoxSolidWithOutline(plusBtnRect, new Color(1f, 1f, 1f, 0.05f), new Color(1f, 1f, 1f, 0.05f));
                        }

                        if (Widgets.ButtonInvisible(plusBtnRect))
                        {
                            if (repeatMode == BillRepeatModeDefOf.Forever)
                            {
                                repeatMode = BillRepeatModeDefOf.RepeatCount;
                                repeatCount = 1;
                            }
                            else if (repeatMode == BillRepeatModeDefOf.TargetCount)
                            {
                                int num = bill.recipe.targetCountAdjustment * GenUI.CurrentAdjustmentMultiplier();
                                targetCount += num;
                                unpauseWhenYouHave += num;
                            }
                            else if (repeatMode == BillRepeatModeDefOf.RepeatCount)
                            {
                                repeatCount += GenUI.CurrentAdjustmentMultiplier();
                            }
                            SoundDefOf.DragSlider.PlayOneShotOnCamera();
                        }
                        if (Widgets.ButtonInvisible(minusBtnRect))
                        {
                            if (repeatMode == BillRepeatModeDefOf.Forever)
                            {
                                repeatMode = BillRepeatModeDefOf.RepeatCount;
                                repeatCount = 1;
                            }
                            else if (repeatMode == BillRepeatModeDefOf.TargetCount)
                            {
                                int num2 = recipeDef.Recipe.targetCountAdjustment * GenUI.CurrentAdjustmentMultiplier();
                                targetCount = Mathf.Max(0, targetCount - num2);
                                unpauseWhenYouHave = Mathf.Max(0, unpauseWhenYouHave - num2);
                            }
                            else if (repeatMode == BillRepeatModeDefOf.RepeatCount)
                            {
                                repeatCount = Mathf.Max(0, repeatCount - GenUI.CurrentAdjustmentMultiplier());
                            }
                            SoundDefOf.DragSlider.PlayOneShotOnCamera();
                        }

                        Text.Anchor = TextAnchor.MiddleRight;
                        Widgets.Label(new Rect(repeatModeRect.x - 4f, repeatModeRect.y + 1f, repeatModeRect.width, repeatModeRect.height), RepeatInfoText);
                        Text.Anchor = TextAnchor.UpperLeft;

                        if (Widgets.ButtonImage(pauseBtnRect, !bill.suspended ? icnPauseBtn : icnUnpauseBtn))
                        {
                            bill.suspended = !bill.suspended;
                        }
                        if (Mouse.IsOver(pauseBtnRect))
                        {
                            TooltipHandler.TipRegion(pauseBtnRect, !bill.suspended ? "SuspendBillTip".Translate() : "UnSuspendBillTip".Translate());
                        }

                        if (Widgets.ButtonImage(closeBtnRect, icnDeleteBtn))
                        {
                            selectedTable.billStack.Delete(bill);
                        }
                        if (Mouse.IsOver(closeBtnRect))
                        {
                            TooltipHandler.TipRegion(closeBtnRect, "Delete".Translate());
                        }

                        if (Widgets.ButtonImage(infoBtnRect, icnDetailsBtn))
                        {
                            Find.WindowStack.Add(new Dialog_BillConfig(bill as Bill_Production, IntVec3.FromVector3(UI.MouseMapPosition())));
                        }
                        if (Mouse.IsOver(infoBtnRect))
                        {
                            TooltipHandler.TipRegion(infoBtnRect, "Details".Translate() + "...");
                        }

                        if (Widgets.ButtonImage(copyBtnRect, TexButton.Copy, Color.white))
                        {
                            BillUtility.Clipboard = bill;
                        }
                        if (Mouse.IsOver(copyBtnRect))
                        {
                            TooltipHandler.TipRegion(copyBtnRect, "CopyBillTip".Translate() + ": " + bill.LabelCap);
                        }
                        
                        Widgets.DrawBoxSolid(new Rect(billRect.xMin, billRect.yMax - 1f, billRect.width, 1f), new Color(1f, 1f, 1f, 0.1f));

                        rectContent.yMin += itemHeight;
                        productionBill.paused = paused;
                        productionBill.repeatMode = repeatMode;
                        productionBill.repeatCount = repeatCount;
                        productionBill.targetCount = targetCount;
                        productionBill.unpauseWhenYouHave = unpauseWhenYouHave;
                    }
                }
                scrollSumBills = bills.Count * itemHeight + addBtnHeight;
                list.End();
                Widgets.EndScrollView();
            }
        }
        private void DrawRecipesContent(Rect rect)
        {
            Building_WorkTable selectedTable = workTables.Find(t => t.thingIDNumber == curTab);
            if (selectedTable == null)
            {
                
            }
            else
            {
                List<Bill> bills = selectedTable.BillStack.Bills;
                List<RecipeInfo> recipeInfos = selectedTable.def.AllRecipes
                    .Where(recipe => recipe.AvailableNow)
                    .Select(recipe => new RecipeInfo(recipe))
                    .ToList();

                Rect scrollRect = new Rect(rect.xMin, rect.yMin, rect.width, rect.height);
                Rect scrollViewRect = new Rect(0f, 0f, rect.width - 16f, scrollSumRecipes);
                if (scrollSumRecipes < scrollRect.height)
                {
                    scrollViewRect = new Rect(0f, 0f, rect.width, scrollSumRecipes);
                }
                switch (curTabRecipe)
                {
                    case 0:
                        Widgets.BeginScrollView(scrollRect, ref scrollPositionRecipes, scrollViewRect);

                        Listing_Standard list = new Listing_Standard();
                        list.Begin(scrollViewRect);
                        for (int i = 0; i < recipeInfos.Count(); i++)
                        {
                            float rowHeight = 40f;
                            Rect rectItem = list.GetRect(rowHeight);
                            Rect itemRect = new Rect(rectItem.xMin, rectItem.yMin, rectItem.width, 40f);
                            Widgets.DrawHighlightIfMouseover(itemRect);
                            RecipeInfo recipeInfo = recipeInfos[i];
                            float textHeight = Text.CalcHeight(recipeInfo.Recipe.LabelCap, itemRect.width);
                            GUI.Label(new Rect(itemRect.xMin + 8f, itemRect.yMin + 9f, itemRect.width, itemRect.height), recipeInfo.Recipe.LabelCap.Truncate(200));
                            Rect tableIconRect = new Rect(itemRect.xMax - 30f - 8f, itemRect.yMin + 5f, 30f, 30f);
                            Widgets.DefIcon(tableIconRect, recipeInfo.Recipe.UIIconThing);
                            if (Widgets.ButtonInvisible(itemRect))
                            {
                                curRecipe = recipeInfo.Recipe.defName;
                            }
                            if (curRecipe == recipeInfo.Recipe.defName) {
                                Widgets.DrawHighlight(itemRect);
                            }
                            itemRect.height += 40f;
                        }
                        scrollSumRecipes = recipeInfos.Count * 40f;
                        list.End();
                        Widgets.EndScrollView();
                        break;
                    case 1:
                        Widgets.BeginScrollView(scrollRect, ref scrollPositionRecipes, scrollViewRect);

                        list = new Listing_Standard();
                        list.Begin(scrollViewRect);
                        if (bills.Count > 0)
                        {
                            for (int i = 0; i < bills.Count; i++)
                            {
                                Bill bill = bills[i];
                                float rowHeight = 40f;
                                Rect rectItem = list.GetRect(rowHeight);
                                Rect itemRect = new Rect(rectItem.xMin, rectItem.yMin, rectItem.width, 40f);
                                string billText = $"#{bill.billStack.IndexOf(bill) + 1} | {bill.LabelCap.Truncate(200)}";
                                GUI.Label(new Rect(rectItem.xMin + 38f, rectItem.yMin + 9f, rectItem.width, rectItem.height), billText);
                                Rect arrowUpRect = new Rect(itemRect.xMin, itemRect.yMin + 5f, 30f, 15f);
                                Rect arrowDownRect = new Rect(itemRect.xMin, itemRect.yMin + 20f, 30f, 15f);

                                if (bill.billStack.IndexOf(bill) > 0)
                                {
                                    if (Widgets.ButtonImage(arrowUpRect, icnArrowUpBtn))
                                    {
                                        bill.billStack.Reorder(bill, -1);
                                        SoundDefOf.Tick_High.PlayOneShotOnCamera();
                                    }
                                    if (Mouse.IsOver(arrowUpRect))
                                    {
                                        TooltipHandler.TipRegionByKey(arrowUpRect, "ReorderBillUpTip".Translate());
                                    }
                                } 
                                else 
                                {   
                                    GUI.color = new Color(1f, 1f, 1f, 0.5f);
                                    Widgets.DrawTextureFitted(arrowUpRect, icnArrowUpBtn, 1f);
                                    GUI.color = Color.white;
                                }
                                
                                if (bill.billStack.IndexOf(bill) < bill.billStack.Count - 1)
                                {
                                    if (Widgets.ButtonImage(arrowDownRect, icnArrowDownBtn))
                                    {
                                        bill.billStack.Reorder(bill, 1);
                                        SoundDefOf.Tick_Low.PlayOneShotOnCamera();
                                    }
                                    if (Mouse.IsOver(arrowDownRect))
                                    {
                                        TooltipHandler.TipRegionByKey(arrowDownRect, "ReorderBillDownTip".Translate());
                                    }
                                } 
                                else
                                {
                                    GUI.color = new Color(1f, 1f, 1f, 0.5f);
                                    Widgets.DrawTextureFitted(arrowDownRect, icnArrowDownBtn, 1f);
                                    GUI.color = Color.white;
                                }
                                itemRect.height += 40f;
                            }
                        }
                        scrollSumRecipes = bills.Count * 40f;
                        list.End();
                        Widgets.EndScrollView();
                        break;
                }
            } 
        }
        private void DrawTablesContent(Rect rect)
        {
            Rect scrollRect = new Rect(rect.xMin, rect.yMin, rect.width, rect.height);
            Rect scrollViewRect = new Rect(0f, 0f, rect.width - 16f, scrollSumTables);
            if (scrollSumTables < scrollRect.height)
            {
                scrollViewRect = new Rect(0f, 0f, rect.width, scrollSumTables);
            }
            
            
            if (tabs.Count() == 0)
            {
                Text.Anchor = TextAnchor.MiddleCenter;
                Widgets.Label(new Rect((rect.xMax - 220f) / 2f, (rect.yMax - Text.CalcHeight("N1.blockTablesEmpty".Translate(), 220f)) / 2f, 220f, Text.CalcHeight("N1.blockTablesEmpty".Translate(), 220f)), "N1.blockTablesEmpty".Translate());
                Text.Anchor = TextAnchor.UpperLeft;
            }
            else
            {
                Widgets.BeginScrollView(scrollRect, ref scrollPositionTables, scrollViewRect);
                Listing_Standard list = new Listing_Standard();
                list.Begin(scrollViewRect);
                foreach (int key in tabs.Keys)
                {
                    Building_WorkTable selectedTable = workTables.Find(t => t.thingIDNumber == key);

                    string value = tabs[key];
                    float rowHeight = 40f;
                    Rect rectItem = list.GetRect(rowHeight);
                    Rect itemRect = new Rect(rectItem.xMin, rectItem.yMin, rectItem.width, 40f);
                    Widgets.DrawHighlightIfMouseover(itemRect);
                    float textHeight = Text.CalcHeight(value, itemRect.width);
                    GUI.Label(new Rect(itemRect.xMin + 8f, itemRect.yMin + 9f, itemRect.width, itemRect.height), value);
                    Rect tableIconRect = new Rect(itemRect.xMax - 30f - 8f, itemRect.yMin + 5f, 30f, 30f);
                    if (Widgets.ButtonInvisible(tableIconRect))
                    {
                        Thing targetThing = Find.CurrentMap.listerThings.AllThings.FirstOrDefault(t => t.thingIDNumber == key);
                        
                        if (targetThing != null)
                        {
                            Find.Selector.ClearSelection();
                            Find.Selector.Select(targetThing);
                            Find.WindowStack.TryRemove(this);
                            CameraJumper.TryJumpAndSelect(targetThing);
                        }
                    }
                    Widgets.DrawTextureFitted(tableIconRect, selectedTable.def.uiIcon, 1f);
                    if (BillUtility.Clipboard != null)
                    {
                        Rect clipboardIconRect = new Rect(tableIconRect.xMax - 60f, tableIconRect.yMin + 2.5f, 30f, 30f);
                        if (selectedTable.def.AllRecipes.FirstOrDefault(recipe => recipe.defName == BillUtility.Clipboard.recipe.defName) == null || !BillUtility.Clipboard.recipe.AvailableNow || !BillUtility.Clipboard.recipe.AvailableOnNow(selectedTable))
                        {
                            GUI.color = Color.gray;
                            Widgets.DrawTextureFitted(clipboardIconRect, TexButton.Paste, 1f);
                            GUI.color = Color.white;
                            if (Mouse.IsOver(clipboardIconRect))
                            {
                                TooltipHandler.TipRegion(clipboardIconRect, "ClipboardBillNotAvailableHere".Translate() + ": " + BillUtility.Clipboard.LabelCap);
                            }
                        }
                        else
                        {
                            if (Widgets.ButtonImageFitted(clipboardIconRect, TexButton.Paste, Color.white))
                            {
                                Bill bill = BillUtility.Clipboard.Clone();
                                bill.InitializeAfterClone();
                                selectedTable.billStack.AddBill(bill);
                                isClipboardAdd = true;
                                SoundDefOf.Tick_Low.PlayOneShotOnCamera();
                            }
                            if (Mouse.IsOver(clipboardIconRect))
                            {
                                TooltipHandler.TipRegion(clipboardIconRect, "PasteBillTip".Translate() + ": " + BillUtility.Clipboard.LabelCap);
                            }
                        }
                    }
                    if (curTab == key) {
                        Widgets.DrawHighlight(itemRect);
                    }
                    if (Widgets.ButtonInvisible(itemRect))
                    {
                        curRecipe = null;
                        curTab = key;
                    }
                }
                scrollSumTables = tabs.Count * 40f;
                list.End();
                Widgets.EndScrollView();
            }
        }
        private void DrawBlockHeaderTabs(Rect rect, string text1, string text2) {
            Text.Font = GameFont.Tiny;

            Rect blockRect = rect;

            Rect btnRect = new Rect(blockRect.xMin + 8f, blockRect.yMin + 7f, blockRect.width / 2f - 8f, 16f);
            Widgets.Label(new Rect(btnRect.xMin + 4f, btnRect.yMin, btnRect.width, btnRect.height), text1.Translate());
            Widgets.DrawTextureFitted(new Rect(btnRect.xMax - 16f, btnRect.yMin, 16f, 16f), icnTabRecipes, 1f);
            Widgets.DrawHighlightIfMouseover(btnRect);

            Rect btnSecondRect = new Rect(blockRect.xMax - btnRect.width - 8f, blockRect.yMin + 7f, blockRect.width / 2f - 8f, 16f);
            Widgets.Label(new Rect(btnSecondRect.xMin + 4f, btnSecondRect.yMin, btnSecondRect.width, btnSecondRect.height), text2.Translate());
            Widgets.DrawTextureFitted(new Rect(btnSecondRect.xMax - 16f, btnSecondRect.yMin, 16f, 16f), icnTabQueues, 1f);
            Widgets.DrawHighlightIfMouseover(btnSecondRect);

            Widgets.DrawBoxSolid(new Rect(blockRect.xMin, blockRect.yMax, blockRect.width, 1f), new Color(1f, 1f, 1f, 0.1f));

            Text.Font = GameFont.Small;

            if (Widgets.ButtonInvisible(btnRect))
            {
                curRecipe = null;
                curTabRecipe = 0;
            }
            if (Widgets.ButtonInvisible(btnSecondRect))
            {
                curRecipe = null;
                curTabRecipe = 1;
            }

            if (curTabRecipe == 0)
            {
                Widgets.DrawHighlight(btnRect);
            }
            else if (curTabRecipe == 1)
            {
                Widgets.DrawHighlight(btnSecondRect);
            }
        }
        private void DrawBlockHeader(Rect rect, string text, Texture2D texture) {
            Text.Font = GameFont.Tiny;

            Rect blockRect = rect;
            Widgets.Label(new Rect(blockRect.xMin + 8f, blockRect.yMin + 6f, blockRect.width, blockRect.height), text.Translate());
            Widgets.DrawTextureFitted(new Rect(blockRect.xMax - 24f, blockRect.yMin + 7f, 16f, 16f), texture, 1f);
            Widgets.DrawBoxSolid(new Rect(blockRect.xMin, blockRect.yMax, blockRect.width, 1f), new Color(1f, 1f, 1f, 0.1f));

            Text.Font = GameFont.Small;
        }
        public override void WindowUpdate()
        {
            base.WindowUpdate();
            if (Event.current.type == EventType.ScrollWheel)
            {
                switch (curRect)
                {
                    case 0:
                        break;
                    case 1:
                        scrollPositionTables.y += Event.current.delta.y;
                        Event.current.Use();
                        break;
                    case 2:
                        scrollPositionRecipes.y += Event.current.delta.y;
                        Event.current.Use();
                        break;
                    case 3:
                        scrollPositionBills.y += Event.current.delta.y;
                        Event.current.Use();
                        break;
                    default:
                        break;
                }
            }
            
        }
    }
}
