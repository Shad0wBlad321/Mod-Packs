using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using Verse;

namespace GeneRipper
{
    public class WorkGiver_CarryToGeneRipper : WorkGiver_CarryToBuilding
    {
        public override ThingRequest ThingRequest => ThingRequest.ForDef(GeneRipper_DefOfs.GeneRipper);

        public override bool ShouldSkip(Pawn pawn, bool forced = false) => !ModsConfig.BiotechActive;
    }
    
}
