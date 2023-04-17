using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;

namespace RitualRewards.Sinnamon_Ritual;

public class RitualAttachableOutcomeEffectWorker_CallVenerated : RitualAttachableOutcomeEffectWorker
{
    private static Pawn SpawnAnimal(PawnKindDef kind, Gender? gender, IntVec3 loc, Map map)
    {
        Pawn pawn = PawnGenerator.GeneratePawn(new PawnGenerationRequest(
            kind: kind,
            canGeneratePawnRelations: false,
            allowGay: false,
            allowFood: false,
            fixedGender: gender));
        _ = GenSpawn.Spawn(pawn, loc, map, Rot4.Random);
        return pawn;
    }

    private float SelectionChance(PawnKindDef k)
    {
        return 1f / k.race.BaseMarketValue;
    }

    public override void Apply(Dictionary<Pawn, int> totalPresence, LordJob_Ritual jobRitual, OutcomeChance outcome, out string extraOutcomeDesc, ref LookTargets letterLookTargets)
    {
        extraOutcomeDesc = "";
        bool flag = outcome.BestPositiveOutcome(jobRitual);
        if (!flag && Rand.Chance(0.7f))
            return;

        Map map = jobRitual.Map;
        if (RCellFinder.TryFindRandomPawnEntryCell(out IntVec3 result, map, CellFinder.EdgeRoadChance_Animal))
        {
            if (jobRitual.Ritual.ideo.VeneratedAnimals.Count == 0)
                return;

            IEnumerable<PawnKindDef> source = DefDatabase<PawnKindDef>.AllDefs.Where((x) => jobRitual.Ritual.ideo.VeneratedAnimals.Contains(x.race));
            PawnKindDef pawnKindDef;
            if (!flag)
            {
                pawnKindDef = source.Where((x) => map.mapTemperature.SeasonAndOutdoorTemperatureAcceptableFor(x.race)).RandomElementByWeight(SelectionChance);
                if (pawnKindDef == null)
                {
                    extraOutcomeDesc = def.letterInfoText + "Sinnamon_VeneratedAnimalFailTemperature".Translate(outcome.label);
                    return;
                }
            }

            pawnKindDef = DefDatabase<PawnKindDef>.AllDefs.Where((x) => jobRitual.Ritual.ideo.VeneratedAnimals.Contains(x.race)).RandomElementByWeight(SelectionChance);
            if (flag)
            {
                if (map.mapTemperature.SeasonAndOutdoorTemperatureAcceptableFor(pawnKindDef.race))
                {
                    extraOutcomeDesc = "Sinnamon_VeneratedAnimalCalledPair".Translate(pawnKindDef.label);
                    letterLookTargets.targets.Add(SpawnAnimal(pawnKindDef, Gender.Male, result, map));
                    letterLookTargets.targets.Add(SpawnAnimal(pawnKindDef, Gender.Female, result, map));
                }
                else
                {
                    extraOutcomeDesc = "Sinnamon_VeneratedAnimalCalledBadWeather".Translate(pawnKindDef.label);
                    letterLookTargets.targets.Add(SpawnAnimal(pawnKindDef, null, result, map));
                }
            }
            else
            {
                letterLookTargets.targets.Add(SpawnAnimal(pawnKindDef, null, result, map));
                extraOutcomeDesc = "Sinnamon_VeneratedAnimalCalled".Translate(pawnKindDef.label);
            }
        }
        else
        {
            extraOutcomeDesc = def.letterInfoText + "Sinnamon_VeneratedAnimalFailNoEntry".Translate();
        }
    }
}
