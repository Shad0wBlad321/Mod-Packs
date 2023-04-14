using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using RimWorld;
using Verse;
using Verse.Noise;

namespace CDDS
{
    [HarmonyPatch(typeof(DeepDrillUtility), "GetBaseResource")]
    class HarmonyPatches
    {
        static bool Prefix(Map map, IntVec3 cell, ref ThingDef __result)
		{
			//Had to tweak this to be compatible with harmony patch syntax..
			if (!map.Biome.hasBedrock)
			{
				__result = null;
				return false;
			}
			//Their code..
			Rand.PushState();
			Rand.Seed = cell.GetHashCode();
			//Our code..
			__result = DefDatabase<ThingDef>.GetNamed("Luciferium"); //map.GetComponent<CMS.MapComponent_StoneGrid>().StoneResourceAt(cell);
			//What we replaced..
			//ThingDef result = (from rock in Find.World.NaturalRockTypesIn(map.Tile)
			//				   select rock.building.mineableThing).RandomElement<ThingDef>();
			//Their code..
			Rand.PopState();
			//Had to remove for harmony patch syntax compatibility..
			//return result;
			//Our Code..
			return false;
        }
    }
}