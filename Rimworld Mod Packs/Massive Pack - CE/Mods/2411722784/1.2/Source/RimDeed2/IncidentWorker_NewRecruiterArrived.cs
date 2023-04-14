using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using Verse.AI;

namespace Rimdeed
{
	public class IncidentWorker_NewRecruiterArrived : IncidentWorker
	{
		public override bool TryExecuteWorker(IncidentParms parms)
		{
			Map map = (Map)parms.target;
			if (Find.World.GetComponent<RimdeedManager>().TryReceiveNewRecruiter(map, out Pawn newApplicant))
            {
				ChoiceLetter let = LetterMaker.MakeLetter(def.letterLabel, this.def.letterText, def.letterDef, newApplicant);
				Find.LetterStack.ReceiveLetter(let);
				return true;
			}
			return false;
		}
	}
}
