using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using Verse;
using CombatExtended;

namespace BillDoorsFramework
{
    public class Verb_ShootNotUnderRoofCE : Verb_ShootCE
    {
        ModExtension_VerbNotUnderRoof extension => EquipmentSource.def.GetModExtension<ModExtension_VerbNotUnderRoof>();

        CompSecondaryVerbCE compSecondaryVerb => EquipmentSource.TryGetComp<CompSecondaryVerbCE>();

        CompSecondaryAmmo compSecondaryAmmo => EquipmentSource.TryGetComp<CompSecondaryAmmo>();
        public override bool Available()
        {
            if (Caster.Position.Roofed(Caster.Map))
            {
                if (extension == null || (compSecondaryVerb != null && ((compSecondaryVerb.IsSecondaryVerbSelected && extension.appliesInSecondaryMode) || (!compSecondaryVerb.IsSecondaryVerbSelected && extension.appliesInPrimaryMode))) || (compSecondaryAmmo != null && ((compSecondaryAmmo.IsSecondaryAmmoSelected && extension.appliesInSecondaryMode) || (!compSecondaryAmmo.IsSecondaryAmmoSelected && extension.appliesInPrimaryMode))))
                {
                    return false;
                }
            }
            return base.Available();
        }
    }

    public class Verb_ShootNotUnderRoofOneUseCE : Verb_ShootCEOneUse
    {
        ModExtension_VerbNotUnderRoof extension => EquipmentSource.def.GetModExtension<ModExtension_VerbNotUnderRoof>();

        CompSecondaryVerbCE compSecondaryVerb => EquipmentSource.TryGetComp<CompSecondaryVerbCE>();

        CompSecondaryAmmo compSecondaryAmmo => EquipmentSource.TryGetComp<CompSecondaryAmmo>();
        public override bool Available()
        {
            if (Caster.Position.Roofed(Caster.Map))
            {
                if (extension == null || (compSecondaryVerb != null && ((compSecondaryVerb.IsSecondaryVerbSelected && extension.appliesInSecondaryMode) || (!compSecondaryVerb.IsSecondaryVerbSelected && extension.appliesInPrimaryMode))) || (compSecondaryAmmo != null && ((compSecondaryAmmo.IsSecondaryAmmoSelected && extension.appliesInSecondaryMode) || (!compSecondaryAmmo.IsSecondaryAmmoSelected && extension.appliesInPrimaryMode))))
                {
                    return false;
                }
            }
            return base.Available();
        }
    }
}
