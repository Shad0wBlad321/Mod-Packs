using System;
using System.Collections.Generic;
using RimWorld;
using Verse;

namespace BillDoorsFramework
{
    public class Verb_ShootCustom : Verb_Shoot
    {
        DefModExtension_ShootUsingRandomProjectile DataRandProj => EquipmentCompSource.parent.def.GetModExtension<DefModExtension_ShootUsingRandomProjectile>();

        ModExtension_Verb_Shotgun DataShotgun => EquipmentSource.def.GetModExtension<ModExtension_Verb_Shotgun>();

        ModExtension_VerbNotUnderRoof DataNotUnderRoof => EquipmentSource.def.GetModExtension<ModExtension_VerbNotUnderRoof>();

        DefModExtension_ShootUsingMechBattery DataMechBattery => EquipmentSource.def.GetModExtension<DefModExtension_ShootUsingMechBattery>();

        ModExtension_RandomBurstBreak randomBurstBreak => EquipmentSource.def.GetModExtension<ModExtension_RandomBurstBreak>();

        CompSecondaryVerb compSecondaryVerb => EquipmentSource.TryGetComp<CompSecondaryVerb>();

        int currenBurstRandomIndex;

        public override void WarmupComplete()
        {
            RandomizeProjectile();
            base.WarmupComplete();
            RandomizeBurstCount();
        }

        public override bool Available()
        {
            if (verbProps.consumeFuelPerBurst > 0f)
            {
                CompRefuelable compRefuelable = caster.TryGetComp<CompRefuelable>();
                if (compRefuelable != null && compRefuelable.Fuel < verbProps.consumeFuelPerBurst)
                {
                    return false;
                }
            }
            return AvailableNotUnderRoof() && AvailableMechBattery() && base.Available();
        }

        protected override bool TryCastShot()
        {
            if (DataRandProj != null && DataRandProj.randomWithinBurst)
            {
                RandomizeProjectile();
            }
            if (base.TryCastShot())
            {
                TryCastShotMechBattery();
                TryCastShotShotgun();
                TryCastShotRandomBurstBreak();
                return true;
            }
            return false;
        }

        public bool AvailableNotUnderRoof()
        {
            return !(DataNotUnderRoof != null && Caster.Position.Roofed(Caster.Map) && (compSecondaryVerb == null || (compSecondaryVerb.IsSecondaryVerbSelected && DataNotUnderRoof.appliesInSecondaryMode) || (!compSecondaryVerb.IsSecondaryVerbSelected && DataNotUnderRoof.appliesInPrimaryMode)));
        }

        public bool AvailableMechBattery()
        {
            Need_MechEnergy battery = CasterPawn?.needs.TryGetNeed<Need_MechEnergy>();
            if (battery != null)
            {
                return battery.CurLevel > DataMechBattery.energyConsumption;
            }
            return true;
        }

        public void RandomizeProjectile()
        {
            if (DataRandProj != null)
            {
                currenBurstRandomIndex = Rand.Range(0, DataRandProj.projectiles.Count - 1);
                verbProps.defaultProjectile = DataRandProj.projectiles[currenBurstRandomIndex];
            }
        }

        public void RandomizeBurstCount()
        {
            if (randomBurstBreak != null)
            {
                burstShotsLeft += randomBurstBreak.randomBurst.RandomInRange;
            }
        }

        void TryCastShotMechBattery()
        {
            if (ModLister.BiotechInstalled && DataMechBattery != null)
            {
                Need_MechEnergy battery = CasterPawn?.needs.TryGetNeed<Need_MechEnergy>();
                if (battery != null)
                {
                    battery.CurLevel -= DataMechBattery.energyConsumption;
                }
            }
        }

        void TryCastShotShotgun()
        {
            if (DataShotgun != null)
            {
                if (DataShotgun.ShotgunPellets > 1)
                {
                    for (int i = 1; i < DataShotgun.ShotgunPellets; i++)
                    {
                        base.TryCastShot();
                    }
                }
                if (DataShotgun.extraProjectile != null && DataShotgun.extraProjectileCount > 0)
                {
                    ThingDef originalProjectile = verbProps.defaultProjectile;
                    verbProps.defaultProjectile = DataShotgun.extraProjectile;
                    for (int i = 0; i < DataShotgun.extraProjectileCount; i++)
                    {
                        base.TryCastShot();
                    }
                    verbProps.defaultProjectile = originalProjectile;
                }
            }
        }

        void TryCastShotRandomBurstBreak()
        {
            if (randomBurstBreak != null && Rand.Chance(randomBurstBreak.chance))
            {
                burstShotsLeft = 1;
            }
        }
    }
}
