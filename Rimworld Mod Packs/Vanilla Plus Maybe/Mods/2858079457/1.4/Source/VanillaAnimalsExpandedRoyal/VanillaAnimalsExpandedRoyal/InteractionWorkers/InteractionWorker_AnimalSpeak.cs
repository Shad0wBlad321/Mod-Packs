
using RimWorld;
using Verse;

namespace VanillaAnimalsExpandedRoyal
{
    public class InteractionWorker_AnimalSpeak : InteractionWorker
    {
        public override float RandomSelectionWeight(Pawn initiator, Pawn recipient)
        {
            if (initiator.def==InternalDefOf.VAERoy_Orangutan && recipient.RaceProps.Humanlike)
            {
                return 1f;
            }
            else return 0;


        }
    }
}
