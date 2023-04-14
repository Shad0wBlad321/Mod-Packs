using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using RimWorld;
using Verse;

namespace BillDoorsFramework
{
    [StaticConstructorOnStartup]
    public class MapPawnsForMedEvac
    {
        [HarmonyPatch(typeof(MapPawns), "PlayerEjectablePodHolder")]
        static class PlayerEjectablePodHolder_PostFix
        {
            [HarmonyPostfix]
            static void PostFix(Thing thing, ref IThingHolder __result)
            {
                if (thing is FlyShipLeaving_MedEvac evac && evac.innerContainer.Any)
                {
                    Skyfaller skyfaller = evac.innerContainer[0] as Skyfaller;
                    __result = skyfaller;
                }
            }
        }
    }
}
