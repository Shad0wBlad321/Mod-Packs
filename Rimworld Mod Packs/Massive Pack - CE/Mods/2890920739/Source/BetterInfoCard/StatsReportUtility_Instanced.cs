using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
using Verse.Sound;
using RimWorld;
using System.Reflection;

namespace BetterInfoCard
{
    class StatsReportUtility_Instanced
    {
        private StatDrawEntry selectedEntry;
        private StatDrawEntry mousedOverEntry;
        private Vector2 scrollPosition;
        private ScrollPositioner scrollPositioner = new ScrollPositioner();
        private Vector2 scrollPositionRightPanel;
        private QuickSearchWidget quickSearchWidget = new QuickSearchWidget();
        private float listHeight;
        private float rightPanelHeight;
        private List<StatDrawEntry> cachedDrawEntries = new List<StatDrawEntry>();
        private List<string> cachedEntryValues = new List<string>();

        public int SelectedStatIndex => cachedDrawEntries.NullOrEmpty<StatDrawEntry>() || selectedEntry == null ? -1 : cachedDrawEntries.IndexOf(selectedEntry);

        public QuickSearchWidget QuickSearchWidget => quickSearchWidget;

        public void Reset()
        {
            scrollPosition = new Vector2();
            scrollPositionRightPanel = new Vector2();
            selectedEntry = (StatDrawEntry)null;
            scrollPositioner.Arm(false);
            mousedOverEntry = (StatDrawEntry)null;
            cachedDrawEntries.Clear();
            cachedEntryValues.Clear();
            quickSearchWidget.Reset();
            PermitsCardUtility.selectedPermit = (RoyalTitlePermitDef)null;
            PermitsCardUtility.selectedFaction = ((ModLister.RoyaltyInstalled && Current.ProgramState == ProgramState.Playing) ? Faction.OfEmpire : null);
        }

        public void DrawStatsReport(Rect rect, Def def, ThingDef stuff)
        {
            if (cachedDrawEntries.NullOrEmpty<StatDrawEntry>())
            {
                StatRequest req = def is BuildableDef def1 ? StatRequest.For(def1, stuff) : StatRequest.ForEmpty();
                cachedDrawEntries.AddRange(def.SpecialDisplayStats(req));
                cachedDrawEntries.AddRange(StatsToDraw(def, stuff).Where<StatDrawEntry>((Func<StatDrawEntry, bool>)(r => r.ShouldDisplay)));
                FinalizeCachedDrawEntries((IEnumerable<StatDrawEntry>)cachedDrawEntries);
            }
            DrawStatsWorker(rect, (Thing)null, (WorldObject)null);
        }

        public void DrawStatsReport(Rect rect, AbilityDef def)
        {
            if (cachedDrawEntries.NullOrEmpty<StatDrawEntry>())
            {
                StatRequest req = StatRequest.ForEmpty();
                cachedDrawEntries.AddRange(def.SpecialDisplayStats(req));
                cachedDrawEntries.AddRange(StatsToDraw(def).Where<StatDrawEntry>((Func<StatDrawEntry, bool>)(r => r.ShouldDisplay)));
                FinalizeCachedDrawEntries((IEnumerable<StatDrawEntry>)cachedDrawEntries);
            }
            DrawStatsWorker(rect, (Thing)null, (WorldObject)null);
        }

        public void DrawStatsReport(Rect rect, Thing thing)
        {
            if (cachedDrawEntries.NullOrEmpty<StatDrawEntry>())
            {
                cachedDrawEntries.AddRange(thing.def.SpecialDisplayStats(StatRequest.For(thing)));
                cachedDrawEntries.AddRange(StatsToDraw(thing).Where<StatDrawEntry>((Func<StatDrawEntry, bool>)(r => r.ShouldDisplay)));
                cachedDrawEntries.RemoveAll((Predicate<StatDrawEntry>)(de => de.stat != null && !de.stat.showNonAbstract));
                FinalizeCachedDrawEntries((IEnumerable<StatDrawEntry>)cachedDrawEntries);
            }
            DrawStatsWorker(rect, thing, (WorldObject)null);
        }

        public void DrawStatsReport(Rect rect, Hediff hediff)
        {
            if (cachedDrawEntries.NullOrEmpty<StatDrawEntry>())
            {
                cachedDrawEntries.AddRange(hediff.SpecialDisplayStats(StatRequest.ForEmpty()));
                cachedDrawEntries.AddRange(StatsToDraw((Def)hediff.def, (ThingDef)null).Where<StatDrawEntry>((Func<StatDrawEntry, bool>)(r => r.ShouldDisplay)));
                FinalizeCachedDrawEntries((IEnumerable<StatDrawEntry>)cachedDrawEntries);
            }
            DrawStatsWorker(rect, (Thing)null, (WorldObject)null);
        }

