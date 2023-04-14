using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace Fortification
{
    public class CompCompositeExplosion : ThingComp
    {
        public CompProperties_CompositeExplosion Props => (CompProperties_CompositeExplosion)this.props;
        public virtual List<CompositeExplosion> CompositeExplosions => Props.compositeExplosions;

    }
}
