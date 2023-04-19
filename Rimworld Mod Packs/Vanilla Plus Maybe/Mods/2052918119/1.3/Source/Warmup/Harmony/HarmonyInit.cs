using HarmonyLib;
using System.Reflection;
using Verse;

namespace Warmup
{
    [StaticConstructorOnStartup]
    public class HarmonyInit
    {
        static HarmonyInit()
        {
            new Harmony("warmup").PatchAll(Assembly.GetExecutingAssembly());
        }
    }
}