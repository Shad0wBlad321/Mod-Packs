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
	[StaticConstructorOnStartup]
	internal static class HarmonyInit
	{
		static HarmonyInit()
		{
			new Harmony("ChickenPlucker.RimDeed").PatchAll();
		}
	}
	[HarmonyPatch(typeof(Building_CommsConsole), "GetFloatMenuOptions")]
	internal static class Building_CommsConsole_Patch
    {
		private static IEnumerable<FloatMenuOption> Postfix(IEnumerable<FloatMenuOption> __result, Building_CommsConsole __instance, Pawn myPawn)
        {
			var list = __result.ToList();
			foreach (var floatOption in __result)
			{
				yield return floatOption;
			}
			if (list.Count() == 1 && list[0].action is null)
            {
				yield break;
            }
            else
            {
				string text = "RimDeed.OpenRimdeed".Translate();
				var floatOption = new FloatMenuOption(text, delegate
				{
					Job job = JobMaker.MakeJob(RD_DefOf.RD_UseRimDeed, __instance);
					myPawn.jobs.TryTakeOrderedJob(job);
				}, MenuOptionPriority.InitiateSocial);

				yield return FloatMenuUtility.DecoratePrioritizedTask(floatOption, myPawn, __instance);
			}
		}
	}

	[HarmonyPatch(typeof(ChoiceLetter), "OpenLetter")]
	internal static class OpenLetter_Patch
	{
		private static bool Prefix(Letter __instance)
		{
			Log.Message("Opened: " + __instance);
			if (RimdeedManager.Instance.TryOpenNewLetter(__instance))
            {
				return false;
            }
			return true;
		}
	}

	[HarmonyPatch(typeof(Pawn), "Kill")]
	internal static class Pawn_Kill_Patch
	{
		private static void Postfix(Pawn __instance)
		{
			try
            {
				var manager = RimdeedManager.Instance;
				if (manager?.hiredPawns != null && manager.hiredPawns.TryGetValue(__instance, out int hiredTick))
				{
					if (hiredTick + (3 * GenDate.TicksPerDay) > Find.TickManager.TicksGame)
					{
						if (manager.BannedUntil > Find.TickManager.TicksGame)
						{
							manager.BannedUntil += GenDate.TicksPerDay * 10;
						}
						else
						{
							manager.BannedUntil = Find.TickManager.TicksGame + (GenDate.TicksPerDay * 10);
						}
						var banPeriod = manager.BannedUntil - Find.TickManager.TicksGame;
						var banDescription = RimdeedTools.GenerateTextFromRule(RD_DefOf.RD_BanDescription);
						Find.LetterStack.ReceiveLetter("RimDeed.Banhammer".Translate(), "RimDeed.BanhammerText".Translate(banPeriod.ToStringTicksToDays(), banDescription), LetterDefOf.NegativeEvent);
					}
				}
			}
			catch { };
		}
	}
}
