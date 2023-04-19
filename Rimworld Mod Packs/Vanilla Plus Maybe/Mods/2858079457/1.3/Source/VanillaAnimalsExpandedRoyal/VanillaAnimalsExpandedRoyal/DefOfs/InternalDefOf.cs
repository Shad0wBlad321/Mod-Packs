using System;
using RimWorld;
using Verse;
using System.Collections.Generic;

namespace VanillaAnimalsExpandedRoyal
{
	[DefOf]
	public static class InternalDefOf
	{
		static InternalDefOf()
		{
			DefOfHelper.EnsureInitializedInCtor(typeof(InternalDefOf));
		}

		public static ThingDef VAERoy_Crane;
		public static ThingDef VAERoy_Peacock;
		public static ThingDef VAERoy_Quail;
		public static ThingDef VAERoy_Orangutan;

		public static ThingDef VAERoy_QuailMeat;
		public static ThingDef MealLavish;
		public static ThingDef MealSimple;
		public static ThingDef MealFine;
		public static ThingDef MealFine_Meat;
		public static ThingDef MealLavish_Meat;

		public static JobDef VAERoy_PeacockNuzzle;
		public static JobDef VAERoy_OrangutanTalk;

		public static InteractionDef VAERoy_PeacockInteraction;
		public static InteractionDef VAERoy_OrangutanInteraction;

		public static HediffDef VAERoy_PsychicSensitivity;

		public static PawnRelationDef VAERoy_TutorRelation;


	}
}
