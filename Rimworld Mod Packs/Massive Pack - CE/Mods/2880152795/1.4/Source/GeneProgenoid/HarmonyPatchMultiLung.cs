using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;
using static System.Net.Mime.MediaTypeNames;


namespace BEWH
{
    [HarmonyPatch(typeof(PawnCapacityWorker_Breathing), "CalculateCapacityLevel")]
    public class MultiLungBreathingSourcePatch
    {
        public static float Postfix(float originalResult, HediffSet __0) 
        {
            HediffSet hediffSet = __0;

            Pawn pawn = hediffSet.pawn;
            if (pawn.DestroyedOrNull() || pawn.genes == null || pawn.Dead)
            {
                return originalResult;
            }
            if (pawn.genes.HasGene(BEWHDefOf.BEWH_MultiLung))
            {
                float result = originalResult + 0.6f;
                return result;
            }
            return originalResult;
        }
    }
}