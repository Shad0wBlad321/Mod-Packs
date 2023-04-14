﻿using System.Collections.Generic;
using System.Text;
using PipeSystem;
using RimWorld;
using UnityEngine;
using Verse;

namespace VNPE
{
    [StaticConstructorOnStartup]
    public class CompRegisterIngredients : CompIngredients
    {
        private static readonly Texture2D transferIcon = ContentFinder<Texture2D>.Get("UI/TransferStorageContent");
        private CompResourceStorage compResourceStorage;

        public override IEnumerable<Gizmo> CompGetGizmosExtra()
        {
            foreach (var gizmo in base.CompGetGizmosExtra())
                yield return gizmo;

            yield return new Command_Action
            {
                action = () =>
                {
                    ingredients.Clear();
                    compResourceStorage.Empty();
                },
                defaultLabel = "VNPE_Drain".Translate(),
                defaultDesc = "VNPE_DrainDesc".Translate(),
                icon = transferIcon,
            };
        }

        public override string CompInspectStringExtra()
        {
            var stringBuilder = new StringBuilder();
            if (ingredients.Count > 0)
            {
                stringBuilder.Append("Ingredients".Translate() + ": ");
                stringBuilder.Append(GetIngredientsString(false, out var _));
            }
            return stringBuilder.ToString();
        }

        public override void CompTickRare()
        {
            base.CompTickRare();

            if (compResourceStorage.AmountStored == 0)
                ingredients.Clear();
        }

        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            base.PostSpawnSetup(respawningAfterLoad);
            compResourceStorage = parent.GetComp<CompResourceStorage>();
        }
    }
}