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
	public static class CP_Utils
	{
		private static readonly Func<Pawn, bool, int> ticksMoveSpeed = (Func<Pawn, bool, int>)Delegate.CreateDelegate(typeof(Func<Pawn, bool, int>), 
			AccessTools.Method(typeof(Pawn), "TicksPerMove"));
		public static int TicksPerMove(this Pawn pawn, bool diagonal)
        {
			return (int)ticksMoveSpeed(pawn, diagonal);
		}
	}
}
