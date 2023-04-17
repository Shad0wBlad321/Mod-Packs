using System.Collections.Generic;
using RimWorld;
using Verse;

namespace RitualRewards.Sinnamon_Ritual;

public static class Utilities
{
    public static string ReduceGameCondition(List<GameCondition> currentConditions, HashSet<string> reducingGC, HashSet<string> endingGC, int divider, string reducingMessage, string endingMessage)
    {
        string text = "";
        foreach (GameCondition currentCondition in currentConditions)
        {
            if (reducingGC != null && reducingGC.Contains(currentCondition.def.defName))
            {
                text += reducingMessage.Translate(currentCondition.def.label);
                currentCondition.Duration /= divider;
            }

            if (endingGC != null && endingGC.Contains(currentCondition.def.defName))
            {
                text += endingMessage.Translate(currentCondition.def.label);
                currentCondition.End();
            }
        }

        return text;
    }
}
