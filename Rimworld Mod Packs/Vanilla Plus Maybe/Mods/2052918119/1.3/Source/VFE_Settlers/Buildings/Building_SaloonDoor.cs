using RimWorld;
using Verse;

namespace VFE_Settlers.Buildings
{
    public class Building_SaloonDoor : Building_Door
    {
        private const float OpenTicks = 20f;
        private const int CloseDelayTicks = 20;

        new public bool FreePassage => true;
        new public bool SlowsPawns => false;

        new public bool CanPhysicallyPass(Pawn p) => true;

        new public bool PawnCanOpen(Pawn p) => true;

        public override void TickRare()
        {
            GenTemperature.EqualizeTemperaturesThroughBuilding(this, 14f, twoWay: true);
        }
    }
}