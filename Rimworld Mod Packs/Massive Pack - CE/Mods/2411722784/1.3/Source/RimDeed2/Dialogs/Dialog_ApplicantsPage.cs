using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;
using Verse.AI;
using Verse.Sound;
using static Verse.Widgets;

namespace Rimdeed
{
	public class Dialog_ApplicantsPage : Dialog_NodeTree
	{
		private Pawn curPawn;

		private static readonly Vector2 PawnPortraitSize = new Vector2(92f, 128f);

		private static readonly Vector2 PawnSelectorPortraitSize = new Vector2(70f, 110f);
		public override float Margin => 0f;
        public override Vector2 InitialSize => new Vector2(610f, Mathf.Min(620, UI.screenHeight));

		private Applicants applicants;
		private DiaOption acceptOption;
		private DiaOption cancelOption;
		private DiaOption exitOption;
		private DiaOption xOption;
		public Dialog_ApplicantsPage(DiaNode startNode, bool radioMode, Applicants applicants)
			: base(startNode, radioMode)
		{
			this.applicants = applicants;
			if (applicants.applicants.Any())
            {
				this.curPawn = applicants.applicants[0];
			}

			acceptOption = new DiaOption("RimDeed.Accept".Translate());
			acceptOption.dialog = this;
			acceptOption.action = delegate ()
			{
				RimdeedManager.Instance.RegisterNewRecruiter(curPawn, Find.CurrentMap, applicants.isGold);
				applicants.applicants.Remove(curPawn);
				if (applicants.applicants.Any())
                {
					curPawn = applicants.applicants[0];
				}
				SoundDefOf.ExecuteTrade.PlayOneShotOnCamera();
			};

			cancelOption = new DiaOption("RimDeed.Cancel".Translate());
			cancelOption.resolveTree = true;
			cancelOption.dialog = this;
			cancelOption.action = delegate ()
			{
				var letter = RimdeedManager.Instance.lettersWithApplicants.FirstOrDefault(x => x.Value == applicants).Key;
				Find.LetterStack.RemoveLetter(letter);
			};
			exitOption = new DiaOption("RimDeed.Exit".Translate());
			exitOption.resolveTree = true;
			exitOption.dialog = this;
			exitOption.action = delegate ()
			{
				if (!applicants.applicants.Any())
                {
					var letter = RimdeedManager.Instance.lettersWithApplicants.FirstOrDefault(x => x.Value == applicants).Key;
					Find.LetterStack.RemoveLetter(letter);
				}
			};
			xOption = new DiaOption("X");
			xOption.resolveTree = true;
			xOption.dialog = this;
			xOption.action = delegate ()
			{
				if (!applicants.applicants.Any())
				{
					var letter = RimdeedManager.Instance.lettersWithApplicants.FirstOrDefault(x => x.Value == applicants).Key;
					Find.LetterStack.RemoveLetter(letter);
				}
			};
		}

		public override void PostOpen()
		{
			base.PostOpen();
			TutorSystem.Notify_Event("PageStart-ConfigureStartingPawns");
		}

		public static readonly Color StackElementBackground = new Color(1f, 1f, 1f, 0.1f);

		[TweakValue("0Rimdeed", 0, 1000)] public static float logoCornerPosX = 20;
		[TweakValue("0Rimdeed", 0, 1000)] public static float logoCornerPosY = 10;
		[TweakValue("0Rimdeed", 0, 1000)] public static float logoCornerWidth = 200;
		[TweakValue("0Rimdeed", 0, 1000)] public static float logoCornerHeight = 50;

		[TweakValue("0Rimdeed", -100, 1000)] public static float TraitBoxPosYOffset = 15;
		[TweakValue("0Rimdeed", -100, 1000)] public static float TraitBoxWidth = 225;
		[TweakValue("0Rimdeed", -100, 1000)] public static float TraitBoxHeight = 160;

		[TweakValue("0Rimdeed", -100, 1000)] public static float IncapableOfPosXOffset = 15;
		[TweakValue("0Rimdeed", -100, 1000)] public static float IncapableOfWidth = 175;