        public void DrawStatsReport(Rect rect, WorldObject worldObject)
        {
            if (cachedDrawEntries.NullOrEmpty<StatDrawEntry>())
            {
                cachedDrawEntries.AddRange(worldObject.def.SpecialDisplayStats(StatRequest.ForEmpty()));
                cachedDrawEntries.AddRange(StatsToDraw(worldObject).Where<StatDrawEntry>((Func<StatDrawEntry, bool>)(r => r.ShouldDisplay)));
                cachedDrawEntries.RemoveAll((Predicate<StatDrawEntry>)(de => de.stat != null && !de.stat.showNonAbstract));
                FinalizeCachedDrawEntries((IEnumerable<StatDrawEntry>)cachedDrawEntries);
            }
            DrawStatsWorker(rect, (Thing)null, worldObject);
        }

        public void DrawStatsReport(Rect rect, RoyalTitleDef title, Faction faction, Pawn pawn = null)
        {
            if (cachedDrawEntries.NullOrEmpty<StatDrawEntry>())
            {
                cachedDrawEntries.AddRange(title.SpecialDisplayStats(StatRequest.For(title, faction, pawn)));
                cachedDrawEntries.AddRange(StatsToDraw(title, faction).Where<StatDrawEntry>((Func<StatDrawEntry, bool>)(r => r.ShouldDisplay)));
                FinalizeCachedDrawEntries((IEnumerable<StatDrawEntry>)cachedDrawEntries);
            }
            DrawStatsWorker(rect, (Thing)null, (WorldObject)null);
        }

        public void DrawStatsReport(Rect rect, Faction faction)
        {
            if (cachedDrawEntries.NullOrEmpty<StatDrawEntry>())
            {
                StatRequest req = StatRequest.ForEmpty();
                cachedDrawEntries.AddRange(faction.def.SpecialDisplayStats(req));
                cachedDrawEntries.AddRange(StatsToDraw(faction).Where<StatDrawEntry>((Func<StatDrawEntry, bool>)(r => r.ShouldDisplay)));
                FinalizeCachedDrawEntries((IEnumerable<StatDrawEntry>)cachedDrawEntries);
            }
            DrawStatsWorker(rect, (Thing)null, (WorldObject)null);
        }

        private IEnumerable<StatDrawEntry> StatsToDraw(
          Def def,
          ThingDef stuff)
        {
            yield return DescriptionEntry(def);
            if (def is BuildableDef eDef)
            {
                StatRequest statRequest = StatRequest.For(eDef, stuff);
                foreach (StatDef stat in DefDatabase<StatDef>.AllDefs.Where<StatDef>((Func<StatDef, bool>)(st => st.Worker.ShouldShowFor(statRequest))))
                    yield return new StatDrawEntry(stat.category, stat, eDef.GetStatValueAbstract(stat, stuff), StatRequest.For(eDef, stuff));
            }
            if (def is ThingDef stuffDef && stuffDef.IsStuff)
            {
                foreach (StatDrawEntry stuffStat in StuffStats(stuffDef))
                    yield return stuffStat;
            }
        }

        private IEnumerable<StatDrawEntry> StatsToDraw(
          RoyalTitleDef title,
          Faction faction)
        {
            yield return DescriptionEntry(title, faction);
        }

        private IEnumerable<StatDrawEntry> StatsToDraw(Faction faction)
        {
            yield return DescriptionEntry(faction);
        }

        private IEnumerable<StatDrawEntry> StatsToDraw(AbilityDef def)
        {
            yield return DescriptionEntry((Def)def);
            StatRequest statRequest = StatRequest.For(def);
            foreach (StatDef stat in DefDatabase<StatDef>.AllDefs.Where<StatDef>((Func<StatDef, bool>)(st => st.Worker.ShouldShowFor(statRequest))))
                yield return new StatDrawEntry(stat.category, stat, def.GetStatValueAbstract(stat), StatRequest.For(def));
        }

