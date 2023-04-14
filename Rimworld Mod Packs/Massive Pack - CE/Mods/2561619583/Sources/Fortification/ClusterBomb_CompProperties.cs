using RimWorld;
using Verse;

namespace Fortification
{
    public class ClusterBomb_CompProperties : CompProperties
	{
		public int clusterBurstCount = 1;

		public ThingDef projectile = null;

		public float errorRange = 0f;

		public bool doDismantlExplosion = false;

		public DamageDef dismantlExplosionDam = DamageDefOf.Bomb;

		public float dismantlExplosionRadius = 1f;

		public int dismantlExplosionDamAmount = -1;

		public float dismantlExplosionArmorPenetration = -1f;

		public SoundDef dismantlExplosionSound = null;

		public IntRange TDDistance = new IntRange(0, 0);

		public ClusterBomb_CompProperties()
		{
			compClass = typeof(CompClusterBomb);
		}
	}
}
