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
	[DefOf]
	public static class RD_DefOf
	{
		public static JobDef RD_UseRimDeed;
		public static IncidentDef RD_NewOrderArrived;
		public static RulePackDef RD_Greetings;
		public static IncidentDef RD_NewRecruiterArrived;
		public static RulePackDef RD_ComplaintResponses;
		public static IncidentDef RD_ComplaintResponse;
		public static ThoughtDef RD_IGotJob;
		public static RulePackDef RD_BanDescription;
		public static TraderKindDef RD_PawnTrader;
		public static ThoughtDef RD_TradedInFromRimdeed;
		public static ThoughtDef RD_TradedInToRimdeed;

	}
}
