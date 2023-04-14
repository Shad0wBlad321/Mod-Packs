﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using JetBrains.Annotations;
using Verse;

namespace BillDoorsFramework
{
    [UsedImplicitly]
    [StaticConstructorOnStartup]
    public class PatchMain
    {
        static PatchMain()
        {
            var instance = new Harmony("BD_HarmonyPatchesCE");
            instance.PatchAll(Assembly.GetExecutingAssembly());
        }
    }
}