		[TweakValue("0Rimdeed", -100, 1000)] public static float PawnListBoxWidth = 150;
		[TweakValue("0Rimdeed", -100, 1000)] public static float PawnListBoxHeight = 60;
		public override void DoWindowContents(Rect rect)
		{
			Text.Font = GameFont.Small;
			Rect xRect = new Rect(rect.xMax - 20, rect.y + 5, 20, 20);
			GUIHelper.OptOnGUI(xOption, xRect);

			var logo = new Rect(logoCornerPosX, logoCornerPosY, logoCornerWidth, logoCornerHeight);
			GUI.DrawTexture(logo, Textures.Rimdeed_corner);
			if (this.applicants.applicants.Any())
            {
				var pawnListBox = new Rect(rect.x + 10, logo.yMax + 15, PawnListBoxWidth, PawnListBoxHeight);
				DrawPawnList(pawnListBox);

				var pawnName = new Rect(logo.xMax + 30, logo.y + 10, 334, 40);
				DrawPawnLabel(pawnName);

				Text.Font = GameFont.Small;
				Text.Anchor = TextAnchor.UpperLeft;

				var pawnInfoBox = new Rect(pawnListBox.xMax + 10, pawnName.yMax + 15, 300, 120);
				Widgets.DrawMenuSection(pawnInfoBox);

				var pawnGenderAgeInfo = new Rect(pawnInfoBox.x, pawnInfoBox.y, 130, 40);
				Widgets.Label(pawnGenderAgeInfo.ContractedBy(5f), CurPawnInfo());

				if (curPawn.Faction != null)
				{
					Rect rect23 = new Rect(pawnGenderAgeInfo.xMax + 10, pawnGenderAgeInfo.y + 5, 150, 22);
					Color color7 = GUI.color;
					GUI.color = StackElementBackground;
					GUI.DrawTexture(rect23, BaseContent.WhiteTex);
					GUI.color = color7;
					Widgets.DrawHighlightIfMouseover(rect23);
					Rect rect24 = new Rect(rect23.x, rect23.y, rect23.width, rect23.height);
					Rect position4 = new Rect(rect23.x + 1f, rect23.y + 1f, 20f, 20f);
					GUI.color = curPawn.Faction.Color;
					GUI.DrawTexture(position4, curPawn.Faction.def.FactionIcon);
					GUI.color = color7;
					Widgets.Label(new Rect(rect24.x + rect24.height + 5f, rect24.y, rect24.width - 10f, rect24.height), curPawn.Faction.Name);
					if (Mouse.IsOver(rect23))
					{
						TaggedString taggedString2 = "Faction".Translate() + "\n\n" + "FactionDesc".Translate(curPawn.Named("PAWN")) + "\n\n" + "ClickToViewFactions".Translate();
						TipSignal tip6 = new TipSignal(taggedString2, curPawn.Faction.loadID * 37);
						TooltipHandler.TipRegion(rect23, tip6);
					}
				}


				string childhoodInfo = "Childhood".Translate() + ": " + curPawn.story.childhood.TitleCapFor(curPawn.gender);
				var pawnBackstoryInfo = new Rect(pawnInfoBox.x, pawnGenderAgeInfo.yMax + 5, pawnInfoBox.width, 35);
				Widgets.Label(pawnBackstoryInfo.ContractedBy(5f), childhoodInfo);

				if (curPawn.story.adulthood != null)
				{
					string adulthoodInfo = "Adulthood".Translate() + ": " + curPawn.story.adulthood.TitleCapFor(curPawn.gender);
					var pawnBackstoryAdulthoodInfo = new Rect(pawnBackstoryInfo.x, pawnBackstoryInfo.yMax, pawnInfoBox.width, 35);
					Widgets.Label(pawnBackstoryAdulthoodInfo.ContractedBy(5f), adulthoodInfo);
				}

				var pawnPortrait = new Rect(pawnInfoBox.xMax + 15, pawnInfoBox.y, 100, pawnInfoBox.height);
				Widgets.DrawMenuSection(pawnPortrait);

				pawnPortrait = pawnPortrait.ContractedBy(17f);

				Rand.PushState();
				Rand.Seed = curPawn.thingIDNumber;
				var rotation = Rand.Chance(0.1f) ? Rot4.Random : Rot4.South; // just a joke
				Rand.PopState();

				GUI.DrawTexture(new Rect(pawnPortrait.center.x - PawnPortraitSize.x / 2f, pawnPortrait.yMin - 24f,
					PawnPortraitSize.x, PawnPortraitSize.y), PortraitsCache.Get(curPawn, PawnPortraitSize, rotation));

				var traitsBoxOuter = new Rect(pawnInfoBox.x, pawnInfoBox.yMax + TraitBoxPosYOffset, TraitBoxWidth, TraitBoxHeight);
				DrawTraits(traitsBoxOuter);

				var incapableOfBoxOuter = new Rect(traitsBoxOuter.xMax + IncapableOfPosXOffset, traitsBoxOuter.y, IncapableOfWidth, traitsBoxOuter.height);
				DrawIncapableOf(incapableOfBoxOuter);

				var greetingBox = new Rect(rect.x + 14, traitsBoxOuter.yMax + 30, rect.width - 38, 180);
				Widgets.DrawMenuSection(greetingBox);
				var greetingTextBox = greetingBox.ContractedBy(10f);
				greetingTextBox.height = 70;
				Rand.PushState(curPawn.thingIDNumber);

				var greetingText = this.applicants.greetings[curPawn.thingIDNumber];
				Rand.PopState();
				Widgets.Label(greetingTextBox, greetingText);
				var passionsInfoBox = new Rect(greetingTextBox.x, greetingTextBox.yMax + 5, greetingTextBox.width, 30);
				Widgets.Label(passionsInfoBox, "RimDeed.VeryPassionateWith".Translate());

				Text.Font = GameFont.Small;
				DrawSkills(passionsInfoBox);

				Rect accept = new Rect(rect.width - 150, greetingBox.yMax + 15, 70, 20);
				GUIHelper.OptOnGUI(acceptOption, accept);

				Rect cancel = new Rect(accept.xMax + 5, accept.y, 70, 20);
				GUIHelper.OptOnGUI(cancelOption, cancel);
			}
			else
            {
				Text.Font = GameFont.Small;
				Rect exit = new Rect(rect.width - 70, rect.height - 40, 70, 20);
				GUIHelper.OptOnGUI(exitOption, exit);

				Text.Font = GameFont.Medium;
				Rect error = new Rect(rect.x + (rect.width / 2) - 120, rect.height / 2, 300, 50);
				Widgets.Label(error, "RimDeed.Error".Translate());
			}

			GUI.color = Color.white;
			Text.Font = GameFont.Small;
			Text.Anchor = TextAnchor.UpperLeft;
		}

