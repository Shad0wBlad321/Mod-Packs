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
	public class Dialog_RimDeed_MainPage : Dialog_NodeTree
	{
        public override float Margin => 0f;
        public override Vector2 InitialSize => new Vector2(620f, Mathf.Min(480, UI.screenHeight));

		private int silverCost;

		private DiaOption seekJobOption;
		private DiaOption freeTrialOption;
		private DiaOption cancelOption;
		private DiaOption contactSupportOption;
		private DiaOption xOption;

		RimdeedManager manager;
		public Dialog_RimDeed_MainPage(DiaNode startNode, bool radioMode)
			: base(startNode, radioMode)
		{
			silverCost = 5000;
			manager = Find.World.GetComponent<RimdeedManager>();
			seekJobOption = new DiaOption("RimDeed.SeekJobApplications".Translate());
			seekJobOption.dialog = this;
			if (!Building_OrbitalTradeBeacon.AllPowered(Find.CurrentMap).Any())
            {
				seekJobOption.Disable("RimDeed.SeekJobApplicationsNoTradeBeacon".Translate());
			}
			else if (!TradeUtility.ColonyHasEnoughSilver(Find.CurrentMap, silverCost))
			{
				seekJobOption.Disable("RimDeed.SeekJobApplicationsNoEnoughCost".Translate());
			}

			seekJobOption.action = delegate()
			{
				TradeUtility.LaunchSilver(Find.CurrentMap, silverCost);
				Find.WindowStack.TryRemove(this);
				DiaNode diaNode = new DiaNode("RimDeed");
				Find.WindowStack.Add(new Dialog_RimDeed_NextPage(diaNode, false));
				manager.RegisterNewOrder(Find.CurrentMap, Rand.RangeInclusive(2, 5));
			};

			freeTrialOption = new DiaOption("RimDeed.FreeTrial".Translate());
			freeTrialOption.dialog = this;
			freeTrialOption.action = delegate ()
			{
				Find.WindowStack.TryRemove(this);
				DiaNode diaNode = new DiaNode("RimDeed");
				Find.WindowStack.Add(new Dialog_RimDeed_NextPage(diaNode, false));
				manager.RegisterNewOrder(Find.CurrentMap, 1);
				manager.trialExpired = true;
				manager.freeTrialTicks = Find.TickManager.TicksGame + (GenDate.TicksPerDay * 90);
			};

			contactSupportOption = new DiaOption("RimDeed.ContactSupport".Translate());
			contactSupportOption.dialog = this;
			contactSupportOption.action = delegate()
			{
				Find.WindowStack.TryRemove(this);
				DiaNode diaNode = new DiaNode("RimDeed");
				Find.WindowStack.Add(new Dialog_RimDeed_ContactSupport(diaNode, false));
			};
			if (manager.complaintResponceTick + GenDate.TicksPerDay > Find.TickManager.TicksGame)
            {
				contactSupportOption.Disable("RimDeed.InvestigationPending".Translate());
			}
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

			Rect logo = new Rect(inRect.x, inRect.y, Textures.Rimdeed_Interface.width, Textures.Rimdeed_Interface.height);
			GUI.DrawTexture(logo, Textures.Rimdeed_Interface);
			Text.Font = GameFont.Tiny;

			Text.Anchor = TextAnchor.MiddleCenter;
			Rect welcome = new Rect(inRect.x + 120, inRect.y + 270, 360, 40);
			Widgets.Label(welcome, "RimDeed.MainPageText".Translate());

			Rect seekJobRect = new Rect(inRect.x + 20, welcome.yMax + 10, inRect.width - 30f, 999f);
			GUIHelper.OptOnGUI(seekJobOption, seekJobRect);

			if (!Find.World.GetComponent<RimdeedManager>().trialExpired)
            {
				Rect freeTrialRect = new Rect(inRect.x + 20, welcome.yMax + 60, inRect.width - 30f, 999f);
				GUIHelper.OptOnGUI(freeTrialOption, freeTrialRect);
			}

			Text.Font = GameFont.Small;
			float xOffset = 0;
			if (manager.complaintResponceTick > 0 && (manager.complaintResponceTick + GenDate.TicksPerDay) > Find.TickManager.TicksGame)
            {
				xOffset = 100;
			}
			else
            {
				xOffset = 180;
			}
			Rect contactSupportRect = new Rect(inRect.x - xOffset, welcome.yMax + 130, inRect.width - 30f, 999f);
			GUIHelper.OptOnGUI(contactSupportOption, contactSupportRect);

			Rect cancelRect = new Rect(inRect.x + 265, welcome.yMax + 130, inRect.width - 30f, 999f);
			GUIHelper.OptOnGUI(cancelOption, cancelRect);
			Text.Anchor = TextAnchor.UpperLeft;
		}
	}
}
