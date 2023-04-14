namespace MechanoidNightVision
{
	using System.Reflection;
	using HarmonyLib;
	using RimWorld;
	using Verse;

	[HarmonyPatch]
	internal class Patch_StatPart_Glow
	{
		static MethodBase TargetMethod() => typeof(StatPart_Glow).GetMethod("ActiveFor", BindingFlags.NonPublic | BindingFlags.Instance);

		[HarmonyPrefix]
		static bool Prefix(ref bool __result, bool ___humanlikeOnly, bool ___ignoreIfIncapableOfSight, bool ___ignoreIfPrefersDarkness, Thing t)
		{
			__result = false;

			if (t is Pawn pawn)
			{
				if (___humanlikeOnly && !pawn.RaceProps.Humanlike)
				{
					return false;
				}

				if (___ignoreIfIncapableOfSight && PawnUtility.IsBiologicallyOrArtificiallyBlind(pawn))
				{
					return false;
				}

				if (___ignoreIfPrefersDarkness)
				{
					if (pawn.Ideo != null && pawn.Ideo.IdeoPrefersDarkness())
					{
						return false;
					}

					if (pawn.genes != null && !pawn.genes.AffectedByDarkness)
					{
						return false;
					}
				}

				if (pawn.RaceProps.IsMechanoid)
				{
					return false;
				}
			}

			__result = t.Spawned;
			return false;
		}
	}
}
