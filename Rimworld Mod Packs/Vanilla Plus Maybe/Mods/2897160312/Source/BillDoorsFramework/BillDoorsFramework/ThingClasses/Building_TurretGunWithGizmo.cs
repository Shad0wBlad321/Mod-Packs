using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using Verse;

namespace BillDoorsFramework
{
    public class Building_TurretGunWithGizmo : Building_TurretGun
    {
        public override IEnumerable<Gizmo> GetGizmos()
        {
            foreach (Gizmo gizmo in base.GetGizmos())
            {
                yield return gizmo;
            }
            if (gun == null) yield break;
            foreach (var comp in (gun as ThingWithComps).AllComps)
            {
                foreach (var gizmo in comp.CompGetGizmosExtra())
                {
                    yield return gizmo;
                }
            }
        }
    }
}