		private void DrawSkills(Rect passionsInfoBox)
        {

			Vector2 offset = new Vector2(passionsInfoBox.x + 5, passionsInfoBox.y + 30);
			Text.Font = GameFont.Small;

			List<SkillDef> allDefsListForReading = DefDatabase<SkillDef>.AllDefsListForReading;
			for (int i = 0; i < allDefsListForReading.Count; i++)
			{
				float x = Text.CalcSize(allDefsListForReading[i].skillLabel.CapitalizeFirst()).x;
				if (x > SkillUI.levelLabelWidth)
				{
					SkillUI.levelLabelWidth = x;
				}
			}

			var skillsWithPassions = curPawn.skills.skills.Where(x => !x.TotallyDisabled).OrderByDescending(x => x.Level).Take(2).Select(x => x.def).ToList();
			for (int j = 0; j < skillsWithPassions.Count; j++)
			{
				SkillDef skillDef = skillsWithPassions[j];
				float y = (float)j * 27f + offset.y;
				SkillUI.DrawSkill(curPawn.skills.GetSkill(skillDef), new Vector2(offset.x, y), SkillUI.SkillDrawMode.Gameplay);
			}
		}
		private void DrawTraits(Rect traitsBoxOuter)
        {
			Widgets.DrawMenuSection(traitsBoxOuter);
			var traitsBox = traitsBoxOuter.ContractedBy(5f);

			List<Trait> traits = curPawn.story.traits.allTraits;
			Text.Font = GameFont.Medium;
			float currentY2 = traitsBox.y;
			Widgets.Label(new Rect(traitsBox.x, currentY2, 200f, 30f), "Traits".Translate());
			currentY2 += 30f;
			Text.Font = GameFont.Small;
			if (traits == null || traits.Count == 0)
			{
				Color color = GUI.color;
				GUI.color = Color.gray;
				Rect rect12 = new Rect(traitsBox.x, currentY2, traitsBox.width, 24f);
				if (Mouse.IsOver(rect12))
				{
					Widgets.DrawHighlight(rect12);
				}
				Widgets.Label(rect12, "None".Translate());
				currentY2 += rect12.height + 2f;
				TooltipHandler.TipRegionByKey(rect12, "None");
				GUI.color = color;
			}
			else
			{
				GenUI.DrawElementStack(new Rect(traitsBox.x, currentY2, traitsBox.width - 5f, traitsBox.height / 4), 22f,
					curPawn.story.traits.allTraits, delegate (Rect r, Trait trait)
					{
						Color color2 = GUI.color;
						GUI.color = StackElementBackground;
						GUI.DrawTexture(r, BaseContent.WhiteTex);
						GUI.color = color2;
						if (Mouse.IsOver(r))
						{
							Widgets.DrawHighlight(r);
						}
						Widgets.Label(new Rect(r.x + 5f, r.y, r.width - 10f, r.height), trait.LabelCap);
						if (Mouse.IsOver(r))
						{
							TooltipHandler.TipRegion(tip: new TipSignal(() => trait.TipString(curPawn), (int)currentY2 * 37), rect: r);
						}
					}, (Trait trait) => Text.CalcSize(trait.LabelCap).x + 10f);
			}
		}

