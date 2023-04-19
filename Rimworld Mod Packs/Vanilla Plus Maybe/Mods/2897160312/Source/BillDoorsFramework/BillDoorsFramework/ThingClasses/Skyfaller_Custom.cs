using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using RimWorld;
using RimWorld.Planet;
using UnityEngine;
using Verse;
using Verse.AI.Group;
using Verse.Noise;
using static System.Collections.Specialized.BitVector32;
using static HarmonyLib.Code;

namespace BillDoorsFramework
{
    public class Skyfaller_Custom : Skyfaller
    {
        public ActiveDropPodInfo ActiveDropPodInfo
        {
            get
            {
                return activeDropPodInfo;
            }
            set
            {
                if (activeDropPodInfo != null)
                {
                    activeDropPodInfo.parent = null;
                }
                if (value != null)
                {
                    value.parent = this;
                }
                activeDropPodInfo = value;
            }
        }

        private ActiveDropPodInfo activeDropPodInfo;

        public SkyfallerCustomExtension extension => def.GetModExtension<SkyfallerCustomExtension>();

        protected override void SpawnThings()
        {
            for (int num = innerContainer.Count - 1; num >= 0; num--)
            {
                GenPlace.TryPlaceThing(innerContainer[num], base.Position, base.Map, ThingPlaceMode.Near, delegate (Thing thing, int count)
                {
                    PawnUtility.RecoverFromUnwalkablePositionOrKill(thing.Position, thing.Map);
                    if (thing.def.Fillage == FillCategory.Full && def.skyfaller.CausesExplosion && def.skyfaller.explosionDamage.isExplosive && thing.Position.InHorDistOf(base.Position, def.skyfaller.explosionRadius))
                    {
                        base.Map.terrainGrid.Notify_TerrainDestroyed(thing.Position);
                    }
                }, null, innerContainer[num].Rotation);
            }

            if (extension != null && extension.returnShuttleFallerDef != null)
            {
                SkyfallerMaker.SpawnSkyfaller(extension.returnShuttleFallerDef, Position, Map);
            }
        }

        public override string Label
        {
            get
            {
                if ((extension == null || extension.variableLabel) && innerContainer.Any)
                {
                    return innerContainer[0].Label;
                }
                return base.Label;
            }
        }
    }

    public class SkyfallerCustomExtension : DefModExtension
    {
        public bool variableLabel;

        public ThingDef returnShuttleFallerDef;
    }
}
