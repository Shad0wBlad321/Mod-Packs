using System;
using Verse;
using Verse.AI;
using RimWorld;
using System.Collections.Generic;
using UnityEngine;
using HarmonyLib;
using System.Linq;

namespace VanillaAnimalsExpandedEndangered
{
	[StaticConstructorOnStartup]
	internal static class HarmonyInit
	{
		static HarmonyInit()
		{
			new Harmony("VanillaAnimalsExpandedEndangered.HarmonyInit").PatchAll();
		}
	}

	[HarmonyPatch(typeof(Pawn), "GetGizmos")]
	public class Pawn_GetGizmos_Patch
	{

		public static List<PawnKindDef> animalList {
            get
            {
				List<PawnKindDef> animalListResult = new List<PawnKindDef>();
				List<ReleaseableAnimalsDef> allReleaseableLists =  DefDatabase<ReleaseableAnimalsDef>.AllDefsListForReading;
                foreach (ReleaseableAnimalsDef individualList in allReleaseableLists) {
					animalListResult.AddRange(individualList.releaseablePawns);


				}
				return animalListResult;
			}
		
		}
			
			
			

		public static HashSet<PawnKindDef> animalsToBanish = animalList.ToHashSet();
		
		/*new HashSet<PawnKindDef>
		{
			animalList
			DefDatabase<PawnKindDef>.GetNamed("AEXP_AfricanWildDog"),
			DefDatabase<PawnKindDef>.GetNamed("AEXP_BlackFootedFerret"),
			DefDatabase<PawnKindDef>.GetNamed("AEXP_BlackRhino"),
			DefDatabase<PawnKindDef>.GetNamed("AEXP_Bonobo"),
			DefDatabase<PawnKindDef>.GetNamed("AEXP_Moa"),
			DefDatabase<PawnKindDef>.GetNamed("AEXP_Panda"),
			DefDatabase<PawnKindDef>.GetNamed("AEXP_Pangolin"),
			DefDatabase<PawnKindDef>.GetNamed("AEXP_Quagga"),
			DefDatabase<PawnKindDef>.GetNamed("AEXP_RockhopperPenguin"),
			DefDatabase<PawnKindDef>.GetNamed("AEXP_TasmanianDevil"),
			DefDatabase<PawnKindDef>.GetNamed("AEXP_Thylacine")
		};*/
		public static void Postfix(ref IEnumerable<Gizmo> __result, Pawn __instance)
		{
			if (__instance.Faction == Faction.OfPlayer && (animalsToBanish.Contains(__instance.kindDef) || (VanillaAnimalsExpandedEndangeredMod.settings.addBanishToAllAnimals&& __instance.kindDef.RaceProps.Animal&& !__instance.kindDef.RaceProps.FenceBlocked&& !__instance.Downed && !__instance.Dead)))
			{
				
				List<Gizmo> list = __result.ToList<Gizmo>();
				Command_Action command_Action = new Command_Action();
				command_Action.defaultLabel = "VAEE.ReleaseToRepopulate".Translate();
				command_Action.icon = ContentFinder<Texture2D>.Get("UI/ReleaseToRepopulate");
				command_Action.defaultDesc = "VAEE.ReleaseToRepopulateDesc".Translate();
				command_Action.action = delegate
				{
					Find.WindowStack.Add(new Dialog_MessageBox("VAEE.ReleasingToTheWildConfirmationRepopulate".Translate(), "Confirm".Translate(), delegate
					{
						if (RCellFinder.TryFindRandomExitSpot(__instance, out IntVec3 spot, TraverseMode.PassDoors))
						{
							__instance.SetFaction(null);
							Job job = JobMaker.MakeJob(DefDatabase<JobDef>.GetNamed("AEXP_GotoTheWild"), spot);
							job.exitMapOnArrival = true;
							__instance.jobs.TryTakeOrderedJob(job);
						}
					}, "VAEE.Cancel".Translate()));
				};
				command_Action.hotKey = KeyBindingDefOf.Misc4;
				list.Add(command_Action);
				__result = list;
			}
		}
	}
}

