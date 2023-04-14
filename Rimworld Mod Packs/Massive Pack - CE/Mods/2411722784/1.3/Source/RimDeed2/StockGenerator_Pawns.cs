using HarmonyLib;
using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;
using Verse.AI;
using Verse.Grammar;

namespace Rimdeed
{
    public class StockGenerator_Pawns : StockGenerator
    {
        public override IEnumerable<Thing> GenerateThings(int forTile, Faction faction = null)
        {
            int count = countRange.RandomInRange;
            for (int i = 0; i < count; i++)
            {
                if (!Find.FactionManager.AllFactionsVisible.Where((Faction fac) => fac != Faction.OfPlayer && fac.def.humanlikeFaction && !fac.temporary).TryRandomElement(out Faction result))
                {
                    break;
                }
                PawnGenerationRequest request = PawnGenerationRequest.MakeDefault();
                request.KindDef = DefDatabase<PawnKindDef>.AllDefs.Where(x => x.RaceProps.Humanlike).RandomElement();
                request.Faction = result;
                request.Tile = forTile;
                request.ForceAddFreeWarmLayerIfNeeded = !trader.orbital;
                request.RedressValidator = ((Pawn x) => x.royalty == null || !x.royalty.AllTitlesForReading.Any());
                yield return PawnGenerator.GeneratePawn(request);
            }
        }

        public override bool HandlesThingDef(ThingDef thingDef)
        {
            if (thingDef.category == ThingCategory.Pawn && thingDef.race.Humanlike)
            {
                return true;
            }
            return false;
        }
    }
}