		private void DrawIncapableOf(Rect incapableOfBoxOuter)
        {
			Widgets.DrawMenuSection(incapableOfBoxOuter);
			var incapableOfBox = incapableOfBoxOuter.ContractedBy(5f);
			Text.Font = GameFont.Medium;
			float currentY3 = incapableOfBox.y;
			Widgets.Label(new Rect(incapableOfBox.x, currentY3, 200f, 30f), "IncapableOf".Translate(curPawn));
			currentY3 += 30f;
			Text.Font = GameFont.Small;

			WorkTags disabledTags = curPawn.CombinedDisabledWorkTags;
			List<WorkTags> disabledTagsList = WorkTagsFrom(disabledTags).ToList();
			bool allowWorkTagVerticalLayout = false;
			GenUI.StackElementWidthGetter<WorkTags> workTagWidthGetter = (WorkTags tag) => Text.CalcSize(tag.LabelTranslated().CapitalizeFirst()).x + 10f;

			if (disabledTags == WorkTags.None)
			{
				GUI.color = Color.gray;
				Rect rect13 = new Rect(incapableOfBox.x, currentY3, incapableOfBox.width, 24f);
				if (Mouse.IsOver(rect13))
				{
					Widgets.DrawHighlight(rect13);
				}
				Widgets.Label(rect13, "None".Translate());
				TooltipHandler.TipRegionByKey(rect13, "None");
			}
			else
			{
				GenUI.StackElementDrawer<WorkTags> drawer = delegate (Rect r, WorkTags tag)
				{
					Color color3 = GUI.color;
					GUI.color = StackElementBackground;
					GUI.DrawTexture(r, BaseContent.WhiteTex);
					GUI.color = color3;
					GUI.color = GetDisabledWorkTagLabelColor(curPawn, tag);
					if (Mouse.IsOver(r))
					{
						Widgets.DrawHighlight(r);
					}
					Widgets.Label(new Rect(r.x + 5f, r.y, r.width - 10f, r.height), tag.LabelTranslated().CapitalizeFirst());
					if (Mouse.IsOver(r))
					{
						TooltipHandler.TipRegion(tip: new TipSignal(() => GetWorkTypeDisabledCausedBy(curPawn, tag) + "\n" + GetWorkTypesDisabledByWorkTag(tag), (int)currentY3 * 32), rect: r);
					}
				};
				if (allowWorkTagVerticalLayout)
				{
					GenUI.DrawElementStackVertical(new Rect(incapableOfBox.x, currentY3, incapableOfBox.width - 5f, incapableOfBox.height / (float)4), 22f, disabledTagsList, drawer, workTagWidthGetter);
				}
				else
				{
					GenUI.DrawElementStack(new Rect(incapableOfBox.x, currentY3, incapableOfBox.width - 5f, incapableOfBox.height / (float)4), 22f, disabledTagsList, drawer, workTagWidthGetter, 5f);
				}
			}
		}

