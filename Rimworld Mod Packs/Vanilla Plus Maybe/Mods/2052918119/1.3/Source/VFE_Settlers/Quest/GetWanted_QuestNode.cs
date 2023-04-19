using RimWorld;
using RimWorld.Planet;
using RimWorld.QuestGen;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace VFE_Settlers.Quest
{
    public class GetWanted_QuestNode : QuestNode
    {
        [NoTranslate]
        public SlateRef<string> storeAs;
        protected override void RunInt()
        {
			Slate slate = QuestGen.slate;
			if (!QuestGen.slate.TryGet(storeAs.GetValue(slate), out Pawn var) || !IsBadPawn(var, slate))
			{
				IEnumerable<Pawn> source = ExistingUsablePawns(slate);
				int num = source.Count();
				var = ((!Rand.Chance(canGeneratePawn.GetValue(slate) ? Mathf.Clamp01(1f - (float)num / (float)maxUsablePawnsToGenerate.GetValue(slate)) : 0f) || (!mustHaveNoFaction.GetValue(slate) && !TryFindFactionForPawnGeneration(slate, out Faction _))) ? source.RandomElement() : GeneratePawn(slate));
				if (var.Faction != null && !var.Faction.def.hidden)
				{
					QuestPart_InvolvedFactions questPart_InvolvedFactions = new QuestPart_InvolvedFactions();
					questPart_InvolvedFactions.factions.Add(var.Faction);
					QuestGen.quest.AddPart(questPart_InvolvedFactions);
				}
				QuestGen.slate.Set(storeAs.GetValue(slate), var);
			}
		}

		private bool IsBadPawn(Pawn pawn, Slate slate)
		{
			Faction faction = pawn.Faction; 
			if (faction == null || !faction.def.humanlikeFaction || faction.defeated || faction.def.hidden || faction.IsPlayer || pawn.IsPrisoner || faction.HostileTo(Faction.OfPlayer))
			{
				return false;
			}
			if (pawn.IsWorldPawn() && Find.WorldPawns.GetSituation(pawn) == WorldPawnSituation.ReservedByQuest)
			{
				return false;
			}
		}

		protected override bool TestRunInt(Slate slate)
        {
            throw new System.NotImplementedException();
        }
    }
}
