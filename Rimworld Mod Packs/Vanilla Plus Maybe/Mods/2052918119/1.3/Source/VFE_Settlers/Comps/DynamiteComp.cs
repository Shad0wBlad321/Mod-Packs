using Verse;

namespace VFE_Settlers.Comps
{
    public class DynamiteComp : Projectile_Explosive
    {
        private int ticksToDetonation;

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref ticksToDetonation, "ticksToDetonation", 0);
        }

        public override void Tick()
        {
            base.Tick();
            if (ticksToDetonation != 0)
            {
                ticksToDetonation--;
                if (ticksToDetonation <= 0)
                {
                    Explode();
                }
            }
            else
            {
                ticksToDetonation = Rand.Range(0, 646) + 20;
            }
        }

        protected override void Impact(Thing hitThing)
        {
            if (Rand.Value < 0.25f)
            {
                Explode();
                return;
            }
            landed = true;
            GenExplosion.NotifyNearbyPawnsOfDangerousExplosive(this, def.projectile.damageDef, launcher.Faction);
        }
    }
}