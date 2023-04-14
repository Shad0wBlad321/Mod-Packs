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
	public class Dialog_RimDeed_TradePage : Dialog_NodeTree
	{
        public override float Margin => 0f;
        public override Vector2 InitialSize => new Vector2(620f, Mathf.Min(480, UI.screenHeight));

		private DiaOption backOption;
		private DiaOption tradeInEmployeesOption;
		private DiaOption woodLogOption;
		private DiaOption silverOption;
		private DiaOption goldOption;
		private DiaOption cancelOption;
		private DiaOption xOption;
		private Pawn negotiator;
		private RimdeedManager manager;
		public Dialog_RimDeed_TradePage(Pawn negotiator, DiaNode startNode, bool radioMode)
			: base(startNode, radioMode)
		{
			this.negotiator = negotiator;
			manager = RimdeedManager.Instance;
			cancelOption = new DiaOption("RimDeed.Cancel".Translate());
			cancelOption.resolveTree = true;
			cancelOption.dialog = this;

			woodLogOption = new DiaOption("RimDeed.Woodlog".Translate(RimdeedSettings.woodlogCost.ToString("c0")));
			woodLogOption.resolveTree = true;
			woodLogOption.dialog = this;
			if (!TradeUtility.ColonyHasEnoughSilver(Find.CurrentMap, RimdeedSettings.woodlogCost))
			{
				woodLogOption.Disable("RimDeed.NotEnoughMoney".Translate());
			};
			woodLogOption.action = delegate ()
			{
				TradeUtility.LaunchSilver(Find.CurrentMap, RimdeedSettings.woodlogCost);
				Find.WindowStack.TryRemove(this);
				DiaNode diaNode = new DiaNode("RimDeed");
				Find.WindowStack.Add(new Dialog_RimDeed_NextPage(negotiator, diaNode, false));
				manager.RegisterNewOrder(Find.CurrentMap, 1);
			};

			silverOption = new DiaOption("RimDeed.Silver".Translate(RimdeedSettings.silverCost.ToString("c0")));
			silverOption.resolveTree = true;
			silverOption.dialog = this;
			if (!TradeUtility.ColonyHasEnoughSilver(Find.CurrentMap, RimdeedSettings.silverCost))
			{
				silverOption.Disable("RimDeed.NotEnoughMoney".Translate());
			};
			silverOption.action = delegate ()
			{
				TradeUtility.LaunchSilver(Find.CurrentMap, RimdeedSettings.silverCost);
				Find.WindowStack.TryRemove(this);
				DiaNode diaNode = new DiaNode("RimDeed");
				Find.WindowStack.Add(new Dialog_RimDeed_NextPage(negotiator, diaNode, false));
				manager.RegisterNewOrder(Find.CurrentMap, Rand.RangeInclusive(2, 5));
			};

			goldOption = new DiaOption("RimDeed.Gold".Translate(RimdeedSettings.goldCost.ToString("c0")));
			goldOption.resolveTree = true;
			goldOption.dialog = this;
			if (!TradeUtility.ColonyHasEnoughSilver(Find.CurrentMap, RimdeedSettings.goldCost))
			{
				goldOption.Disable("RimDeed.NotEnoughMoney".Translate());
			};
			goldOption.action = delegate ()
			{
				TradeUtility.LaunchSilver(Find.CurrentMap, RimdeedSettings.goldCost);
				Find.WindowStack.TryRemove(this);
				DiaNode diaNode = new DiaNode("RimDeed");
				Find.WindowStack.Add(new Dialog_RimDeed_NextPage(negotiator, diaNode, false));
				manager.RegisterNewOrder(Find.CurrentMap, 5, true);
			};

			tradeInEmployeesOption = new DiaOption("RimDeed.TradeInEmployees".Translate());
			tradeInEmployeesOption.dialog = this;
			tradeInEmployeesOption.action = delegate ()
			{
				manager.pawnTrader.StartTrade(negotiator);
				Find.WindowStack.Add(new Dialog_Trade(negotiator, manager.pawnTrader));
			};
			backOption = new DiaOption("RimDeed.Back".Translate());
			backOption.resolveTree = true;
			backOption.dialog = this;
			backOption.action = delegate ()
			{
				Find.WindowStack.TryRemove(this);
				DiaNode diaNode = new DiaNode("RimDeed");
				Find.WindowStack.Add(new Dialog_RimDeed_MainPage(negotiator, diaNode, false));
			};

			cancelOption = new DiaOption("RimDeed.Cancel".Translate());
			cancelOption.resolveTree = true;
			cancelOption.dialog = this;


			xOption = new DiaOption("X");
			xOption.resolveTree = true;
			xOption.dialog = this;
		}

		public override void PreClose()
		{
			base.PreClose();
			curNode.PreClose();
		}

		public override void PostClose()
		{
			base.PostClose();
			if (closeAction != null)
			{
				closeAction();
			}
		}

		public override void WindowOnGUI()
		{
			if (screenFillColor != Color.clear)
			{
				GUI.color = screenFillColor;
				GUI.DrawTexture(new Rect(0f, 0f, UI.screenWidth, UI.screenHeight), BaseContent.WhiteTex);
				GUI.color = Color.white;
			}
			base.WindowOnGUI();
		}

		public override void DoWindowContents(Rect inRect)
		{
			Text.Font = GameFont.Small;
			Rect xRect = new Rect(inRect.xMax - 20, inRect.y + 5, 20, 20);
			GUIHelper.OptOnGUI(xOption, xRect);

			Rect logo = new Rect(inRect.x, inRect.y - 50, Textures.Rimdeed_Main.width, Textures.Rimdeed_Main.height);
			GUI.DrawTexture(logo, Textures.Rimdeed_Main);
			Text.Font = GameFont.Tiny;

			Text.Anchor = TextAnchor.MiddleCenter;
			Text.Font = GameFont.Medium;
			Rect welcome = new Rect(inRect.x + 150, inRect.y + 170, 360, 40);
			Widgets.Label(welcome, "RimDeed.ServicePackages".Translate());

			Text.Font = GameFont.Small;
			Text.Anchor = TextAnchor.MiddleLeft;

			var woodlogIcon = ThingDefOf.WoodLog.uiIcon;
			Rect woodLogIconBox = new Rect(inRect.x + 60, welcome.yMax + 5, 50, 50);
			GUI.DrawTexture(woodLogIconBox, woodlogIcon);

			Rect woodlog = new Rect(woodLogIconBox.xMax + 20, welcome.yMax + 35, inRect.width - 30f, 999f);
			var height = GUIHelper.OptOnGUI(woodLogOption, woodlog);

			var silverIcon = ThingDefOf.Silver.uiIcon;
			Rect silverIconBox = new Rect(woodLogIconBox.x, woodLogIconBox.yMax + 10, 50, 50);
			GUI.DrawTexture(silverIconBox, silverIcon);

			Rect silver = new Rect(woodlog.x, woodlog.y + height + 30, inRect.width - 30f, 999f);
			height = GUIHelper.OptOnGUI(silverOption, silver);

			var goldIcon = ThingDefOf.Gold.uiIcon;
			Rect goldIconBox = new Rect(silverIconBox.x, silverIconBox.yMax, 50, 50);
			GUI.DrawTexture(goldIconBox, goldIcon);

			Rect gold = new Rect(woodlog.x, silver.y + height + 30, inRect.width - 30f, 999f);
			GUIHelper.OptOnGUI(goldOption, gold);

			Text.Anchor = TextAnchor.MiddleCenter;

			Rect backBox = new Rect(inRect.x + 20, welcome.yMax + 230, 50, 40f);
			GUIHelper.OptOnGUI(backOption, backBox);

			var textSize = Text.CalcSize(tradeInEmployeesOption.text);
			Rect tradeInBox = new Rect(backBox.xMax + 190, backBox.y, textSize.x, textSize.y);
			GUIHelper.OptOnGUI(tradeInEmployeesOption, tradeInBox);
			Rect tradeInIconBox = new Rect((tradeInBox.x - Textures.TradeIn.width) - 5, tradeInBox.y - 5, Textures.TradeIn.width, Textures.TradeIn.height);
			GUI.DrawTexture(tradeInIconBox, Textures.TradeIn);

			Rect cancelBox = new Rect(inRect.width - 90, backBox.y, 60, 40f);
			GUIHelper.OptOnGUI(cancelOption, cancelBox);
			Text.Anchor = TextAnchor.UpperLeft;
		}
	}
}
