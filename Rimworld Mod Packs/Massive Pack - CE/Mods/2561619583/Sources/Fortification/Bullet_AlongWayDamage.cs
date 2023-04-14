using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace Fortification
{
    public class Bullet_AlongWayDamage : Projectile
	{
		private int tick = 0;

		public override void Tick()
		{
			Vector3 exactPosition = ExactPosition;
			if (landed)
			{
				return;
			}
			ticksToImpact--;
			if (!ExactPosition.InBounds(base.Map))
			{
				ticksToImpact++;
				base.Position = ExactPosition.ToIntVec3();
				ExplosionDestroy(exactPosition);
				Destroy();
				return;
			}
			if (ticksToImpact == 60 && Find.TickManager.CurTimeSpeed == TimeSpeed.Normal && def.projectile.soundImpactAnticipate != null)
			{
				def.projectile.soundImpactAnticipate.PlayOneShot(this);
			}
			if (ticksToImpact <= 0)
			{
				if (base.DestinationCell.InBounds(base.Map))
				{
					base.Position = base.DestinationCell;
				}
				ExplosionDestroy(exactPosition);
				Destroy();
			}
			else if (tick <= 10)
			{
				tick++;
			}
			else
			{
				TakeDamage(exactPosition);
			}
		}

		private void TakeDamage(Vector3 exact)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			List<Thing> list = new List<Thing>(exact.ToIntVec3().GetThingList(base.Map));
			foreach (Thing item in list)
			{
				if (item != this && item.def.useHitPoints)
				{
					item.TakeDamage(new DamageInfo(DamageDefOf.Crush, 10f, 0f, -1f, launcher, null, base.EquipmentDef));
				}
				if (item is Pawn pawn && !pawn.Dead && !pawn.Downed && GetComp<CompAlongWayDamage>().Props.alongOnWayHediff != null)
				{
					Hediff hediff = HediffMaker.MakeHediff(GetComp<CompAlongWayDamage>().Props.alongOnWayHediff, pawn);
					hediff.Severity = GetComp<CompAlongWayDamage>().Props.alongOnWayHediffSeverity;
					pawn.health.AddHediff(hediff);
				}
			}
		}

		private void ExplosionDestroy(Vector3 exact)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			GenExplosion.DoExplosion(exact.ToIntVec3(),
				base.Map,
				GetComp<CompAlongWayDamage>().Props.endExplosionRadius,
				GetComp<CompAlongWayDamage>().Props.endExplosionDam,
				(Thing)this,
				GetComp<CompAlongWayDamage>().Props.endExplosionDamAmount,
				GetComp<CompAlongWayDamage>().Props.endExplosionArmorPenetration,
				(SoundDef)null,
				(ThingDef)null,
				(ThingDef)null,
				(Thing)null,
				(ThingDef)null,
				1f,
				1,
				null,
				false,
				(ThingDef)null,
				0f,
				1,
				0f,
				false,
				(float?)null,
				(List<Thing>)null
				);
		}
	}
}
