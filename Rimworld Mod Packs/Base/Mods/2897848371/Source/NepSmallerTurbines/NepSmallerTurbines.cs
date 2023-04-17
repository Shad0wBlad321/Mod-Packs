using System;
using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
using System.Reflection;

using RimWorld;
//using UnityEngine;
using Verse;
using HarmonyLib;


namespace NepSmallerTurbines
{
    [StaticConstructorOnStartup]
    public static class NepSmallerTurbines
    {
        static float newBladeWidth = 4.0f; //Default is 6.6


        static NepSmallerTurbines() //our constructor
        {
            Log.Message("NepSmallerTurbines Loaded");

            Harmony harmony = new Harmony("nep.smallerturbines");
            harmony.PatchAll();

            // set the bladeWidth tweakable value - luckily this is a tweakable and not a real static float
            // Do this by looking through every tweakable to find the correct one
            // Top level loop structure taken from Verse.EditWindow_TweakValues.FindAllTweakables()

            bool foundBladeWidth = false;
            foreach (Type type in GenTypes.AllTypes)
            {
                foreach (FieldInfo fieldInfo in type.GetFields(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic))
                {
                    if (fieldInfo.TryGetAttribute<TweakValue>() != null)
                    {
                        // look at the 2 strings fieldInfo.DeclaringType.FullName, fieldInfo.Name
                        //Log.Message(string.Format("NepSmallerTurbines DEBUG: {0} {1} ", fieldInfo.DeclaringType.FullName, fieldInfo.Name));
                        //if it it blade length then 
                        if (fieldInfo.DeclaringType.FullName == "RimWorld.CompPowerPlantWind" & fieldInfo.Name == "BladeWidth")
                        {
                            Log.Message(string.Format("NepSmallerTurbines Setting Value of Tweakable {0} {1} to {2}", fieldInfo.DeclaringType.FullName, fieldInfo.Name, newBladeWidth));
                            fieldInfo.SetValue(null, newBladeWidth);
                            foundBladeWidth = true;
                            break;
                        }
                       

                    }
                }

            }
            if (!foundBladeWidth)
            {
                Log.Message("NepSmallerTurbines Uunable to find BladeWidth tweakable value"); //Not worth making this an error, just log it and carry on
            }

        }
    }

    //
    //
    //public class NepSmallerTurbinesConfig : Mod
    //{
    //    public NepSmallerTurbinesConfig(ModContentPack content) : base(content)
    //    {
    //        Log.Message("NepSmallerTurbines Inherited Project Mod Class Loaded");
    //    }
    //}
    //


    //redo size of keep-clear area

    [HarmonyPatch]
    public static class NepSmallerTurbinesPatch
    {
        [HarmonyPatch(typeof(WindTurbineUtility), nameof(WindTurbineUtility.CalculateWindCells))]
        [HarmonyPostfix]
        public static IEnumerable<IntVec3> NepSmallerTurbinesPostFix(IEnumerable<IntVec3> __result, IntVec3 center, Rot4 rot, IntVec2 size)
        {
            // This is a postfix patch that completely ignores the results of the original and redoes everything with slightly different numbers.

            //Function reurns an IEnumerable of cells that are the  area that needs to be unblocked for the turbine to work.

            CellRect rectA = default(CellRect);
            CellRect rectB = default(CellRect);
            int num = 0;
            int num2;
            int num3;
            if (rot == Rot4.North || rot == Rot4.East)
            {
                num2 = 7;
                num3 = 3;
            }
            else
            {
                num2 = 3;
                num3 = 7;
                num = -1;
            }
            if (rot.IsHorizontal)
            {
                rectA.minX = center.x + 2 + num;
                rectA.maxX = center.x + 2 + num2 + num;
                rectB.minX = center.x - 1 - num3 + num;
                rectB.maxX = center.x - 1 + num;
                // with an even width the centre tile is no longer centered so need to do E/W and N/S with different offsets
                if (rot == Rot4.East)
                {
                    rectB.minZ = (rectA.minZ = center.z - 2);
                    rectB.maxZ = (rectA.maxZ = center.z + 1);
                }
                else
                {
                    rectB.minZ = (rectA.minZ = center.z - 1);
                    rectB.maxZ = (rectA.maxZ = center.z + 2);
                }

            }
            else
            {
                rectA.minZ = center.z + 2 + num;
                rectA.maxZ = center.z + 2 + num2 + num;
                rectB.minZ = center.z - 1 - num3 + num;
                rectB.maxZ = center.z - 1 + num;
                // with an even width the centre tile is no longer centered so need to do E/W and N/S with different offsets
                if (rot == Rot4.North)
                { 
                    rectB.minX = (rectA.minX = center.x - 1);
                    rectB.maxX = (rectA.maxX = center.x + 2);
                }
                else
                {
                    rectB.minX = (rectA.minX = center.x - 2);
                    rectB.maxX = (rectA.maxX = center.x + 1);
                }

            }
            int num4;
            for (int z = rectA.minZ; z <= rectA.maxZ; z = num4 + 1)
            {
                for (int x = rectA.minX; x <= rectA.maxX; x = num4 + 1)
                {
                    yield return new IntVec3(x, 0, z);
                    num4 = x;
                }
                num4 = z;
            }
            for (int z = rectB.minZ; z <= rectB.maxZ; z = num4 + 1)
            {
                for (int x = rectB.minX; x <= rectB.maxX; x = num4 + 1)
                {
                    yield return new IntVec3(x, 0, z);
                    num4 = x;
                }
                num4 = z;
            }
            yield break;



        }

    }

}
