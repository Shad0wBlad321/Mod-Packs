using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace Fortification
{
    public class CompProperties_CompositeExplosion :CompProperties
    {
        public CompProperties_CompositeExplosion()
        {
            compClass = typeof(CompCompositeExplosion);
        }
        public List<CompositeExplosion> compositeExplosions = new List<CompositeExplosion>();
    }
    public class CompositeExplosion
    {
        public int countdown;
        public float radius;
        public DamageDef damamgeDef;
        public int amount;
        public float? armorPenetration=null;
        public SoundDef explosionSound = null;

        public ThingDef preExplosionSpawnThingDef = null;
        public float preExplosionSpawnChance = 0;
        public int preExplosionSpawnThingCount = 0;

        public ThingDef postExplosionSpawnThingDef = null;
        public ThingDef postExplosionSpawnThingDefWater = null;
        public GasType postExplosionGasType = 0;
        public float postExplosionSpawnChance = 0;
        public int postExplosionSpawnThingCount = 0;
        public float chanceToStartFire = 0;
    }
}