        private IEnumerable<StatDrawEntry> StatsToDraw(Thing thing)
        {
            yield return DescriptionEntry(thing);
            StatDrawEntry statDrawEntry = QualityEntry(thing);
            if (statDrawEntry != null)
                yield return statDrawEntry;
            foreach (StatDef stat in DefDatabase<StatDef>.AllDefs.Where<StatDef>((Func<StatDef, bool>)(st => st.Worker.ShouldShowFor(StatRequest.For(thing)))))
            {
                if (!stat.Worker.IsDisabledFor(thing))
                {
                    float statValue = thing.GetStatValue(stat);
                    if (stat.showOnDefaultValue || (double)statValue != (double)stat.defaultBaseValue)
                        yield return new StatDrawEntry(stat.category, stat, statValue, StatRequest.For(thing));
                }
                else
                    yield return new StatDrawEntry(stat.category, stat);
            }
            if (thing.def.useHitPoints)
                yield return new StatDrawEntry(StatCategoryDefOf.BasicsImportant, (string)"HitPointsBasic".Translate().CapitalizeFirst(), thing.HitPoints.ToString() + " / " + thing.MaxHitPoints.ToString(), (string)"Stat_HitPoints_Desc".Translate(), 99998);
            foreach (StatDrawEntry specialDisplayStat in thing.SpecialDisplayStats())
                yield return specialDisplayStat;
            if (thing.def.IsStuff)
            {
                foreach (StatDrawEntry stuffStat in StuffStats(thing.def))
                    yield return stuffStat;
            }
        }

        private IEnumerable<StatDrawEntry> StatsToDraw(
          WorldObject worldObject)
        {
            yield return DescriptionEntry(worldObject);
            foreach (StatDrawEntry specialDisplayStat in worldObject.SpecialDisplayStats)
                yield return specialDisplayStat;
        }

        private IEnumerable<StatDrawEntry> StuffStats(ThingDef stuffDef)
        {
            int i;
            if (!stuffDef.stuffProps.statFactors.NullOrEmpty<StatModifier>())
            {
                for (i = 0; i < stuffDef.stuffProps.statFactors.Count; ++i)
                    yield return new StatDrawEntry(StatCategoryDefOf.StuffStatFactors, stuffDef.stuffProps.statFactors[i].stat, stuffDef.stuffProps.statFactors[i].value, StatRequest.ForEmpty(), ToStringNumberSense.Factor);
            }
            if (!stuffDef.stuffProps.statOffsets.NullOrEmpty<StatModifier>())
            {
                for (i = 0; i < stuffDef.stuffProps.statOffsets.Count; ++i)
                    yield return new StatDrawEntry(StatCategoryDefOf.StuffStatOffsets, stuffDef.stuffProps.statOffsets[i].stat, stuffDef.stuffProps.statOffsets[i].value, StatRequest.ForEmpty(), ToStringNumberSense.Offset);
            }
        }

        private void FinalizeCachedDrawEntries(IEnumerable<StatDrawEntry> original)
        {
            cachedDrawEntries = original.OrderBy<StatDrawEntry, int>((Func<StatDrawEntry, int>)(sd => sd.category.displayOrder)).ThenByDescending<StatDrawEntry, int>((Func<StatDrawEntry, int>)(sd => sd.DisplayPriorityWithinCategory)).ThenBy<StatDrawEntry, string>((Func<StatDrawEntry, string>)(sd => sd.LabelCap)).ToList<StatDrawEntry>();
            quickSearchWidget.noResultsMatched = !cachedDrawEntries.Any<StatDrawEntry>();
            foreach (StatDrawEntry cachedDrawEntry in cachedDrawEntries)
                cachedEntryValues.Add(cachedDrawEntry.ValueString);
            if (selectedEntry != null)
                selectedEntry = cachedDrawEntries.FirstOrDefault<StatDrawEntry>((Predicate<StatDrawEntry>)(e => e.Same(selectedEntry)));
            //if (!quickSearchWidget.filter.Active)
            //    return;
            //foreach (StatDrawEntry cachedDrawEntry in cachedDrawEntries)
            //{
            //    if (Matches(cachedDrawEntry))
            //    {
            //        selectedEntry = cachedDrawEntry;
            //        scrollPositioner.Arm();
            //        break;
            //    }
            //}
        }

        private bool Matches(StatDrawEntry sd) => quickSearchWidget.filter.Matches(sd.LabelCap);

        private StatDrawEntry DescriptionEntry(Def def) => new StatDrawEntry(StatCategoryDefOf.BasicsImportant, (string)"Description".Translate(), "", def.description, 99999, hyperlinks: Dialog_InfoCard.DefsToHyperlinks((IEnumerable<DefHyperlink>)def.descriptionHyperlinks));

