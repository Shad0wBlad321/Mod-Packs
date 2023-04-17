using System.Collections.Generic;
using System.Text;
using Verse;
using RimWorld;

namespace BetterInfoCard
{
    class StatWorker_AttackTool : StatWorker
    {


        public override float GetValueUnfinalized(StatRequest req, bool applyPostProcess = true)
        {
            return GetAllTools(req).Count;
        }


        public override string GetExplanationUnfinalized(StatRequest req, ToStringNumberSense numberSense)
        {
            StringBuilder res = new StringBuilder();
            foreach (var tool in GetAllTools(req))
            {
                res.AppendLine($"{tool.label}");
                res.AppendLine($"  {"DamageLower".Translate()}：{tool.power}");
                res.AppendLine($"  {"chance".Translate()}：{tool.chanceFactor}");
                if(tool.linkedBodyPartsGroup!=null)
                {
                    res.AppendLine($"  {"body_part".Translate()}：{tool.linkedBodyPartsGroup.label}");
                }
            }
            return res.ToString();
            

        }

        public override string GetStatDrawEntryLabel(StatDef stat, float value, ToStringNumberSense numberSense, StatRequest optionalReq, bool finalized = true)
        {
            return GetAllTools(optionalReq).Count.ToString();
        }

        public override bool ShouldShowFor(StatRequest req)
        {
            if (base.ShouldShowFor(req))
            {
                return req.Thing is Pawn;
            }
            return false;
        }


        private List<Tool> GetAllTools(StatRequest optionalReq)
        {
            List<Tool> res = new List<Tool>();
            Pawn pawn = optionalReq.Thing as Pawn;
            if (pawn?.Tools == null) return res;
            res.AddRange(pawn.Tools);
            return res;
        }
    }
}
