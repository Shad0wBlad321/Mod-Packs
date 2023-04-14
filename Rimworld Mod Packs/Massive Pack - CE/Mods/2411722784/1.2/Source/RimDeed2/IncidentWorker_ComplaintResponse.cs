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
			string rootKeyword = RD_DefOf.RD_ComplaintResponses.RulesPlusIncludes.Where(x => x.keyword == "r_logentry").RandomElement().keyword;
			GrammarRequest request = default(GrammarRequest);
			request.Includes.Add(RD_DefOf.RD_ComplaintResponses);
			string description = GrammarResolver.Resolve(rootKeyword, request);

			ChoiceLetter let = LetterMaker.MakeLetter(def.letterLabel, description, def.letterDef, null, null);
			Find.LetterStack.ReceiveLetter(let);
			return true;
		}
    }
}
