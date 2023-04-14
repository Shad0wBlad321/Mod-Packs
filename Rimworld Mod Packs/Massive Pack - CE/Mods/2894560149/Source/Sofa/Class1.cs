using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using Verse;

namespace DoctorStupid.Sofa
{
    [StaticConstructorOnStartup]
    public static class DoctorStupidSofaInitialize
    {
        static DoctorStupidSofaInitialize()
        {
            ThingDef sofaDef = DefDatabase<ThingDef>.GetNamed("DocStu_Sofa", true);
            if (LoadedModManager.GetMod<SofaMod>().GetSettings<SofaSettings>().couchMode)
            {
                sofaDef.label = "couch";
            }
        }
    }
}