		private static IEnumerable<WorkTags> WorkTagsFrom(WorkTags tags)
		{
			foreach (WorkTags allSelectedItem in tags.GetAllSelectedItems<WorkTags>())
			{
				if (allSelectedItem != 0)
				{
					yield return allSelectedItem;
				}
			}
		}
		private static string GetWorkTypeDisabledCausedBy(Pawn pawn, WorkTags workTag)
		{
			List<object> workTypeDisableCauses = GetWorkTypeDisableCauses(pawn, workTag);
			StringBuilder stringBuilder = new StringBuilder();
			foreach (object item in workTypeDisableCauses)
			{
				if (item is Backstory)
				{
					stringBuilder.AppendLine("IncapableOfTooltipBackstory".Translate((item as Backstory).TitleFor(pawn.gender)));
				}
				else if (item is Trait)
				{
					stringBuilder.AppendLine("IncapableOfTooltipTrait".Translate((item as Trait).LabelCap));
				}
				else if (item is Hediff)
				{
					stringBuilder.AppendLine("IncapableOfTooltipHediff".Translate((item as Hediff).LabelCap));
				}
				else if (item is RoyalTitle)
				{
					stringBuilder.AppendLine("IncapableOfTooltipTitle".Translate((item as RoyalTitle).def.GetLabelFor(pawn)));
				}
				else if (item is Quest)
				{
					stringBuilder.AppendLine("IncapableOfTooltipQuest".Translate((item as Quest).name));
				}
			}
			return stringBuilder.ToString();
		}

		private static readonly Color TitleCausedWorkTagDisableColor = new Color(0.67f, 0.84f, 0.9f);
		private static Color GetDisabledWorkTagLabelColor(Pawn pawn, WorkTags workTag)
		{
			foreach (object workTypeDisableCause in GetWorkTypeDisableCauses(pawn, workTag))
			{
				if (workTypeDisableCause is RoyalTitleDef)
				{
					return TitleCausedWorkTagDisableColor;
				}
			}
			return Color.white;
		}

		private static List<object> GetWorkTypeDisableCauses(Pawn pawn, WorkTags workTag)
		{
			List<object> list = new List<object>();
			if (pawn.story != null && pawn.story.childhood != null && (pawn.story.childhood.workDisables & workTag) != 0)
			{
				list.Add(pawn.story.childhood);
			}
			if (pawn.story != null && pawn.story.adulthood != null && (pawn.story.adulthood.workDisables & workTag) != 0)
			{
				list.Add(pawn.story.adulthood);
			}
			if (pawn.health != null && pawn.health.hediffSet != null)
			{
				foreach (Hediff hediff in pawn.health.hediffSet.hediffs)
				{
					HediffStage curStage = hediff.CurStage;
					if (curStage != null && (curStage.disabledWorkTags & workTag) != 0)
					{
						list.Add(hediff);
					}
				}
			}
			if (pawn.story.traits != null)
			{
				for (int i = 0; i < pawn.story.traits.allTraits.Count; i++)
				{
					Trait trait = pawn.story.traits.allTraits[i];
					if ((trait.def.disabledWorkTags & workTag) != 0)
					{
						list.Add(trait);
					}
				}
			}
			if (pawn.royalty != null)
			{
				foreach (RoyalTitle item in pawn.royalty.AllTitlesForReading)
				{
					if (item.conceited && (item.def.disabledWorkTags & workTag) != 0)
					{
						list.Add(item);
					}
				}
			}
			foreach (QuestPart_WorkDisabled item2 in QuestUtility.GetWorkDisabledQuestPart(pawn))
			{
				if ((item2.disabledWorkTags & workTag) != 0 && !list.Contains(item2.quest))
				{
					list.Add(item2.quest);
				}
			}
			return list;
		}
		private static string GetWorkTypesDisabledByWorkTag(WorkTags workTag)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine("IncapableOfTooltipWorkTypes".Translate());
			foreach (WorkTypeDef allDef in DefDatabase<WorkTypeDef>.AllDefs)
			{
				if ((allDef.workTags & workTag) > WorkTags.None)
				{
					stringBuilder.Append("- ");
					stringBuilder.AppendLine(allDef.pawnLabel);
				}
			}
			return stringBuilder.ToString();
		}
		private string CurPawnInfo()
		{
			string text = (curPawn.gender == Gender.None) ? "" : curPawn.gender.GetLabel(curPawn.AnimalOrWildMan());
			if (curPawn.RaceProps.Animal || curPawn.RaceProps.IsMechanoid)
			{
				string str = GenLabel.BestKindLabel(curPawn, mustNoteGender: false, mustNoteLifeStage: true);
				if (curPawn.Name != null)
				{
					text = text + " " + str;
				}
			}
			if (curPawn.ageTracker != null)
			{
				if (text.Length > 0)
				{
					text += ", ";
				}
				text += "AgeIndicator".Translate(curPawn.ageTracker.AgeNumberString);
			}
			return text.CapitalizeFirst();
		}



