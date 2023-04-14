using RimWorld;
using System.Collections.Generic;
using System.Linq;
using Verse;

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
                var kind = DefDatabase<PawnKindDef>.AllDefs.Where(x => x.RaceProps.Humanlike).RandomElement();
                PawnGenerationRequest request = new PawnGenerationRequest(kind, result)
                {
                    Tile = forTile,
                    ForceAddFreeWarmLayerIfNeeded = !trader.orbital,
                    RedressValidator = (Pawn x) => x.royalty == null || !x.royalty.AllTitlesForReading.Any()
                };
                yield return PawnGenerator.GeneratePawn(request);
            }
        }

        public override bool HandlesThingDef(ThingDef thingDef)
        {
            return thingDef.category == ThingCategory.Pawn && thingDef.race.Humanlike;
        }
    }
}