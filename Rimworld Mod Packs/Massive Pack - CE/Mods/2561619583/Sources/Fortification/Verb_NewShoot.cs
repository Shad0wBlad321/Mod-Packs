using RimWorld;
using Verse;

namespace Fortification
{
    public class Verb_NewShoot : Verb_Shoot
    {
        protected override bool TryCastShot()
        {
            bool result;
            CompChangeableProjectile compChangeableProjectile = EquipmentSource.GetComp<CompChangeableProjectile>();
            if (compChangeableProjectile != null)
            {
                ThingDef thingDef = null;
                if (burstShotsLeft > 1)
                {
                    thingDef = compChangeableProjectile.LoadedShell;
                }
                result = base.TryCastShot();
                if (thingDef != null)
                {
                    compChangeableProjectile.LoadShell(thingDef, 1);
                }
            }
            else
            {
                result = base.TryCastShot();
            }
            if (result == true)
            {
                if (Caster != null && EquipmentSource.GetComp<CompCastPushHeat>() is CompCastPushHeat compCastPushHeat)
                {
                    if(compCastPushHeat.EnergyPerCast !=0)
                    {
                        GenTemperature.PushHeat(Caster.Position, Caster.Map, compCastPushHeat.EnergyPerCast);
                    }
                }
            }
            return result;
        }
    }
}
