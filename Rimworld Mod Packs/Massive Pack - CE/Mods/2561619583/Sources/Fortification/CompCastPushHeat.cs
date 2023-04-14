using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace Fortification
{
    public class CompCastPushHeat : ThingComp
    {
		public CompProperties_CastPushHeat Props => (CompProperties_CastPushHeat)this.props;
        public virtual float EnergyPerCast => Props.energyPerCast;
	}
}
