using HarmonyLib;
using RimWorld;
using Verse;

namespace VPEAutoPlow
{
	[HarmonyPatch(typeof(Plant), "PlantCollected")]
	public static class Patch_Plant_PlantCollected
	{
		[HarmonyPrefix]
		[HarmonyBefore(new string[] { "com.vanillaplantsexpanded" })]
		public static void AccessMap(Plant __instance, out Map __state)
		{
			__state = __instance.Map;
		}

		[HarmonyPostfix]
		[HarmonyAfter(new string[] { "com.vanillaplantsexpanded", "Owlchemist.SmartFarming" })]
		public static void SetPlowBlueprintAfterHarvest(Plant __instance, Map __state)
		{
			if (__instance.def.plant.HarvestDestroys)
			{
				IntVec3 pos = __instance.Position;
				TerrainDef plowedSoilDef = VPEAutoPlowDefOf.VCE_TilledSoil;
				if (__state.zoneManager.ZoneAt(pos) is Zone_Growing zone && zone.AllowAutoPlow() && plowedSoilDef.IsResearchFinished)
				{
					if (GenConstruct.CanPlaceBlueprintAt(plowedSoilDef, pos, Rot4.North, __state, false).Accepted)
					{
                        GenConstruct.PlaceBlueprintForBuild(plowedSoilDef, pos, __state, Rot4.North, Faction.OfPlayer, null);
					}

				}
			}
		}
	}
}
