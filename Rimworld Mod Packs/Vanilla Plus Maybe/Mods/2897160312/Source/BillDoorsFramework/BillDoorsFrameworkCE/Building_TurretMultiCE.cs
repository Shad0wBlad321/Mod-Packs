using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using UnityEngine;
using Verse;
using CombatExtended;

namespace BillDoorsFramework
{
    public class Building_TurretMultiCE : Building_TurretGunCE
    {
        ModExtension_Building_TurretMulti Extension;
        CompRefuelable CompRefuelable;

        List<FuelPercentAndGraphicPair> Graphics = new List<FuelPercentAndGraphicPair>();

        public override void SpawnSetup(Map map, bool respawningAfterLoad)
        {
            base.SpawnSetup(map, respawningAfterLoad);
            Extension = def.GetModExtension<ModExtension_Building_TurretMulti>();
            CompRefuelable = this.TryGetComp<CompRefuelable>();
            Graphics = Extension?.graphicDatas;
        }

        public override Material TurretTopMaterial
        {
            get
            {
                if (Extension != null)
                {
                    Material mat = null;
                    if (CompAmmo != null)
                    {
                        for (int i = 0; i < Graphics.Count; i++)
                        {
                            if (CompAmmo.CurMagCount <= Graphics[i].fuelPercent)
                            {
                                mat = MaterialPool.MatFrom(Graphics[i].graphicData.texPath);
                            }
                        }
                    }
                    else
                    {
                        for (int i = 0; i < Graphics.Count; i++)
                        {
                            if (CompRefuelable?.FuelPercentOfMax <= Graphics[i].fuelPercent)
                            {
                                mat = MaterialPool.MatFrom(Graphics[i].graphicData.texPath);
                            }
                        }
                    }
                    return mat ?? def.building.turretTopMat;
                }
                return this.def.building.turretTopMat;
            }
        }
    }
}
