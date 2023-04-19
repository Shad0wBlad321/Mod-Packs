using RimWorld;
using RimWorld.BaseGen;
using System.Linq;
using Verse;
using Verse.AI.Group;

namespace VFE_Settlers.GenSteps
{
    public class GenStep_CaravanRaid : GenStep_Scatterer
    {
        public override int SeedPart
        {
            get
            {
                return 383313881;
            }
        }

        protected override bool CanScatterAt(IntVec3 c, Map map)
        {
            return base.CanScatterAt(c, map) && c.Standable(map);
        }

        protected override void ScatterAt(IntVec3 loc, Map map, GenStepParams parms, int count = 1)
        {
            CellRect rectToDefend;
            if (!MapGenerator.TryGetVar<CellRect>("RectOfInterest", out rectToDefend))
            {
                rectToDefend = CellRect.CenteredOn(map.Center, 10);
            }
            Faction faction;
            if (map.ParentFaction == null || map.ParentFaction == Faction.OfPlayer)
            {
                faction = Find.FactionManager.RandomEnemyFaction(false, false, true, TechLevel.Undefined);
            }
            else
            {
                faction = map.ParentFaction;
            }

            ResolveParams resolveParamsPawn = default;
            resolveParamsPawn.rect = rectToDefend;
            resolveParamsPawn.faction = faction;
            resolveParamsPawn.singlePawnLord = LordMaker.MakeNewLord(faction, new LordJob_DefendBase(faction, rectToDefend.CenterCell), map, null);
            resolveParamsPawn.pawnGroupKindDef = faction.def.pawnGroupMakers.FirstOrDefault(x => x.kindDef.defName == "Trader").kindDef ?? PawnGroupKindDefOf.Trader;
            resolveParamsPawn.pawnGroupMakerParams = new PawnGroupMakerParms();
            resolveParamsPawn.pawnGroupMakerParams.tile = map.Tile;
            resolveParamsPawn.pawnGroupMakerParams.faction = faction;
            resolveParamsPawn.pawnGroupMakerParams.inhabitants = true;
            resolveParamsPawn.pawnGroupMakerParams.seed = resolveParamsPawn.settlementPawnGroupSeed;
            resolveParamsPawn.pawnGroupMakerParams.points = StorytellerUtility.DefaultParmsNow(IncidentCategoryDefOf.ThreatBig, Find.RandomPlayerHomeMap).points / 2;
            BaseGen.symbolStack.Push("pawnGroup", resolveParamsPawn);
            BaseGen.globalSettings.map = map;
            BaseGen.Generate();

            MapGenerator.rootsToUnfog.Add(loc);
        }
    }
}