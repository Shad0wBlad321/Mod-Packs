using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using Verse;

namespace BillDoorsLootsAndShelves
{
    public class RoomRoleWorker_Museum : RoomRoleWorker
    {
        public override float GetScore(Room room)
        {
            int num = 0;
            List<Thing> containedAndAdjacentThings = room.ContainedAndAdjacentThings;
            for (int i = 0; i < containedAndAdjacentThings.Count; i++)
            {
                if (containedAndAdjacentThings[i] is Building_Locker locker && locker.isDisplay)
                {
                    num++;
                }
            }
            return 50f * (float)num;
        }
    }
}
