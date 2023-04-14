using Verse;
using RimWorld;
using System.Collections.Generic;

namespace Fortification
{
    public class CompProperties_ExpolsionWithEvents : CompProperties
    {
        public CompProperties_ExpolsionWithEvents()
        {
            this.compClass = typeof(CompExpolsionWithEvents);
        }
        public List<Condition> conditions;
    }
    public class Condition
    {
        public GameConditionDef conditionDef;
        public int percent;
        public IntRange duration;
    }

}
