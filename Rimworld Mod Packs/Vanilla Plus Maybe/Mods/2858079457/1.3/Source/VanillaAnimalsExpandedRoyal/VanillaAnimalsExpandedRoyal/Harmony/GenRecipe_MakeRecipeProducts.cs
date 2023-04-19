using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;
using RimWorld;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;

namespace VanillaAnimalsExpandedRoyal
{




    [HarmonyPatch(typeof(GenRecipe))]
    [HarmonyPatch("MakeRecipeProducts")]

    public static class VanillaAnimalsExpandedRoyal_GenRecipe_MakeRecipeProducts_Patch
    {
        
        public static IEnumerable<Thing> Postfix(IEnumerable<Thing> values, RecipeDef recipeDef)
        {
            List<Thing> resultingList = values.ToList();
            if (recipeDef.products != null)
            {

                if (ModLister.HasActiveModWithName("Vanilla Cooking Expanded"))
                {
                    StaticCollectionsClass.AddMealToList(ThingDef.Named("VCE_FineBake"));
                    StaticCollectionsClass.AddMealToList(ThingDef.Named("VCE_SimpleGrill"));
                    StaticCollectionsClass.AddMealToList(ThingDef.Named("VCE_FineGrill"));
                }
                foreach (Thing thing in resultingList)
                {
                    if (StaticCollectionsClass.allowedMeals.Contains(thing.def))
                    {
                        CompIngredients compIngredients = thing.TryGetComp<CompIngredients>();
                        if (compIngredients != null)
                        {
                            if (compIngredients.ingredients.Contains(InternalDefOf.VAERoy_QuailMeat))
                            {
                                if(thing.def== InternalDefOf.MealFine_Meat)
                                {
                                    thing.def = InternalDefOf.MealLavish_Meat;
                                } else
                                if (thing.def.defName == "VCE_FineBake")
                                {
                                    thing.def = ThingDef.Named("VCE_LavishBake");
                                }
                                else
                                if (thing.def.defName == "VCE_SimpleGrill" || thing.def.defName == "VCE_FineGrill")
                                {
                                    thing.def = ThingDef.Named("VCE_LavishGrill");
                                }
                                else thing.def = InternalDefOf.MealLavish;

                            }
                        }
                    }
                    
                }

            }
            return resultingList;



          
            




        }

    }





}
