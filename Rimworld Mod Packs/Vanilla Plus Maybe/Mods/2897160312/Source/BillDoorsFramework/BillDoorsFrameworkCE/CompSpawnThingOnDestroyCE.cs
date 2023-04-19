using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using Verse;
using CombatExtended;

namespace BillDoorsFramework
{
    public class CompSpawnThingOnDestroyCE : CompSpawnThingOnDestroy
    {
        protected override Faction GetFaction()
        {
            if (parent is ProjectileCE projectile)
            {
                return projectile.launcher.Faction;
            }
            return base.GetFaction();
        }
    }
}
