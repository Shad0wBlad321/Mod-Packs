using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace BillDoorsFramework
{
    public class CompKeyCardUsedEffect : ThingComp
    {
        public virtual void UsedEffect(CompUsable_KeyCardRequirement comp) { }
    }
}
