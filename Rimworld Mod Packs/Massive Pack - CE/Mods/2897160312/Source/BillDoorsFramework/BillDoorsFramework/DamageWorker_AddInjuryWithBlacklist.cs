using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using Verse;

namespace BillDoorsFramework
{
    internal class DamageWorker_AddInjuryWithBlacklist : DamageWorker_AddInjury
    {
        ModExtension_DamageBlacklist extension => def.GetModExtension<ModExtension_DamageBlacklist>();

        protected override void ExplosionDamageThing(Explosion explosion, Thing t, List<Thing> damagedThings, List<Thing> ignoredThings, IntVec3 cell)
        {
            if (extension.ignoredDefs.Contains(t.def))
            {
                return;
            }
            base.ExplosionDamageThing(explosion, t, damagedThings, ignoredThings, cell);
        }
    }

    public class ModExtension_DamageBlacklist : DefModExtension
    {
        public List<ThingDef> ignoredDefs = new List<ThingDef>();
    }
}
