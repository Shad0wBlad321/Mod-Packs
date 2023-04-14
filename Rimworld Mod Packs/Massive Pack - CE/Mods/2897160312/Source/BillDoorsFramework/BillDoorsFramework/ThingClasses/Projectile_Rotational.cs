using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using UnityEngine;
using Verse;

namespace BillDoorsFramework
{
    public class Projectile_Rotational : Bullet
    {
        public bool Landed => landed;
        public float rotationAngle;

        RotationalProjExtension Extension => def.GetModExtension<RotationalProjExtension>();

        public virtual float Speed => landed ? 0 : Extension == null ? 1 : Extension.speed;

        public override void Draw()
        {
            Quaternion rotation = Quaternion.AngleAxis(rotationAngle, Vector3.up);
            Graphics.DrawMesh(MeshPool.GridPlane(def.graphicData.drawSize), DrawPos, rotation, def.DrawMatSingle, 0);
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

    public class RotationalProjExtension : DefModExtension
    {
        public float speed = 1f;
    }
}
