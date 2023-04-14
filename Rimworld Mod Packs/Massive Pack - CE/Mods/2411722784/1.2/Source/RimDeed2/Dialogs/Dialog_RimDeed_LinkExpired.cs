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
	public class Dialog_RimDeed_LinkExpired : Dialog_NodeTree
	{
        public override float Margin => 0f;
        public override Vector2 InitialSize => new Vector2(620f, Mathf.Min(480, UI.screenHeight));

		private DiaOption cancelOption;
		private DiaOption xOption;
		public Dialog_RimDeed_LinkExpired(DiaNode startNode, bool radioMode)
			: base(startNode, radioMode)
		{
			cancelOption = new DiaOption("RimDeed.Exit".Translate());
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

			Rect logo = new Rect(inRect.x, inRect.y, Textures.Rimdeed_Main.width, Textures.Rimdeed_Main.height);
			GUI.DrawTexture(logo, Textures.Rimdeed_Main);
			Text.Font = GameFont.Tiny;

			Text.Anchor = TextAnchor.MiddleCenter;
			Rect welcome = new Rect(inRect.x + 120, inRect.y + 270, 360, 40);
			Widgets.Label(welcome, "RimDeed.LinkExpired".Translate());

			Text.Font = GameFont.Small;
			Rect rect4 = new Rect(inRect.x + 265, welcome.yMax + 130, inRect.width - 30f, 999f);
			GUIHelper.OptOnGUI(cancelOption, rect4);
			Text.Anchor = TextAnchor.UpperLeft;
		}
	}
}
