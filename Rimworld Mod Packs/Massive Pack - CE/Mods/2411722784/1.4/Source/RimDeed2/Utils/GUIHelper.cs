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
using Verse.Grammar;
using Verse.Sound;
using static Verse.Widgets;

namespace Rimdeed
{
	public static class GUIHelper
	{
		public static float OptOnGUI(DiaOption option, Rect rect, bool active = true)
		{
			Color textColor = Widgets.NormalOptionColor;
			string text = option.text;
			if (option.disabled)
			{
				textColor = option.DisabledOptionColor;
				if (option.disabledReason != null)
				{
					text = text + " (" + option.disabledReason + ")";
				}
			}
			rect.height = Text.CalcHeight(text, rect.width);
			if (option.hyperlink.def != null)
			{
				Widgets.HyperlinkWithIcon(rect, option.hyperlink, text);
			}
			else if (ButtonTextWorker(rect, text, drawBackground: false, !option.disabled, textColor, active && !option.disabled, false) == DraggableResult.Pressed)
			{
				option.Activate();
			}
			return rect.height;
		}

		private static DraggableResult ButtonTextWorker(Rect rect, string label, bool drawBackground, bool doMouseoverSound, Color textColor, bool active, bool draggable)
		{
			TextAnchor anchor = Text.Anchor;
			Color color = GUI.color;
			if (drawBackground)
			{
				Texture2D atlas = ButtonBGAtlas;
				if (Mouse.IsOver(rect))
				{
					atlas = ButtonBGAtlasMouseover;
					if (Input.GetMouseButton(0))
					{
						atlas = ButtonBGAtlasClick;
					}
				}
				DrawAtlas(rect, atlas);
			}
			if (doMouseoverSound)
			{
				MouseoverSounds.DoRegion(rect);
			}
			if (!drawBackground)
			{
				GUI.color = textColor;
				if (Mouse.IsOver(rect))
				{
					GUI.color = MouseoverOptionColor;
				}
			}
			if (drawBackground)
			{
				Text.Anchor = TextAnchor.MiddleCenter;
			}
			else
			{
			}
			bool wordWrap = Text.WordWrap;
			if (rect.height < Text.LineHeight * 2f)
			{
				//Text.WordWrap = false;
			}
			Label(rect, label);
			Text.Anchor = anchor;
			GUI.color = color;
			Text.WordWrap = wordWrap;
			if (active && draggable)
			{
				return ButtonInvisibleDraggable(rect);
			}
			if (active)
			{
				if (!ButtonInvisible(rect, doMouseoverSound: false))
				{
					return DraggableResult.Idle;
				}
				return DraggableResult.Pressed;
			}
			return DraggableResult.Idle;
		}
	}
}