        private StatDrawEntry DescriptionEntry(Faction faction) => new StatDrawEntry(StatCategoryDefOf.BasicsImportant, (string)"Description".Translate(), "", faction.GetReportText, 99999, hyperlinks: Dialog_InfoCard.DefsToHyperlinks((IEnumerable<DefHyperlink>)faction.def.descriptionHyperlinks));

        private StatDrawEntry DescriptionEntry(RoyalTitleDef title, Faction faction) => new StatDrawEntry(StatCategoryDefOf.BasicsImportant, (string)"Description".Translate(), "", title.GetReportText(faction), 99999, hyperlinks: Dialog_InfoCard.TitleDefsToHyperlinks(title.GetHyperlinks(faction)));

        private StatDrawEntry DescriptionEntry(Thing thing) => new StatDrawEntry(StatCategoryDefOf.BasicsImportant, (string)"Description".Translate(), "", thing.DescriptionFlavor, 99999, hyperlinks: Dialog_InfoCard.DefsToHyperlinks((IEnumerable<DefHyperlink>)thing.def.descriptionHyperlinks));

        private StatDrawEntry DescriptionEntry(WorldObject worldObject) => new StatDrawEntry(StatCategoryDefOf.BasicsImportant, (string)"Description".Translate(), "", worldObject.GetDescription(), 99999);

        private StatDrawEntry QualityEntry(Thing t)
        {
            QualityCategory qc;
            return !t.TryGetQuality(out qc) ? (StatDrawEntry)null : new StatDrawEntry(StatCategoryDefOf.BasicsImportant, (string)"Quality".Translate(), qc.GetLabel().CapitalizeFirst(), (string)"QualityDescription".Translate(), 99999);
        }

        public void SelectEntry(int index)
        {
            if (index < 0 || index > cachedDrawEntries.Count)
                return;
            SelectEntry(cachedDrawEntries[index]);
        }

        public void SelectEntry(StatDef stat, bool playSound = false)
        {
            foreach (StatDrawEntry cachedDrawEntry in cachedDrawEntries)
            {
                if (cachedDrawEntry.stat == stat)
                {
                    SelectEntry(cachedDrawEntry, playSound);
                    return;
                }
            }
            Messages.Message((string)"MessageCannotSelectInvisibleStat".Translate((NamedArgument)(Def)stat), MessageTypeDefOf.RejectInput, false);
        }

        private void SelectEntry(StatDrawEntry rec, bool playSound = true)
        {
            selectedEntry = rec;
            scrollPositioner.Arm();

            SetStatsReportDirty();


            if (!playSound)
                return;
            SoundDefOf.Tick_High.PlayOneShotOnCamera();
        }

