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
	public class Dialog_RimDeed_ContactSupport_NextPage : Dialog_NodeTree
	{
        public override float Margin => 0f;
        public override Vector2 InitialSize => new Vector2(620f, Mathf.Min(480, UI.screenHeight));

		private DiaOption backOption;
		private DiaOption cancelOption;
		private DiaOption xOption;
		private Pawn negotiator;
		public Dialog_RimDeed_ContactSupport_NextPage(Pawn negotiator, DiaNode startNode, bool radioMode)
			: base(startNode, radioMode)
		{
			this.negotiator = negotiator;
			cancelOption = new DiaOption("RimDeed.Exit".Translate());
			cancelOption.resolveTree = true;
			cancelOption.dialog = this;

			backOption = new DiaOption("RimDeed.Back".Translate());
			backOption.dialog = this;
			backOption.action = delegate ()
			{
				Find.WindowStack.TryRemove(this);
				DiaNode diaNode = new DiaNode("RimDeed");
				Find.WindowStack.Add(new Dialog_RimDeed_MainPage(negotiator, diaNode, false));
			};

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

			Rect logo = new Rect(inRect.x, inRect.y, Textures.Customer_Support.width, Textures.Customer_Support.height);
			GUI.DrawTexture(logo, Textures.Customer_Support);
			Text.Font = GameFont.Tiny;

			Text.Anchor = TextAnchor.MiddleCenter;
			Rect customerService = new Rect(inRect.x + 120, inRect.y + 260, 360, 30);
			Text.Font = GameFont.Medium;
			Widgets.Label(customerService, "RimDeed.CustomerService".Translate());

			Text.Font = GameFont.Small;

			Rect howCanWeHelpYou = new Rect(customerService.x, customerService.yMax, 360, 40);
			Widgets.Label(howCanWeHelpYou, "RimDeed.ThankYouWeWillLookIntoIt".Translate());


			Rect rect4 = new Rect(inRect.x + 265, customerService.yMax + 150, inRect.width - 30f, 999f);
			GUIHelper.OptOnGUI(cancelOption, rect4);

			Rect back = new Rect(inRect.x - 230, rect4.y, inRect.width - 30f, 999f);
			GUIHelper.OptOnGUI(backOption, back);
			Text.Anchor = TextAnchor.UpperLeft;
		}
	}
}
