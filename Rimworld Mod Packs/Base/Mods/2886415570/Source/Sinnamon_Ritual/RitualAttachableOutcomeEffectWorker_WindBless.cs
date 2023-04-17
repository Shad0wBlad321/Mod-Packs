using System;
using System.Collections.Generic;
using RimWorld;
using Verse;

namespace RitualRewards.Sinnamon_Ritual;

public class RitualAttachableOutcomeEffectWorker_WindBless : RitualAttachableOutcomeEffectWorker
{
    public override void Apply(Dictionary<Pawn, int> totalPresence, LordJob_Ritual jobRitual, OutcomeChance outcome, out string extraOutcomeDesc, ref LookTargets letterLookTargets)
    {
        HashSet<string> reducingGC = new()
            { "HeatWave", "GlobalWarming" };
        List<GameCondition> list = new();
        jobRitual.Map.GameConditionManager.GetAllGameConditionsAffectingMap(jobRitual.Map, list);
        int divider = outcome.BestPositiveOutcome(jobRitual) ? 4 : 2;
        string text = Utilities.ReduceGameCondition(list, reducingGC, null, divider, "WindBlessAffectedEvents", string.Empty);
        GameCondition_ForceWeather gameCondition_ForceWeather = (GameCondition_ForceWeather)Activator.CreateInstance(typeof(GameCondition_ForceWeather));
        gameCondition_ForceWeather.startTick = Find.TickManager.TicksGame;
        gameCondition_ForceWeather.def = GameConditionDef.Named("Sinnamon_WindBlessing");
        gameCondition_ForceWeather.Duration = outcome.BestPositiveOutcome(jobRitual) ? 120000 : 60000;
        gameCondition_ForceWeather.uniqueID = Find.UniqueIDsManager.GetNextGameConditionID();
        gameCondition_ForceWeather.weather = WeatherDef.Named("Sinnamon_Windy");
        jobRitual.Map.GameConditionManager.RegisterCondition(gameCondition_ForceWeather);
        extraOutcomeDesc = def.letterInfoText + text;
    }
}
