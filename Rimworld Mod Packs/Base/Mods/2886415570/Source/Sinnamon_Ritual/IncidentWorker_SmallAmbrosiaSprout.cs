using System;
using System.Collections.Generic;
using RimWorld;
using Verse;

namespace RitualRewards.Sinnamon_Ritual;

public class IncidentWorker_SmallAmbrosiaSprout : IncidentWorker
{
    private const int MinRoomCells = 32;

    private const int SpawnRadius = 6;

    protected override bool CanFireNowSub(IncidentParms parms)
    {
        if (!base.CanFireNowSub(parms))
            return false;

        Map map = (Map)parms.target;

        return map.weatherManager.growthSeasonMemory.GrowthSeasonOutdoorsNow && TryFindRootCell(map, out _);
    }

    protected override bool TryExecuteWorker(IncidentParms parms)
    {
        Map map = (Map)parms.target;
        if (!TryFindRootCell(map, out IntVec3 cell))
            return false;

        Thing thing = null;
        int num = Rand.Range((int)parms.points % 100, (int)parms.points / 100);
        for (int i = 0; i < num; i++)
        {
            IntVec3 root = cell;
            Map map2 = map;
            int radius = SpawnRadius;
            Predicate<IntVec3> predicate;
            Predicate<IntVec3> extraValidator = predicate = (x) => CanSpawnAt(x, map);
            if (!CellFinder.TryRandomClosewalkCellNear(root, map2, radius, out IntVec3 result, extraValidator))
                break;

            result.GetPlant(map)?.Destroy();
            Thing thing2 = GenSpawn.Spawn(ThingDefOf.Plant_Ambrosia, result, map);
            thing ??= thing2;
        }

        if (thing == null)
            return false;

        SendStandardLetter(parms, thing);
        return true;
    }

    private static bool TryFindRootCell(Map map, out IntVec3 cell)
    {
        return CellFinderLoose.TryFindRandomNotEdgeCellWith(10, (x) => CanSpawnAt(x, map) && x.GetRoom(map).CellCount >= 2 * MinRoomCells, map, out cell);
    }

    private static bool CanSpawnAt(IntVec3 c, Map map)
    {
        if (!c.Standable(map) || c.Fogged(map) ||
            map.fertilityGrid.FertilityAt(c) < ThingDefOf.Plant_Ambrosia.plant.fertilityMin ||
            !c.GetRoom(map).PsychologicallyOutdoors || c.GetEdifice(map) != null ||
            !PlantUtility.GrowthSeasonNow(c, map))
        {
            return false;
        }

        Plant plant = c.GetPlant(map);
        if (plant != null && plant.def.plant.growDays > 10f)
            return false;

        List<Thing> thingList = c.GetThingList(map);
        for (int i = 0; i < thingList.Count; i++)
        {
            if (thingList[i].def == ThingDefOf.Plant_Ambrosia)
                return false;
        }

        return true;
    }
}
