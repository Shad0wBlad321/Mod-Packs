using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace BillDoorsFramework
{
    internal class CompPawnEquipmentGizmo : ThingComp
    {
        public override IEnumerable<Gizmo> CompGetGizmosExtra()
        {
            ThingWithComps thingWithComps = ((parent is Pawn pawn) ? pawn.equipment.Primary : null);
            if (thingWithComps == null || thingWithComps.AllComps.NullOrEmpty())
            {
                yield break;
            }
            IEnumerable<CompEquippedGizmo> comps = thingWithComps.GetComps<CompEquippedGizmo>();
            foreach (CompEquippedGizmo allComp in comps)
            {
                foreach (Gizmo item in allComp.CompGetGizmosEquipped())
                {
                    yield return item;
                }
            }
        }
    }
}
