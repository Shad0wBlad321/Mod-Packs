using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;
using Verse.AI;

namespace Casevac
{
	[DefOf]
	public static class CP_DefOf
	{
		public static JobDef CP_CasevacRescue;
		public static ThoughtDef CP_RescuedMe;
		public static ThoughtDef CP_RescuedTogether;
	}
}
