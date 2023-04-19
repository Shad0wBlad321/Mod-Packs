
using Verse;
using System;
using RimWorld;
using System.Collections.Generic;
using System.Linq;


namespace VanillaAnimalsExpandedRoyal
{

    public static class StaticCollectionsClass
    {

        //This static class stores lists of animals and pawns for different things.



        public static int ownedCranesInMap = 0;

        public static List<ThingDef> allowedMeals = new List<ThingDef>() { InternalDefOf.MealSimple, InternalDefOf.MealFine, InternalDefOf.MealFine_Meat };

        public static void AddMealToList(ThingDef thing)
        {

            if (!allowedMeals.Contains(thing))
            {
                allowedMeals.Add(thing);
            }
        }


    }
}
