using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using static System.Net.Mime.MediaTypeNames;


namespace BEWH
{
    [HarmonyPatch(typeof(BodyPartDef), "GetMaxHealth")]
    public class GetMaxHealth_Patch
    {
        private static void Postfix(BodyPartDef __instance, Pawn pawn, ref float __result)
        {
            float amount = 1;
            if (pawn.genes != null)
            {
                if (pawn.genes.HasGene(BEWHDefOf.BEWH_SecondaryHeart))
                {
                    amount += 0.2f;
                }
                if (pawn.genes.HasGene(BEWHDefOf.BEWH_Ossmodula))
                {
                    amount += 0.4f;
                }
                if (pawn.genes.HasGene(BEWHDefOf.BEWH_Biscopea))
                {
                    amount += 0.3f;
                }
                if (pawn.genes.HasGene(BEWHDefOf.BEWH_Melanochrome))
                {
                    amount += 0.6f;
                }
                if (pawn.genes.HasGene(BEWHDefOf.BEWH_SinewCoil))
                {
                    amount += 0.5f;
                }
                if (pawn.genes.HasGene(BEWHDefOf.BEWH_NurgleMark))
                {
                    amount += 1.1f;
                }
                if (pawn.genes.HasGene(BEWHDefOf.BEWH_UndividedMark))
                {
                    amount += 0.25f;
                }
                if (pawn.genes.HasGene(BEWHDefOf.BEWH_DaemonHide))
                {
                    amount += 4.5f;
                }
                if (pawn.genes.HasGene(BEWHDefOf.BEWH_Custodes))
                {
                    amount += 8.5f;
                }
                if (pawn.genes.HasGene(BEWHDefOf.BEWH_Primarch))
                {
                    amount += 17.5f;
                }
            }
            __result *= amount;
        }
    }
}