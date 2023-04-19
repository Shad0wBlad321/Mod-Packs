using RimWorld;
using System.Linq;
using Verse;
using Verse.Grammar;
using VFE_Settlers.Hediffs;

namespace VFE_Settlers.Utilities
{
    internal static class UtilityPawn
    {
        public static bool GetWantedComponent(Pawn thing, out HediffComp_Wanted wanted)
        {
            wanted = thing.health.hediffSet.GetFirstHediffOfDef(Defs.HediffDefOf.Wanted).TryGetComp<HediffComp_Wanted>();
            return wanted != null;
        }

        public static bool GetWantedComponent(Corpse thing, out HediffComp_Wanted wanted)
        {
            wanted = thing.InnerPawn.health.hediffSet.GetFirstHediffOfDef(Defs.HediffDefOf.Wanted).TryGetComp<HediffComp_Wanted>();
            return wanted != null;
        }

        public static Pawn GenerateWanted(Faction alliedFaction, Faction enemyFaction, int reward, out HediffComp_Wanted wan)
        {
            PawnKindDef result = null;
            result = enemyFaction.RandomPawnKind();
            if (result == null)
            {
                result = DefDatabase<PawnKindDef>.AllDefsListForReading.Where((PawnKindDef kind) => kind.race.race.Humanlike).RandomElement();
            }
            Pawn newCriminal = PawnGenerator.GeneratePawn(new PawnGenerationRequest(result, enemyFaction, PawnGenerationContext.NonPlayer, -1, true));

            Hediff wanted = HediffMaker.MakeHediff(Defs.HediffDefOf.Wanted, newCriminal);
            wan = wanted.TryGetComp<HediffComp_Wanted>();
            wan.WantedBy = alliedFaction;
            wan.Reward = reward;
            newCriminal.health.AddHediff(wanted);

            NameTriple name = newCriminal.Name as NameTriple;
            string firstName = name.First;
            string lastName = name.Last;
            GrammarRequest request = default;
            request.Includes.Add(Defs.RulePackDefOf.NamerWantedNickname);
            string nickName = GrammarResolver.Resolve("defaultWantedNick", request);
            newCriminal.Name = new NameTriple(firstName, nickName, lastName);
            Find.WorldPawns.PassToWorld(newCriminal);

            return newCriminal;
        }
    }
}