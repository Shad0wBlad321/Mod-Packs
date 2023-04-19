using RimWorld.BaseGen;
using System.Collections.Generic;
using Verse;
using System;
using RimWorld;
using Verse.AI.Group;
using System.Linq;

namespace VFE_Settlers.GenSteps {
    public class GenStep_CaravanRaid : GenStep_Scatterer {
        public override int SeedPart {
            get {
                return 383313881;
            }
        }
        protected override bool CanScatterAt(IntVec3 c, Map map) {
            return base.CanScatterAt(c, map) && c.Standable(map);
        }

        protected override void ScatterAt(IntVec3 loc, Map map, int count = 1) {
            CellRect rectToDefend;
            if (!MapGenerator.TryGetVar<CellRect>("RectOfInterest", out rectToDefend)) {
                rectToDefend = CellRect.CenteredOn(map.Center, 10);
            }
            Faction faction;
            if (map.ParentFaction == null || map.ParentFaction == Faction.OfPlayer) {
                faction = Find.FactionManager.RandomEnemyFaction(false, false, true, TechLevel.Undefined);
            }
            else {
                faction = map.ParentFaction;
            }
            ResolveParams resolveParamsPawn = default;
            resolveParamsPawn.rect = rectToDefend;
            resolveParamsPawn.faction = faction;
            TraverseParms traverseParms = TraverseParms.For(TraverseMode.PassDoors, Danger.Deadly, false);
            resolveParamsPawn.singlePawnLord = LordMaker.MakeNewLord(faction, new LordJob_DefendBase(faction, rectToDefend.CenterCell), map, null);
            resolveParamsPawn.pawnGroupKindDef = faction.def.pawnGroupMakers.FirstOrDefault(x => x.kindDef.defName == "Trader").kindDef ?? PawnGroupKindDefOf.Trader;
            resolveParamsPawn.pawnGroupMakerParams = new PawnGroupMakerParms();
            resolveParamsPawn.pawnGroupMakerParams.tile = map.Tile;
            resolveParamsPawn.pawnGroupMakerParams.faction = faction;
            PawnGroupMakerParms parm = resolveParamsPawn.pawnGroupMakerParams;
            float? settlementPawnGroupPoints = resolveParamsPawn.settlementPawnGroupPoints;
            parm.points = ((!settlementPawnGroupPoints.HasValue) ? SymbolResolver_Settlement.DefaultPawnsPoints.RandomInRange : settlementPawnGroupPoints.Value);
            resolveParamsPawn.pawnGroupMakerParams.inhabitants = true;
            resolveParamsPawn.pawnGroupMakerParams.seed = resolveParamsPawn.settlementPawnGroupSeed;
            BaseGen.symbolStack.Push("pawnGroup", resolveParamsPawn);
            BaseGen.globalSettings.map = map;
            BaseGen.Generate();
            MapGenerator.rootsToUnfog.Add(loc);
        }
    }
}
