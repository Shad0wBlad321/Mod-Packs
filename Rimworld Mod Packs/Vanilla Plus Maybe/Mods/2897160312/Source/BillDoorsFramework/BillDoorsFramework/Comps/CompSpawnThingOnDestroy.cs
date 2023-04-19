using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using Verse;

namespace BillDoorsFramework
{
    public class CompSpawnThingOnDestroy : ThingComp
    {
        public CompProperties_SpawnThingOnDestroy Props
        {
            get
            {
                return (CompProperties_SpawnThingOnDestroy)this.props;
            }
        }

        private Map cachedMap;

        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            base.PostSpawnSetup(respawningAfterLoad);
            cachedMap = this.parent.Map;
        }

        protected virtual Faction GetFaction()
        {
            if (parent is Projectile projectile)
            {
                return projectile.Launcher.Faction;
            }
            return parent.Faction;
        }

        public override void PostDestroy(DestroyMode mode, Map previousMap)
        {
            Thing thing = ThingMaker.MakeThing(this.Props.def);
            thing.SetFaction(GetFaction());
            if (!thing.def.MadeFromStuff)
            {
                GenPlace.TryPlaceThing(thing, parent.Position, cachedMap, ThingPlaceMode.Near);
            }
            base.PostDestroy(mode, previousMap);
        }
    }

    public class CompProperties_SpawnThingOnDestroy : CompProperties
    {
        public CompProperties_SpawnThingOnDestroy()
        {
            compClass = typeof(CompSpawnThingOnDestroy);
        }

        public ThingDef def;
    }
}