		private void DrawPawnList(Rect rect)
		{
			rect = rect.ContractedBy(4f);

			DrawPawnListLabelAbove(rect, "RimDeed.JobApplications".Translate());
			for (int i = 0; i < applicants.applicants.Count; i++)
			{
				Pawn pawn = applicants.applicants[i];
				GUI.BeginGroup(rect);
				Rect rect3 = new Rect(Vector2.zero, rect.size);
				Widgets.DrawOptionBackground(rect3, curPawn == pawn);
				MouseoverSounds.DoRegion(rect3);
				GUI.color = new Color(1f, 1f, 1f, 0.2f);
				Rand.PushState();
				Rand.Seed = pawn.thingIDNumber;
				var rotation = Rand.Chance(0.1f) ? Rot4.Random : Rot4.South; // just a joke
				Rand.PopState();
				GUI.DrawTexture(new Rect(110f - PawnSelectorPortraitSize.x / 2f, 40f - PawnSelectorPortraitSize.y / 2f, PawnSelectorPortraitSize.x, PawnSelectorPortraitSize.y), PortraitsCache.Get(pawn, PawnSelectorPortraitSize, rotation));
				GUI.color = Color.white;
				Rect rect4 = rect3.ContractedBy(4f).Rounded();
				NameTriple nameTriple = pawn.Name as NameTriple;
				Widgets.Label(label: (nameTriple == null) ? pawn.LabelShort : (string.IsNullOrEmpty(nameTriple.Nick) ? nameTriple.First : nameTriple.Nick), rect: rect4.TopPart(0.5f).Rounded());
				if (Text.CalcSize(pawn.story.TitleCap).x > rect4.width)
				{
					Widgets.Label(rect4.BottomPart(0.5f).Rounded(), pawn.story.TitleShortCap);
				}
				else
				{
					Widgets.Label(rect4.BottomPart(0.5f).Rounded(), pawn.story.TitleCap);
				}
				if (Event.current.type == EventType.MouseDown && Mouse.IsOver(rect3))
				{
					curPawn = pawn;
					SoundDefOf.Tick_Tiny.PlayOneShotOnCamera();
				}
				GUI.EndGroup();
				rect.y += 60f;
			}
		}

		private void DrawPawnListLabelAbove(Rect rect, string label)
		{
			rect.yMax = rect.yMin;
			rect.yMin -= 35f;
			//rect.xMin -= 4f;
			Text.Font = GameFont.Tiny;
			Text.Anchor = TextAnchor.LowerLeft;
			Widgets.Label(rect, label);
			Text.Anchor = TextAnchor.UpperLeft;
			Text.Font = GameFont.Small;
		}

		private void DrawPawnLabel(Rect rect)
        {
			NameTriple nameTriple = curPawn.Name as NameTriple;
			if (nameTriple != null)
			{
				Widgets.DrawMenuSection(rect);
				Text.Font = GameFont.Medium;
				Text.Anchor = TextAnchor.MiddleCenter;
				Widgets.Label(rect.ContractedBy(3), nameTriple.ToStringFull);
			}
		}

		public void SelectPawn(Pawn c)
		{
			if (c != curPawn)
			{
				curPawn = c;
			}
		}
	}
}
