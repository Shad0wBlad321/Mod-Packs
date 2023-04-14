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

namespace Rimdeed
{
	[StaticConstructorOnStartup]
	public static class Textures
	{
		public static readonly Texture2D Rimdeed_corner = ContentFinder<Texture2D>.Get("UI/Rimdeed_corner");
		public static readonly Texture2D Rimdeed_Interface = ContentFinder<Texture2D>.Get("UI/Rimdeed_Interface");
		public static readonly Texture2D Rimdeed_Main = ContentFinder<Texture2D>.Get("UI/Rimdeed_Main");
		public static readonly Texture2D Customer_Support = ContentFinder<Texture2D>.Get("UI/Customer_Support");
		public static readonly Texture2D TradeIn = ContentFinder<Texture2D>.Get("UI/TRADEIN");

	}
}
