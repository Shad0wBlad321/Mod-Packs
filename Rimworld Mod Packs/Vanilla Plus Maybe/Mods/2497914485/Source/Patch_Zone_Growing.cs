using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using Verse;

namespace VPEAutoPlow
{
	public static class Extension_Zone_Growing
	{
		private static readonly ConditionalWeakTable<Zone_Growing, StrongBox<bool>> _allowAutoPlow = new ConditionalWeakTable<Zone_Growing, StrongBox<bool>>();
		
		public static ref bool AllowAutoPlow(this Zone_Growing t)
		{
			return ref _allowAutoPlow.GetOrCreateValue(t).Value;
		}
	}

	[HarmonyPatch(typeof(Zone_Growing), "ExposeData")]
	public static class Patch_Zone_Growing_ExposeData
	{
		[HarmonyPostfix]
		public static void Expose_AllowAutoPlow(Zone_Growing __instance)
		{
			Scribe_Values.Look<bool>(ref __instance.AllowAutoPlow(), "allowAutoPlow", false, false);
		}
	}

	[HarmonyPatch(typeof(Zone_Growing), "GetGizmos")]
	public static class Patch_Zone_Growing_GetGizmos
	{
		[HarmonyPostfix]
		public static IEnumerable<Gizmo> Add_AllowAutoPlow_Gizmo(IEnumerable<Gizmo> __result, Zone_Growing __instance)
		{
			foreach (Gizmo g in __result)
			{
				yield return g;
			}
			if (VPEAutoPlowDefOf.VCE_TilledSoil != null) // preventing bugs. Some mods deactivate that DefOf.
			{
				if (VPEAutoPlowDefOf.VCE_TilledSoil.IsResearchFinished)
				{
					yield return new Command_Toggle
					{
						defaultLabel = "VPEAutoPlow.allowAutoPlow".Translate(),
						defaultDesc = "VPEAutoPlow.allowAutoPlowDesc".Translate(),
						icon = VPEAutoPlowTextures.VCE_Plow,
						isActive = (() => __instance.AllowAutoPlow()),
						toggleAction = delegate ()
						{
							__instance.AllowAutoPlow() = !__instance.AllowAutoPlow();
						}
					};
				}
			}
		}
	}
}
