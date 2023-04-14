using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using Verse.AI;
using Verse.Grammar;

namespace Rimdeed
{
	public class IncidentWorker_ComplaintResponse : IncidentWorker
	{
		public override bool TryExecuteWorker(IncidentParms parms)
		{
			string description = RimdeedTools.GenerateTextFromRule(RD_DefOf.RD_ComplaintResponses);
			ChoiceLetter let = LetterMaker.MakeLetter(def.letterLabel, description, def.letterDef, null, null);
			Find.LetterStack.ReceiveLetter(let);
			return true;
		}
    }
}
