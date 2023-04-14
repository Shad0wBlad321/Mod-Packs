using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using Verse;

namespace BillDoorsLootsAndShelves
{
    public class StatPart_DisplayShelf : StatPart
    {
        public bool nullifyNegativeStat;

        public float statFromWealthMult;

        float GetStat(Building_Locker locker)
        {
            float f = locker.tempStorage[0].GetStatValue(parentStat);
            if (f < 0 && nullifyNegativeStat) { f = 0; }
            return f;
        }

        float GetStatFromWealth(Building_Locker locker)
        {
            float f = locker.tempStorage[0].GetStatValue(RimWorld.StatDefOf.MarketValue);
            if (statFromWealthMult > 0) { f *= statFromWealthMult * locker.tempStorage[0].stackCount; }
            return f;
        }

        public override string ExplanationPart(StatRequest req)
        {
            Log.Message("1");
            if (req.HasThing && req.Thing is Building_Locker locker && locker.isDisplay && locker.tempStorage.Any)
            {
                return "BDsStatPart_DisplayShelf".Translate(locker.tempStorage[0].LabelNoParenthesis, GetStat(locker).ToString(), GetStatFromWealth(locker).ToString());
            }
            Log.Message("2");
            return "";
        }

        public override void TransformValue(StatRequest req, ref float val)
        {
            Log.Message("3");
            if (req.HasThing && req.Thing is Building_Locker locker && locker.isDisplay && locker.tempStorage.Any)
            {
                val += GetStat(locker) + GetStatFromWealth(locker);
            }
            Log.Message("4");
        }
    }
}
