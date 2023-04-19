using RimWorld;
using RimWorld.BaseGen;
using System.Linq;
using Verse;
using Verse.AI.Group;

namespace VFE_Settlers.Utilities
{
    public static class UtilityBase
    {
        public static void GenerateStreet(Map map, TerrainDef floorDef, CellRect rect)
        {
            TerrainGrid terrainGrid = map.terrainGrid;
            foreach (var cell in rect)
            {
                terrainGrid.SetTerrain(cell, floorDef);
            }
        }

        /// <summary>
        /// This makes a block town with a road that goes through town.
        /// </summary>
        /// <param name="map">The Rimworld Map Object</param>
        /// <param name="horizontal">Whether the road should horizontal or vertical</param>
        /// <param name="faction">The faction this base belongs two</param>
        /// <param name="c">The center of the town</param>
        /// <param name="settlementWidth">The width of the settlement</param>
        /// <param name="settlementHeight">The height of the settlement</param>
        /// <param name="roadWidth">The extra added space for the road in between the settlement sides, this value is doubled on use</param>
        /// <param name="roadExtension">The extra added space before and after the road start and end</param>
        /// <param name="blocks">The number of sub division each side should have</param>
        public static bool GenerateTown(Map map, bool horizontal, Faction faction, IntVec3 c, int settlementWidth, int settlementHeight, int roadWidth, int roadExtension, int blocks)
        {
            try
            {
                if (settlementWidth / blocks < 12 || settlementHeight / blocks < 12)
                {
                    Log.Message("Failed to generate settlement.");
                    return false;
                }
                CellRect rectRoad;
                CellRect rectRoad1;
                CellRect rectRoad2;
                TerrainDef rockDef = BaseGenUtility.RegionalRockTerrainDef(map.Tile, true);
                TerrainDef dirtDef = DefDatabase<TerrainDef>.AllDefsListForReading.First(f => f.defName == "PackedDirt");
                TerrainDef woodDef = DefDatabase<TerrainDef>.AllDefsListForReading.First(f => f.defName == "WoodPlankFloor");
                if (Rand.Bool)
                {
                    int width = settlementWidth / blocks;
                    int height = settlementHeight / 2;
                    int x = c.x - (settlementWidth / 2);
                    for (int i = 0; i < blocks; i++)
                    {
                        CellRect temp = new CellRect(x + ((width + (i == 0 ? 0 : 1)) * i), c.z - (height + roadWidth + 1), width - 1, height - 1);
                        temp.ClipInsideMap(map);
                        ResolveParams resolveParamsInner = default;
                        resolveParamsInner.rect = temp;
                        resolveParamsInner.faction = faction;
                        BaseGen.symbolStack.Push("outdoorLighting", resolveParamsInner);
                        BaseGen.symbolStack.Push("ensureCanReachMapEdge", resolveParamsInner);
                        BaseGen.symbolStack.Push("basePart_outdoors", resolveParamsInner);
                        ResolveParams resolveParamsBridge = resolveParamsInner;
                        resolveParamsBridge.floorDef = TerrainDefOf.Bridge;
                        bool? floorOnlyIfTerrainSupports = resolveParamsInner.floorOnlyIfTerrainSupports;
                        resolveParamsBridge.floorOnlyIfTerrainSupports = new bool?(!floorOnlyIfTerrainSupports.HasValue || floorOnlyIfTerrainSupports.Value);
                        BaseGen.symbolStack.Push("floor", resolveParamsBridge);
                    }
                    for (int i = 0; i < blocks; i++)
                    {
                        CellRect temp = new CellRect(x + ((width + (i == 0 ? 0 : 1)) * i), c.z + 2, width - 1, height - 1);
                        temp.ClipInsideMap(map);
                        ResolveParams resolveParamsInner = default;
                        resolveParamsInner.rect = temp;
                        resolveParamsInner.faction = faction;
                        BaseGen.symbolStack.Push("outdoorLighting", resolveParamsInner);
                        BaseGen.symbolStack.Push("ensureCanReachMapEdge", resolveParamsInner);
                        BaseGen.symbolStack.Push("basePart_outdoors", resolveParamsInner);
                        ResolveParams resolveParamsBridge = resolveParamsInner;
                        resolveParamsBridge.floorDef = TerrainDefOf.Bridge;
                        bool? floorOnlyIfTerrainSupports = resolveParamsInner.floorOnlyIfTerrainSupports;
                        resolveParamsBridge.floorOnlyIfTerrainSupports = new bool?(!floorOnlyIfTerrainSupports.HasValue || floorOnlyIfTerrainSupports.Value);
                        BaseGen.symbolStack.Push("floor", resolveParamsBridge);
                    }
                    rectRoad = new CellRect(x - roadExtension, (c.z - roadWidth), settlementWidth + (roadExtension * 2), roadWidth);
                    rectRoad1 = new CellRect(x - (roadExtension - 2), (c.z - (roadWidth + 2)), settlementWidth + (roadExtension * 2) - 4, 2);
                    rectRoad2 = new CellRect(x - (roadExtension - 2), c.z, settlementWidth + (roadExtension * 2) - 4, 2);
                }
                else
                {
                    int width = settlementWidth / 2;
                    int height = settlementHeight / blocks;
                    int z = c.z - (settlementHeight / 2);
                    for (int i = 0; i < blocks; i++)
                    {
                        CellRect temp = new CellRect(c.x - (width + roadWidth + 1), z + ((height + (i == 0 ? 0 : 1)) * i), width - 1, height - 1);
                        //CellRect temp2 = new CellRect(c.x - (width + roadWidth + 2), (z + ((height + (i == 0 ? 0 : 1)) * i)) - 1, width, height + 1);
                        //temp.ClipInsideMap(map);
                        //GenerateStreet(map, dirtDef, temp2);
                        ResolveParams resolveParamsInner = default;
                        resolveParamsInner.rect = temp;
                        resolveParamsInner.faction = faction;
                        BaseGen.symbolStack.Push("outdoorLighting", resolveParamsInner);
                        BaseGen.symbolStack.Push("ensureCanReachMapEdge", resolveParamsInner);
                        BaseGen.symbolStack.Push("basePart_outdoors", resolveParamsInner);
                        ResolveParams resolveParamsBridge = resolveParamsInner;
                        resolveParamsBridge.floorDef = TerrainDefOf.Bridge;
                        bool? floorOnlyIfTerrainSupports = resolveParamsInner.floorOnlyIfTerrainSupports;
                        resolveParamsBridge.floorOnlyIfTerrainSupports = new bool?(!floorOnlyIfTerrainSupports.HasValue || floorOnlyIfTerrainSupports.Value);
                        BaseGen.symbolStack.Push("floor", resolveParamsBridge);
                    }
                    for (int i = 0; i < blocks; i++)
                    {
                        CellRect temp = new CellRect(c.x + 2, z + ((height + (i == 0 ? 0 : 1)) * i), width - 1, height - 1);
                        //CellRect temp2 = new CellRect(c.x + 2, (z + ((height + (i == 0 ? 0 : 1)) * i)) - 1, width , height + 1);
                        //temp.ClipInsideMap(map);
                        //GenerateStreet(map, dirtDef, temp2);
                        ResolveParams resolveParamsInner = default;
                        resolveParamsInner.rect = temp;
                        resolveParamsInner.faction = faction;
                        BaseGen.symbolStack.Push("outdoorLighting", resolveParamsInner);
                        BaseGen.symbolStack.Push("ensureCanReachMapEdge", resolveParamsInner);
                        BaseGen.symbolStack.Push("basePart_outdoors", resolveParamsInner);
                        ResolveParams resolveParamsBridge = resolveParamsInner;
                        resolveParamsBridge.floorDef = TerrainDefOf.Bridge;
                        bool? floorOnlyIfTerrainSupports = resolveParamsInner.floorOnlyIfTerrainSupports;
                        resolveParamsBridge.floorOnlyIfTerrainSupports = new bool?(!floorOnlyIfTerrainSupports.HasValue || floorOnlyIfTerrainSupports.Value);
                        BaseGen.symbolStack.Push("floor", resolveParamsBridge);
                    }
                    rectRoad = new CellRect(c.x - roadWidth, z - roadExtension, roadWidth, settlementHeight + (roadExtension * 2));
                    rectRoad1 = new CellRect(c.x - (roadWidth + 2), z - (roadExtension - 2), 2, settlementHeight + (roadExtension * 2) - 4);
                    rectRoad2 = new CellRect(c.x, z - (roadExtension - 2), 2, settlementHeight + (roadExtension * 2) - 4);
                }
                rectRoad.ClipInsideMap(map);
                rectRoad1.ClipInsideMap(map);
                rectRoad2.ClipInsideMap(map);
                GenerateStreet(map, dirtDef, rectRoad);
                GenerateStreet(map, rockDef, rectRoad1);
                GenerateStreet(map, rockDef, rectRoad2);

                ResolveParams resolveParamsPawn = default;
                resolveParamsPawn.rect = rectRoad;
                resolveParamsPawn.faction = faction;
                TraverseParms traverseParms = TraverseParms.For(TraverseMode.PassDoors, Danger.Deadly, false);
                resolveParamsPawn.singlePawnLord = LordMaker.MakeNewLord(faction, new LordJob_DefendBase(faction, rectRoad.CenterCell), map, null);
                resolveParamsPawn.pawnGroupKindDef = faction.def.pawnGroupMakers.FirstOrDefault(x => x.kindDef.defName == "Settlement").kindDef ?? PawnGroupKindDefOf.Settlement;
                resolveParamsPawn.pawnGroupMakerParams = new PawnGroupMakerParms();
                resolveParamsPawn.pawnGroupMakerParams.tile = map.Tile;
                resolveParamsPawn.pawnGroupMakerParams.faction = faction;
                PawnGroupMakerParms parms = resolveParamsPawn.pawnGroupMakerParams;
                float? settlementPawnGroupPoints = resolveParamsPawn.settlementPawnGroupPoints;
                parms.points = ((!settlementPawnGroupPoints.HasValue) ? SymbolResolver_Settlement.DefaultPawnsPoints.RandomInRange : settlementPawnGroupPoints.Value);
                resolveParamsPawn.pawnGroupMakerParams.inhabitants = true;
                resolveParamsPawn.pawnGroupMakerParams.seed = resolveParamsPawn.settlementPawnGroupSeed;
                BaseGen.symbolStack.Push("pawnGroup", resolveParamsPawn);
                BaseGen.symbolStack.Push("outdoorLighting", resolveParamsPawn);

                BaseGen.globalSettings.map = map;
                BaseGen.globalSettings.minBuildings = 1;
                BaseGen.globalSettings.minBarracks = 1;
                BaseGen.Generate();
                return true;
            }
            catch (System.Exception)
            {
                Log.Message("Failed to generate settlement.");
                return false;
            }
        }
    }
}