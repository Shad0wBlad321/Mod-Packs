using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CombatExtended;

namespace BillDoorsFramework
{
    class ProjectileMissileCE : BulletCE
    {
        public override void Tick()
        {
            if (this.intendedTargetThing != null)
            {
                //Log.Message("dstint: " + destinationInt + ", dst: " + Destination + ", intended: " + intendedTargetThing.DrawPos + ", origin: " + this.origin);
                this.destinationInt.x = intendedTargetThing.DrawPos.x;
                this.destinationInt.y = intendedTargetThing.DrawPos.z;
                //Log.Message("actual speed: " + this.shotSpeed + ", def speed: " + this.def.projectile.speed);
            }
            base.Tick();
        }
    }
}
