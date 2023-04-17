using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;

namespace RitualRewards.Sinnamon_Ritual;

public class RitualAttachableOutcomeEffectWorker_Insects : RitualAttachableOutcomeEffectWorker
{
    private const float bestOutcome = 10f;

    private const float regularOutcome = 5f;

    public override void Apply(Dictionary<Pawn, int> totalPresence, LordJob_Ritual jobRitual, OutcomeChance outcome, out string extraOutcomeDesc, ref LookTargets letterLookTargets)
    {
        Map map = jobRitual.Map;
        IntVec3 intVec = FindRootTunnelLoc(map);
        if (intVec == IntVec3.Invalid)
        {
            extraOutcomeDesc = "Sinnamon_InsectNoExit".Translate();
            return;
        }

        bool flag = outcome.BestPositiveOutcome(jobRitual);
        int num = flag ? (int)bestOutcome : (int)regularOutcome;
        float num2 = flag ? bestOutcome : regularOutcome;
        List<PawnKindDef> possibleInsectList = GetPossibleInsectList(map);
        while (num > 0 && num2 > 0f)
        {
            num--;
            num2 -= SpawnInsects(intVec, map, possibleInsectList);
        }

        extraOutcomeDesc = def.letterInfoText;
        letterLookTargets = new LookTargets(intVec, map);
    }

    private static float SpawnInsects(IntVec3 intVec, Map map, List<PawnKindDef> insectList)
    {
        IntVec3 loc = CellFinder.RandomClosewalkCellNear(intVec, map, 3);
        PawnKindDef pawnKindDef = insectList.RandomElement();
        Pawn newThing = PawnGenerator.GeneratePawn(
            new PawnGenerationRequest(
                kind: pawnKindDef,
                canGeneratePawnRelations: false,
                allowGay: false,
                allowFood: false));
        _ = GenSpawn.Spawn(newThing, loc, map, Rot4.Random);
        return pawnKindDef.RaceProps.baseBodySize;
    }

    private static List<PawnKindDef> GetPossibleInsectList(Map map)
    {
        static bool Validator(List<CompProperties> compList)
        {
            foreach (CompProperties comp in compList)
            {
                Type compClass = comp.compClass;
                if (compClass.Name is "CompUntameable" or "CompFloating")
                    return false;
            }

            return true;
        }

        return DefDatabase<PawnKindDef>.AllDefs.Where((x) => x.RaceProps.Insect && !x.defName.StartsWith("VFEI_VatGrown") && x.RaceProps.wildness <= 0.8 && map.mapTemperature.SeasonAndOutdoorTemperatureAcceptableFor(x.race) && Validator(x.race.comps)).ToList();
    }

    private static IntVec3 FindRootTunnelLoc(Map map)
    {
        return InfestationCellFinder.TryFindCell(out IntVec3 cell, map)
            ? cell
            : RCellFinder.TryFindRandomCellNearTheCenterOfTheMapWith((x) => x.Standable(map) && !x.Fogged(map), map, out cell)
            ? cell
            : IntVec3.Invalid;
    }
}
