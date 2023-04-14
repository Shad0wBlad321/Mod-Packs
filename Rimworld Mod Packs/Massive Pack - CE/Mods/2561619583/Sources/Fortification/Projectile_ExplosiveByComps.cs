using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace Fortification
{
    public class CompProperties_SmokeTrail : CompProperties
    {
        public CompProperties_SmokeTrail()
        {
            compClass = typeof(CompSmokeTrail);
        }
        public GasType spawnGasType = GasType.BlindSmoke;
        public int ThickCount = 1;
    }
    public class CompSmokeTrail : ThingComp
    {
        public CompProperties_SmokeTrail Props => (CompProperties_SmokeTrail)props;
        public GasType gasType = GasType.BlindSmoke;
        public int thickCount = 1;
        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
             thickCount = parent.GetComp<CompSmokeTrail>().Props.ThickCount;
            gasType = parent.GetComp<CompSmokeTrail>().Props.spawnGasType;
        }
    }

    [StaticConstructorOnStartup]
    public class Projectile_SmokeTrailed : Projectile
    {
        private CompSmokeTrail SmokeTrail => this.GetComp<CompSmokeTrail>();
        private static readonly Material shadowMaterial = MaterialPool.MatFrom("Things/Skyfaller/SkyfallerShadowCircle", ShaderDatabase.Transparent);
        private float ArcHeightFactor
        {
            get
            {
                float num = def.projectile.arcHeightFactor;
                float num2 = (destination - origin).MagnitudeHorizontalSquared();
                if (num * num > num2 * 0.2f * 0.2f)
                {
                    num = Mathf.Sqrt(num2) * 0.2f;
                }
                return num;
            }
        }
        public override void Draw()
        {
            float num = ArcHeightFactor * GenMath.InverseParabola(DistanceCoveredFraction);
            Vector3 drawPos = DrawPos;
            Vector3 position = drawPos + new Vector3(0f, 0f, 1f) * num;
            if (def.projectile.shadowSize > 0f)
            {
                DrawShadow(drawPos, num);
            }
            Graphics.DrawMesh(MeshPool.GridPlane(def.graphicData.drawSize), position, ExactRotation, def.DrawMatSingle, 0);
            if (SmokeTrail != null)
            {
                GenExplosion.DoExplosion(position.ToIntVec3(), this.Map, SmokeTrail.thickCount, DamageDefOf.Smoke, null, -1, -1f, null, null, null, null, null, 0f, 1, GasType.BlindSmoke);
            }

        }
        private void DrawShadow(Vector3 drawLoc, float height)
        {
            if (!(shadowMaterial == null))
            {
                float num = def.projectile.shadowSize * Mathf.Lerp(1f, 0.6f, height);
                Vector3 s = new Vector3(num, 1f, num);
                Vector3 vector = new Vector3(0f, -0.01f, 0f);
                Matrix4x4 matrix = default(Matrix4x4);
                matrix.SetTRS(drawLoc + vector, Quaternion.identity, s);
                Graphics.DrawMesh(MeshPool.plane10, matrix, shadowMaterial, 0);
            }
        }
    }
    public class Projectile_ExplosiveByComps: Projectile_Explosive
    {
        public override void SpawnSetup(Map map, bool respawningAfterLoad)
        {
            base.SpawnSetup(map, respawningAfterLoad);
            compCompositeExplosion = GetComp<CompCompositeExplosion>();
            compExpolsionWithEvents = GetComp<CompExpolsionWithEvents>();
        }
        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref ticksToDetonation_ForComps, "ticksToDetonation_ForComps",-1);
        }
        public override void Tick()
        {
            if (ticksToDetonation_ForComps > 0)
            {
                ticksToDetonation_ForComps--;
                if (compCompositeExplosion != null)
                {
                    foreach (CompositeExplosion explosion in compCompositeExplosion.CompositeExplosions)
                    {
                        if(explosion.countdown == ticksToDetonation_ForComps)
                        {
                            GenExplosion.DoExplosion(
                                Position,
                                Map,
                                explosion.radius,
                                explosion.damamgeDef,
                                launcher,
                                explosion.amount,

                                explosion.armorPenetration ?? -1,
                                explosion.explosionSound,
                                equipmentDef,
                                def,
                                intendedTarget.Thing,

                                explosion.postExplosionSpawnThingDef,
                                explosion.postExplosionSpawnChance,
                                explosion.postExplosionSpawnThingCount,
                                explosion.postExplosionGasType,
                                false,
                                explosion.preExplosionSpawnThingDef,
                                explosion.preExplosionSpawnChance,
                                explosion.preExplosionSpawnThingCount,
                                explosion.chanceToStartFire,
                                false,
                                origin.AngleToFlat(destination),
                                null,
                                null,
                                true,
                                def.projectile.damageDef.expolosionPropagationSpeed,
                                0f,
                                true,
                                explosion.postExplosionSpawnThingDefWater,
                                0
                                );
                        }
                    }
                }
            }
            base.Tick();
        }
        protected override void Impact(Thing hitThing, bool blockedByShield)
        {
            ticksToDetonation_ForComps = def.projectile.explosionDelay;
            if (compExpolsionWithEvents != null)
            {
                foreach (Condition condition in compExpolsionWithEvents.Props.conditions)
                {
                    TryStartCondition(condition);
                }
            }
            base.Impact(hitThing, blockedByShield);
        }
        private void TryStartCondition(Condition condition)
        {
            if (Rand.Range(1, 101) > condition.percent)
            {
                return;
            }
            foreach (GameCondition x in Map.gameConditionManager.ActiveConditions)
            {
                if (x.def == condition.conditionDef)
                    return;
            }
            GameCondition gameCondition = GameConditionMaker.MakeCondition(condition.conditionDef, condition.duration.RandomInRange);
            Map.gameConditionManager.RegisterCondition(gameCondition);
        }
        public int ticksToDetonation_ForComps = -1;
        public CompCompositeExplosion compCompositeExplosion;
        public CompExpolsionWithEvents compExpolsionWithEvents;
    }
}
