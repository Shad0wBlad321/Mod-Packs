using System;
using System.Collections.Generic;
using RimWorld;
using Verse;
using CombatExtended;

namespace BillDoorsFramework
{
    public class Verb_ShootCustomCE : Verb_ShootCE
    {
        DefModExtension_ShootUsingRandomProjectile DataRandProj => EquipmentCompSource.parent.def.GetModExtension<DefModExtension_ShootUsingRandomProjectile>();

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

        public override bool TryCastShot()
        {
            if (base.TryCastShot())
            {
                TryCastShotFireAllAtOnce();
                TryCastShotMechBattery();
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

        void TryCastShotRandomBurstBreak()
        {
            if (randomBurstBreak != null && Rand.Chance(randomBurstBreak.chance))
            {
                burstShotsLeft = 1;
            }
        }

        void TryCastShotFireAllAtOnce()
        {
            if (EquipmentSource.def.HasModExtension<ModExtension_FireAllAtOnceCE>())
            {
                int barrelCount = EquipmentSource.def.GetModExtension<ModExtension_FireAllAtOnceCE>().barrelCount;
                for (int i = 1; i < ShotsPerBurst; i++)
                {
                    base.TryCastShot();
                    burstShotsLeft--;
                    if (barrelCount > 1)
                    {
                        MuzzleFlashInvoker.MuzzleFlashInvoker.SpawnMuzzleFlash(this, Rand.Range(0, barrelCount - 1));
                    }
                }
            }
        }
    }
}
