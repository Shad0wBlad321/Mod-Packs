using RimWorld;
using System.Text;
using Verse;

namespace VFE_Settlers.Hediffs
{
    public class HediffComp_Wanted : HediffComp
    {
        public Faction WantedBy = null;
        public int Reward = 0;

        public override void CompExposeData()
        {
            Scribe_References.Look(ref WantedBy, "WantedBy");
            Scribe_Values.Look(ref Reward, "Reward");
        }

        public override string CompLabelInBracketsExtra
        {
            get
            {
                StringBuilder stringBuilder = new StringBuilder();
                stringBuilder.Append("WantedHediff".Translate(WantedBy, Reward * 10));
                return stringBuilder.ToString();
            }
        }
    }

    public class HediffCompProperties_Wanted : HediffCompProperties
    {
        public HediffCompProperties_Wanted()
        {
            compClass = typeof(HediffComp_Wanted);
        }
    }
}