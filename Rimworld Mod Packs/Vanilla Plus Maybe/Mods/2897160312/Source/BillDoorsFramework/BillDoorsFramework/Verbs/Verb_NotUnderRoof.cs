using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using Verse;

namespace BillDoorsFramework
{
    public class Verb_ShootNotUnderRoof : Verb_Shoot
    {
        ModExtension_VerbNotUnderRoof extension => EquipmentSource.def.GetModExtension<ModExtension_VerbNotUnderRoof>();

        CompSecondaryVerb compSecondaryVerb => EquipmentSource.TryGetComp<CompSecondaryVerb>();
        public override bool Available()
        {
            if (Caster.Position.Roofed(Caster.Map)
                && (compSecondaryVerb == null || extension == null || (compSecondaryVerb.IsSecondaryVerbSelected && extension.appliesInSecondaryMode) || (!compSecondaryVerb.IsSecondaryVerbSelected && extension.appliesInPrimaryMode)))
            {
                return false;
            }
            return base.Available();
        }
    }

    public class Verb_ShootNotUnderRoofOneUse : Verb_ShootOneUse
    {
        ModExtension_VerbNotUnderRoof extension => EquipmentSource.def.GetModExtension<ModExtension_VerbNotUnderRoof>();

        CompSecondaryVerb compSecondaryVerb => EquipmentSource.TryGetComp<CompSecondaryVerb>();
        public override bool Available()
        {
            if (Caster.Position.Roofed(Caster.Map)
                && (compSecondaryVerb == null || extension == null || (compSecondaryVerb.IsSecondaryVerbSelected && extension.appliesInSecondaryMode) || (!compSecondaryVerb.IsSecondaryVerbSelected && extension.appliesInPrimaryMode)))
            {
                return false;
            }
            return base.Available();
        }
    }

    public class ModExtension_VerbNotUnderRoof : DefModExtension
    {
        public bool appliesInPrimaryMode = true;
        public bool appliesInSecondaryMode = true;
    }
}
