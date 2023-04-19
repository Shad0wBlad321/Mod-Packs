using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace BillDoorsFramework
{
    public class PlaceWorker_ShowVerbRadius : PlaceWorker
    {
        public override AcceptanceReport AllowsPlacing(BuildableDef checkingDef, IntVec3 loc, Rot4 rot, Map map, Thing thingToIgnore = null, Thing thing = null)
        {
            foreach (VerbProperties verbProperties in ((ThingDef)checkingDef).building.turretGunDef.Verbs)
            {
                if (verbProperties.range > 0f)
                {
                    GenDraw.DrawRadiusRing(loc, verbProperties.range);
                }
                if (verbProperties.minRange > 0f)
                {
                    GenDraw.DrawRadiusRing(loc, verbProperties.minRange);
                }
            }
            return true;
        }
    }

    public class PlaceWorker_ShowVerbRadiusBySight : PlaceWorker
    {
        public override AcceptanceReport AllowsPlacing(BuildableDef checkingDef, IntVec3 loc, Rot4 rot, Map map, Thing thingToIgnore = null, Thing thing = null)
        {
            foreach (VerbProperties verbProperties in ((ThingDef)checkingDef).building.turretGunDef.Verbs)
            {
                if (verbProperties.range > 0f)
                {
                    GenDraw.DrawRadiusRing(loc, verbProperties.range, Color.white, (IntVec3 x) => GenSight.LineOfSight(loc, x, map));
                }
                if (verbProperties.minRange > 0f)
                {
                    GenDraw.DrawRadiusRing(loc, verbProperties.minRange, Color.white, (IntVec3 x) => GenSight.LineOfSight(loc, x, map));
                }
            }
            return true;
        }
    }
}
