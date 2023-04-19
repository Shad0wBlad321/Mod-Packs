using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using UnityEngine;
using Verse;
using static HarmonyLib.Code;

namespace BillDoorsFramework
{
    public class CompEquippedGizmo : ThingComp
    {
        public virtual IEnumerable<Gizmo> CompGetGizmosEquipped()
        {
            yield return null;
        }
    }
}
