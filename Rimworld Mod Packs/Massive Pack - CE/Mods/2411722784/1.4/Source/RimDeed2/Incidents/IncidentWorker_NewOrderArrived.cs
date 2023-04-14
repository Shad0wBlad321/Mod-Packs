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
	public class IncidentWorker_NewOrderArrived : IncidentWorker
	{
		public override bool TryExecuteWorker(IncidentParms parms)
		{
			Map map = (Map)parms.target;
			ChoiceLetter let = LetterMaker.MakeLetter(def.letterLabel, this.def.letterText, def.letterDef, null, null);
			Find.LetterStack.ReceiveLetter(let);
			RimdeedManager.Instance.ReceiveNewOrder(let);
			return true;
		}
	}
}
