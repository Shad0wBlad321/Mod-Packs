using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using Verse;

namespace BillDoorsFramework
{
    public class Verb_ShootUsingMechBattery : Verb_Shoot
    {
        public DefModExtension_ShootUsingMechBattery Data
        {
            get
            {
                return EquipmentCompSource.parent.def.GetModExtension<DefModExtension_ShootUsingMechBattery>();
            }
        }

        public DefModExtension_ShootUsingRandomProjectile DataProj
        {
            get
            {
                return EquipmentCompSource.parent.def.GetModExtension<DefModExtension_ShootUsingRandomProjectile>();
            }
        }

        public ModExtension_Verb_Shotgun DataShotgun
        {
            get
            {
                return EquipmentCompSource.parent.def.GetModExtension<ModExtension_Verb_Shotgun>();
            }
        }

        int currenBurstRandomIndex;

        public override void WarmupComplete()
        {
            if (DataProj != null)
            {
                currenBurstRandomIndex = Rand.Range(0, DataProj.projectiles.Count - 1);
                verbProps.defaultProjectile = DataProj.projectiles[currenBurstRandomIndex];
            }
            base.WarmupComplete();
        }

        public override bool Available()
        {
            if (ModLister.BiotechInstalled && Data != null)
            {
                Need_MechEnergy battery = CasterPawn.needs.TryGetNeed<Need_MechEnergy>();
                if (battery != null)
                {
                    return battery.CurLevel > Data.energyConsumption;
                }
            }
            return base.Available();
        }

        protected override bool TryCastShot()
        {
            if (base.TryCastShot())
            {
                if (ModLister.BiotechInstalled && Data != null)
                {
                    Need_MechEnergy battery = CasterPawn.needs.TryGetNeed<Need_MechEnergy>();
                    if (battery != null)
                    {
                        battery.CurLevel -= Data.energyConsumption;
                    }
                }
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
                return true;
            }
            return false;
        }
    }
    public class DefModExtension_ShootUsingMechBattery : DefModExtension
    {
        public float energyConsumption = 0.001f;
    }

    public class Verb_ShootWithRandomProjectile : Verb_Shoot
    {
        public DefModExtension_ShootUsingRandomProjectile DataProj
        {
            get
            {
                return EquipmentCompSource.parent.def.GetModExtension<DefModExtension_ShootUsingRandomProjectile>();
            }
        }

        int currenBurstRandomIndex;

        public override void WarmupComplete()
        {
            currenBurstRandomIndex = Rand.Range(0, DataProj.projectiles.Count - 1);
            verbProps.defaultProjectile = DataProj.projectiles[currenBurstRandomIndex];
            base.WarmupComplete();
        }
    }

    public class DefModExtension_ShootUsingRandomProjectile : DefModExtension
    {
        public List<ThingDef> projectiles = new List<ThingDef>();
        public bool randomWithinBurst = false;
    }
}
