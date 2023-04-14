using Verse;
using System.Collections.Generic;

namespace SanguophageFeedRelationship
{
    class SFR_AutoFeeders : GameComponent
    {
        // so many thanks to feldoh10 for all their help with debugging !

        //HashSet<Pawn> autoFeeders;
        static HashSet<int> autoFeederPawns;

        public SFR_AutoFeeders(Game g):base() {
            autoFeederPawns = new HashSet<int>();
        }
        //public SFR_AutoFeeders() { }
        

        public void SetAutoFeed(Pawn pawn, bool included)
        {
            if (autoFeederPawns.Contains(pawn.thingIDNumber))
            {
                if (included)
                    return;
                else
                {
                    autoFeederPawns.Remove(pawn.thingIDNumber);
                    //Log.Message("Removed " + pawn.Name + ". List now contains: " + autoFeederPawns.Count.ToString()); 
                }
            }
            else {
                if (included)
                {
                    autoFeederPawns.Add(pawn.thingIDNumber);
                    //Log.Message("Added " + pawn.Name + ". List now contains: " + autoFeederPawns.Count.ToString()); 
                }
                else
                    return;
            }
        }

        public bool GetAutoFeed(int pawnID)
        {
            //Log.Message("- Sending to check: " + pawnID + ". There are " + autoFeederPawns.Count + " pawns in the list.");
            return autoFeederPawns.Contains(pawnID);
            //return autoFeeders.Contains(pawn);
        }
        
        public override void ExposeData()
        {
            base.ExposeData();
            if (Scribe.mode == LoadSaveMode.PostLoadInit)  
            { 

                if (autoFeederPawns == null)
                    autoFeederPawns = new HashSet<int>();

                /*if (autoFeeders == null)
                    autoFeeders = new HashSet<Pawn>();

                Scribe_Collections.Look(ref autoFeeders, "autoFeederPawns", LookMode.Reference); // bring in the old list
                Log.Message("SFR old list: " + autoFeeders.Count.ToString());

                if (!autoFeeders.EnumerableNullOrEmpty()) // if there were any saved
                {
                    foreach (Pawn pawn in autoFeeders)
                    {
                        int pID = pawn.thingIDNumber;
                        if (autoFeederPawns.Contains(pID))  // and they were already in the new list, remove them
                        {
                            autoFeeders.Remove(pawn);
                        }
                        else                              // if they weren't in the new list add them, and remove from the old list
                        {
                            autoFeederPawns.Add(pID);
                            autoFeeders.Remove(pawn);
                        }
                    }
                    Log.Message("Updated SFR old list: " + autoFeeders.Count.ToString());
                    Log.Message("Updated SFR new list: " + autoFeederPawns.Count.ToString());
                }*/
            }


            //Log.Message("autoFeeders list was null so created a new list");
            

            //Log.Message("EXPOSE DATA List count: " + autoFeeders.Count.ToString() + "  " + Scribe.mode.ToString());
            Scribe_Collections.Look(ref autoFeederPawns, "autoFeederPawnsIDs", LookMode.Value);
            //Log.Message("SFR new list: " + autoFeederPawns.Count.ToString());

            /*if (Scribe.mode == LoadSaveMode.Saving)
            {
                Scribe_Collections.Look(ref autoFeeders, "autoFeederPawns", LookMode.Reference);
                Log.Message("SFR old list: " + autoFeeders.Count.ToString());
            }*/

        }

    }
}
