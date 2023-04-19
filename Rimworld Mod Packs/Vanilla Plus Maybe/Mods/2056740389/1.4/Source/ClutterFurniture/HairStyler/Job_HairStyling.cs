﻿using ApparelColorChange;
using System;
using System.Collections.Generic;
using Verse;
using Verse.AI;
namespace HairStyling
{
    public class Job_HairStyling : JobDriver
    {
        private const TargetIndex ColorChanger = TargetIndex.A;
        private const TargetIndex CellInd = TargetIndex.B;
        private static string ErrorMessage = "Hairstyling job called on building that is not <Hair styling bench>";
        protected override IEnumerable<Toil> MakeNewToils()
        {
            this.FailOnDestroyedOrNull(TargetIndex.A);
            this.FailOnDespawnedNullOrForbidden(TargetIndex.A);
            yield return Toils_Reserve.Reserve(TargetIndex.A);
            yield return Toils_Goto.GotoCell(TargetIndex.B, PathEndMode.OnCell);
            yield return Toils_WaitWithSoundAndEffect();
            yield break;
        }
        private Toil Toils_WaitWithSoundAndEffect()
        {
            return new Toil
            {
                initAction = delegate
                {
                    Hairstylering rainbowSquieerl = TargetA.Thing as Hairstylering;
                    if (rainbowSquieerl != null)
                    {
                        Hairstylering rainbowSquieerl2 = TargetA.Thing as Hairstylering;
                        if (GetActor().Position == TargetA.Thing.InteractionCell)
                        {
                            rainbowSquieerl2.HairStyler(GetActor());
                        }
                    }
                    else
                    {
                        Log.Error(ErrorMessage.Translate());
                    }
                },
                defaultCompleteMode = ToilCompleteMode.Instant
            };
        }
    }
}
