using HarmonyLib;
using System.Reflection;
using Verse;


namespace VanillaAnimalsExpandedRoyal
{
    //Setting the Harmony instance
    [StaticConstructorOnStartup]
    public class Main
    {
        static Main()
        {
            var harmony = new Harmony("com.vanillaanimalsexpandedroyal");
            harmony.PatchAll(Assembly.GetExecutingAssembly());
        }


    }
}
