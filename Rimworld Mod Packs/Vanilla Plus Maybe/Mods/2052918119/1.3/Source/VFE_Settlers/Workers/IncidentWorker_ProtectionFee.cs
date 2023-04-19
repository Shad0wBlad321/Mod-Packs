using RimWorld;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace VFE_Settlers.Workers
{
    public class IncidentWorker_ProtectionFee : IncidentWorker_RaidEnemy
    {
        protected override bool CanFireNowSub(IncidentParms parms)
        {
            if (base.CanFireNowSub(parms) && parms.target != null)
            {
                Map map = (Map)parms.target;
                List<Map> maps = Find.Maps.Where(x => x.IsPlayerHome).ToList();
                if (map != null && maps.Contains(map))
                {
                    return true;
                }
            }
            return false;
        }

        protected override bool TryExecuteWorker(IncidentParms parms)
        {
            if (parms.target != null)
            {
                Map map = (Map)parms.target;
                if (map != null)
                {
                    return Utilities.UtilityEvent.ProtectionFee(map, parms);
                }
            }
            return false;
        }
    }
}