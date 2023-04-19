using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using CombatExtended;
using UnityEngine;
using Verse;

namespace BillDoorsFramework
{

    public class ProjectileCE_Rotational : BulletCE
    {

        public float rotationAngle;

        RotationalProjExtension Extension => def.GetModExtension<RotationalProjExtension>();

        public virtual float Speed => landed ? 0 : Extension == null ? 1 : Extension.speed;

        public override void Draw()
        {
            if (base.FlightTicks == 0 && launcher != null && launcher is Pawn)
            {
                return;
            }
            Quaternion rotation = Quaternion.AngleAxis(rotationAngle, Vector3.up);
            Graphics.DrawMesh(MeshPool.GridPlane(def.graphicData.drawSize), DrawPos, rotation, def.DrawMatSingle, 0);
            if (!castShadow)
            {
                return;
            }
            Vector3 position = new Vector3(ExactPosition.x, def.Altitude - 0.01f, ExactPosition.z - Mathf.Lerp(shotHeight, 0f, base.fTicks / base.StartingTicksToImpact));
            PropertyInfo propertyInfo = null;
            PropertyInfo[] properties = typeof(ProjectileCE).GetProperties(BindingFlags.Instance | BindingFlags.NonPublic);
            foreach (PropertyInfo propertyInfo2 in properties)
            {
                if (propertyInfo2.Name == "ShadowMaterial")
                {
                    propertyInfo = propertyInfo2;
                    break;
                }
            }
            Material material = (Material)propertyInfo.GetValue(this, null);
            Graphics.DrawMesh(MeshPool.GridPlane(def.graphicData.drawSize), position, rotation, material, 0);
        }

        public override void Tick()
        {
            rotationAngle += Speed;
            base.Tick();
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref rotationAngle, "rotationAngle", 0);
        }
    }
}