        private void DrawStatsWorker(
          Rect rect,
          Thing optionalThing,
          WorldObject optionalWorldObject)
        {
            Rect rect1 = new Rect(rect);
            if(!collapse) rect1.width *= 0.5f;

            scrollPositioner.ClearInterestRects();
            Text.Font = GameFont.Small;
            Rect viewRect1 = new Rect(0.0f, 0.0f, rect1.width - 16f, listHeight);
            Widgets.BeginScrollView(rect1, ref scrollPosition, viewRect1);
            float curY = 0.0f;
            string str = (string)null;
            mousedOverEntry = (StatDrawEntry)null;
            for (int index = 0; index < cachedDrawEntries.Count; ++index)
            {
                StatDrawEntry ent = cachedDrawEntries[index];
                if ((string)ent.category.LabelCap != str)
                {
                    Widgets.ListSeparator(ref curY, viewRect1.width, (string)ent.category.LabelCap);
                    str = (string)ent.category.LabelCap;
                }
                bool highlightLabel = false;
                bool lowlightLabel = false;
                bool selected = selectedEntry == ent;
                bool flag = false;
                GUI.color = Color.white;
                if (quickSearchWidget.filter.Active)
                {
                    if (Matches(ent))
                    {
                        highlightLabel = true;
                        flag = true;
                    }
                    else
                    {
                        lowlightLabel = true;
                        if (BetterInfoCard.singleton.settings.hideFilteredStat)
                        {
                            continue;
                        }
                    }
                        
                }
                Rect rect3 = new Rect(8f, curY, viewRect1.width - 8f, 30f);

                System.Action onClick = () =>
                {
                    SelectEntry(ent);
                    if (selected)
                    {
                        if (compareFocusEntry.HasValue && compareFocusEntry.Value.Item1 == this && compareFocusEntry.Value.Item2.Same(ent))
                        {
                            UnFocusCompareEntry();
                        }
                        else
                        {
                            FocusCompareEntry(this, ent);
                        }
                    }
                };


                curY += ent.Draw(rect3.x, rect3.y, rect3.width, selected, highlightLabel, lowlightLabel, onClick,
                    (Action)(() => { mousedOverEntry = ent; }  ), scrollPosition, rect1, cachedEntryValues[index]);

                
                rect3.yMax = curY;
                if (selected | flag)
                    scrollPositioner.RegisterInterestRect(rect3);
            }
            listHeight = curY + 100f;
            Widgets.EndScrollView();


            Rect rect2 = new Rect(rect) { x = rect1.xMax };
            rect2.width = rect.xMax - rect2.x;

            scrollPositioner.ScrollVertically(ref scrollPosition, rect1.size);
            if (!collapse)
            {
                Rect outRect = rect2.ContractedBy(10f);
                StatDrawEntry statDrawEntry = selectedEntry ?? mousedOverEntry ?? cachedDrawEntries.FirstOrDefault<StatDrawEntry>();
                if (statDrawEntry == null)
                    return;
                Rect viewRect2 = new Rect(0.0f, 0.0f, outRect.width - 16f, rightPanelHeight);
                StatRequest statRequest = !statDrawEntry.hasOptionalReq ? (optionalThing == null ? StatRequest.ForEmpty() : StatRequest.For(optionalThing)) : statDrawEntry.optionalReq;
                string explanationText = statDrawEntry.GetExplanationText(statRequest);
                float num1 = 0.0f;
                Widgets.BeginScrollView(outRect, ref scrollPositionRightPanel, viewRect2);
                Rect rect4 = viewRect2;
                rect4.width -= 4f;
                Widgets.Label(rect4, explanationText);
                float num2 = Text.CalcHeight(explanationText, rect4.width) + 10f;
                float num3 = num1 + num2;
                IEnumerable<Dialog_InfoCard.Hyperlink> hyperlinks = statDrawEntry.GetHyperlinks(statRequest);
                if (hyperlinks != null)
                {
                    Rect rect3 = new Rect(rect4.x, rect4.y + num2, rect4.width, rect4.height - num2);
                    Color color = GUI.color;
                    GUI.color = Widgets.NormalOptionColor;
                    foreach (Dialog_InfoCard.Hyperlink hyperlink in hyperlinks)
                    {
                        float height = Text.CalcHeight(hyperlink.Label, rect3.width);
                        Widgets.HyperlinkWithIcon(new Rect(rect3.x, rect3.y, rect3.width, height), hyperlink, (string)"ViewHyperlink".Translate((NamedArgument)hyperlink.Label));
                        rect3.y += height;
                        rect3.height -= height;
                        num3 += height;
                    }
                    GUI.color = color;
                }
                rightPanelHeight = num3;
                Widgets.EndScrollView();
            }



            if(dirtyFlag)
            {
                dirtyFlag = false;
                cachedDrawEntries.Clear();
                cachedEntryValues.Clear();
            }

        }

        public void Notify_QuickSearchChanged()
        {
            cachedDrawEntries.Clear();
            cachedEntryValues.Clear();
        }



        // Append
        public bool collapse;
        public bool minify;

        public static (StatsReportUtility_Instanced,StatDrawEntry,string)? compareFocusEntry;
        private bool dirtyFlag;


        private static void FocusCompareEntry(StatsReportUtility_Instanced uti,StatDrawEntry entry)
        {
            compareFocusEntry = (uti,entry, entry.Get_labelInt());
            foreach (var kp in Dialog_InfoCard_Patch.infoCardStatsDic.Values)
            {
                foreach (var dent in kp.cachedDrawEntries)
                {
                    if(dent.stat==entry.stat && kp!=uti && dent.Get_labelInt() == entry.Get_labelInt())
                    {
                        //dent.Set_labelInt(entry.Get_labelInt());
                        kp.SelectEntry(dent);
                    }
                }
            }
        }


        public StatDrawEntry GetStatEntry(StatDef statDef,string labelInt)
        {
            return cachedDrawEntries.Find(t => t.stat == statDef && t.Get_labelInt() == labelInt);
        }

        private static void UnFocusCompareEntry()
        {
            compareFocusEntry = null;
        }

        private void SetStatsReportDirty()
        {
            dirtyFlag = true;
            
        }

    }
}
