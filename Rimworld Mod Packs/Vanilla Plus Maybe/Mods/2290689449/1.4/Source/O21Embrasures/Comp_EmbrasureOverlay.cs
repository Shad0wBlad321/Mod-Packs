using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;
using RimWorld;
using Verse;

namespace SeamlessEmbrasures
{
    public class Comp_EmbrasureOverlay : ThingComp
    {
        public CompProperties_EmbrasureOverlay Props => (CompProperties_EmbrasureOverlay)props;

        private Graphic graphicInt;

        private Graphic DefaultGraphic
        {
            get
            {
                if(graphicInt == null)
                {
                        graphicInt = Props.embrasureGraphics.GraphicColoredFor(parent);
                }
                return graphicInt;
            }
        }

        private Graphic Graphic
        {
            get
            {
                return DefaultGraphic;
            }
        }

        public override void PostDraw()
        {
            base.PostDraw();
            MaterialPropertyBlock propertyBlock = new MaterialPropertyBlock();
            if (parent.Stuff != null && parent.Stuff.stuffProps.color != null)
            {
                propertyBlock.SetColor(ShaderPropertyIDs.Color, parent.Stuff.stuffProps.color);
            }
            Matrix4x4 matrix = default(Matrix4x4);
            matrix.SetTRS(parent.DrawPos + new Vector3(0f, 0.01f, 0f), Quaternion.AngleAxis(0, Vector3.up), new Vector3(parent.def.graphicData.drawSize.x, 1, parent.def.graphicData.drawSize.y));
            Graphics.DrawMesh(MeshPool.plane10, matrix, Graphic.MatSingleFor(parent), 0, null, 0, propertyBlock);
        }
    }
}
