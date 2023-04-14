using System.Collections.Generic;
using System.Text;
using Verse;
using RimWorld;

namespace BetterInfoCard
{
    class StatWorker_BodyPart : StatWorker
    {
        public override float GetValueUnfinalized(StatRequest req, bool applyPostProcess = true)
        {
            return GetBodyInfo(req).noMissingParts.Count;
        }

        private class BodyPartInfo
        {
            public Pawn pawn;
            public HashSet<BodyPartRecord> noMissingParts;
            public List<BodyPartRecord> allDefPart;
        }


        public override string GetExplanationUnfinalized(StatRequest req, ToStringNumberSense numberSense)
        {
            StringBuilder res = new StringBuilder();
            var bodyInfo = GetBodyInfo(req);
            foreach (var part in bodyInfo.allDefPart)
            {
                if(part.parent==null)
                {
                    res.AppendInNewLine(GetBodyPartStr(part, bodyInfo));
                }
                
            }
            return res.ToString();
        }

        private string GetBodyPartStr(BodyPartRecord part, BodyPartInfo bodyInfo, string indent="")
        {
            StringBuilder res = new StringBuilder();
            bool missing = !bodyInfo.noMissingParts.Contains(part);
            int maxHitPoint = (int)part.def.GetMaxHealth(bodyInfo.pawn);
            int currentHitPoint = (int)bodyInfo.pawn.health.hediffSet.GetPartHealth(part);
            
            string line = indent + $"    {part.Label} ( {"HitPointsBasic".Translate()}：{currentHitPoint}/{maxHitPoint}，{"coverage".Translate()}：{part.coverage})";
            if (missing) line = line.Colorize(UnityEngine.Color.grey);
            res.Append(line);
            foreach (var child in part.GetDirectChildParts())
            {
                res.AppendInNewLine(GetBodyPartStr(child, bodyInfo, indent + "  "));
            }
            return res.ToString();
        }

        public override string GetStatDrawEntryLabel(StatDef stat, float value, ToStringNumberSense numberSense, StatRequest optionalReq, bool finalized = true)
        {
            return GetBodyInfo(optionalReq).noMissingParts.Count.ToString();
        }

        public override bool ShouldShowFor(StatRequest req)
        {
            if (base.ShouldShowFor(req))
            {
                return req.Thing is Pawn;
            }
            return false;
        }

        private BodyPartInfo GetBodyInfo(StatRequest optionalReq)
        {
            Pawn pawn = optionalReq.Thing as Pawn;
            List<BodyPartRecord> defParts = new List<BodyPartRecord>();
            foreach (var reocrd in pawn.def.race.body.AllParts) defParts.Add(reocrd);

            HashSet<BodyPartRecord> noMissingPart = new HashSet<BodyPartRecord>();
            foreach (var reocrd in pawn.health.hediffSet.GetNotMissingParts()) noMissingPart.Add(reocrd);
            return new BodyPartInfo() { pawn = pawn, allDefPart = defParts, noMissingParts = noMissingPart };
        }


    }
}
