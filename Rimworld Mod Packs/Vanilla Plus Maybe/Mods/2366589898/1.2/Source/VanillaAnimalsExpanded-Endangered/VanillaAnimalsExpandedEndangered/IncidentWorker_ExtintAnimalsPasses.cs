using System;
using Verse;
using Verse.AI;
using RimWorld;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace VanillaAnimalsExpandedEndangered
{
	public class IncidentWorker_ExtintAnimalsPasses : IncidentWorker
	{
		protected override bool CanFireNowSub(IncidentParms parms)
		{
			Map map = (Map)parms.target;
			if (map.gameConditionManager.ConditionIsActive(GameConditionDefOf.ToxicFallout))
			{
				return false;
			}
			IntVec3 cell;
			return TryFindEntryCell(map, out cell);
		}

		public static HashSet<PawnKindDef> extintPawnKinds = new HashSet<PawnKindDef>
		{
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
		};

		protected override bool TryExecuteWorker(IncidentParms parms)
		{
			Map map = (Map)parms.target;
			if (!TryFindEntryCell(map, out IntVec3 cell))
			{
				return false;
			}
			var candidates = extintPawnKinds.Where(x => map.mapTemperature.SeasonAndOutdoorTemperatureAcceptableFor(x.race));
			if (!candidates.Any())
            {
				return false;
            }

			PawnKindDef animal = candidates.RandomElement();
			int num = Rand.RangeInclusive(90000, 150000);
			IntVec3 result = IntVec3.Invalid;
			if (!RCellFinder.TryFindRandomCellOutsideColonyNearTheCenterOfTheMap(cell, map, 10f, out result))
			{
				result = IntVec3.Invalid;
			}

			IntVec3 loc = CellFinder.RandomClosewalkCellNear(cell, map, 10);
			Pawn male = PawnGenerator.GeneratePawn(new PawnGenerationRequest(animal, fixedGender: Gender.Male));
			GenSpawn.Spawn(male, loc, map, Rot4.Random);

			loc = CellFinder.RandomClosewalkCellNear(cell, map, 10);
			Pawn female = PawnGenerator.GeneratePawn(new PawnGenerationRequest(animal, fixedGender: Gender.Female));
			GenSpawn.Spawn(female, loc, map, Rot4.Random);

			male.mindState.exitMapAfterTick = Find.TickManager.TicksGame + num;
			female.mindState.exitMapAfterTick = Find.TickManager.TicksGame + num;
			if (result.IsValid)
			{
				male.mindState.forcedGotoPosition = CellFinder.RandomClosewalkCellNear(result, map, 10);
				female.mindState.forcedGotoPosition = CellFinder.RandomClosewalkCellNear(result, map, 10);
			}
			SendStandardLetter("VAEE.LetterLabelExtintAnimalsPasses".Translate(animal.label).CapitalizeFirst(), "VAEE.LetterExtintAnimalsPasses".Translate(animal.label), 
				LetterDefOf.PositiveEvent, parms, male);
			return true;
		}

		private bool TryFindEntryCell(Map map, out IntVec3 cell)
		{
			return RCellFinder.TryFindRandomPawnEntryCell(out cell, map, CellFinder.EdgeRoadChance_Animal + 0.2f);
		}
	}
}

