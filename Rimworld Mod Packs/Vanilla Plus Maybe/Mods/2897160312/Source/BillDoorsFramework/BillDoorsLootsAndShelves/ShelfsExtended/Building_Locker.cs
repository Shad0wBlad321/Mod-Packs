using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.Noise;
using Verse.Sound;
using static RimWorld.FleshTypeDef;

namespace BillDoorsLootsAndShelves
{
    public class DisplayOffsetInfo : IExposable
    {
        public float drawSizeMult = 1;
        public float drawRot;
        public Vector3 drawOffset;
        public bool shouldFlip;
        public string description;
        public bool useSingleText;

        public void Assign(Vector3 drawOffset, float drawSizeMult = 1, float drawRot = 0, bool shouldFlip = false, bool useSingleText = false)
        {
            this.drawSizeMult = drawSizeMult;
            this.drawRot = drawRot;
            this.drawOffset = drawOffset;
            this.shouldFlip = shouldFlip;
            this.useSingleText = useSingleText;
        }

        public void Assign(DisplayOffsetInfo source)
        {
            this.drawSizeMult = source.drawSizeMult;
            this.drawRot = source.drawRot;
            this.drawOffset = source.drawOffset;
            this.shouldFlip = source.shouldFlip;
            this.useSingleText = source.useSingleText;
        }

        public override string ToString()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("BDshelvesDisplayOffsetInfo_Drawsize".Translate() + drawSizeMult.ToString());
            stringBuilder.AppendLine("BDshelvesDisplayOffsetInfo_Rotation".Translate() + drawRot.ToString());
            stringBuilder.AppendLine("BDshelvesDisplayOffsetInfo_DrawOffset".Translate() + drawOffset.ToString());
            stringBuilder.AppendLine("BDshelvesDisplayOffsetInfo_FlipTexture".Translate() + shouldFlip.ToString());
            stringBuilder.AppendLine("BDshelvesDisplayOffsetInfo_SingleTexture".Translate() + useSingleText.ToString());
            return stringBuilder.ToString();
        }

        public void ExposeData()
        {
            Scribe_Values.Look(ref drawSizeMult, "drawSizeMult", 1);
            Scribe_Values.Look(ref drawRot, "drawRot", 0);
            Scribe_Values.Look(ref drawOffset, "drawOffset");
            Scribe_Values.Look(ref shouldFlip, "shouldFlip", false);
            Scribe_Values.Look(ref description, "description");
            Scribe_Values.Look(ref useSingleText, "useSingleText", false);
        }
    }

    public class Building_Locker : Building_ShelfExpanded
    {
        public override int MaxItemsInCell => useOpenTexture ? base.MaxItemsInCell : 0;

        protected Graphic GlassGraphic => def.building.mechGestatorCylinderGraphic?.Graphic;

        protected Graphic OpenGraphic => def.graphicData.GraphicColoredFor(this);

        LockerExtension extension;

        public DisplayOffsetInfo displayInfo;
        public bool isDisplay => extension != null && extension.isDisplay;

        public override void PostPostMake()
        {
            base.PostPostMake();

            if (displayInfo == null)
            {
                displayInfo = new DisplayOffsetInfo();
            }

            extension = def.GetModExtension<LockerExtension>();
            if (extension != null)
            {
                displayInfo.drawSizeMult = extension.drawSizeMult;
                displayInfo.drawOffset = extension.drawOffset;
            }
        }

        public override IEnumerable<IntVec3> AllSlotCells()
        {
            if (!base.Spawned)
            {
                yield break;
            }
            if (isDisplay)
            {
                yield return this.TrueCenter().ToIntVec3();
                yield break;
            }
            foreach (IntVec3 i in base.AllSlotCells())
            {
                yield return i;
            }
        }

        public override void SpawnSetup(Map map, bool respawningAfterLoad)
        {
            base.SpawnSetup(map, respawningAfterLoad);
            extension = def.GetModExtension<LockerExtension>();
        }

        public override void Flick()
        {
            if (useOpenTexture)
            {
                int safety = this.OccupiedRect().Area * MaxItemsInCell;
                while (slotGroup.HeldThings.Any() && safety > 0)
                {
                    safety--;
                    var t = slotGroup.HeldThings.FirstOrDefault();
                    t.DeSpawn();
                    tempStorage.TryAdd(t);
                }
                useOpenTexture = false;
            }
            else
            {
                useOpenTexture = true;
                if (tempStorage.Any())
                {
                    tempStorage.TryDropAll(Position, Map, ThingPlaceMode.Near, nearPlaceValidator: (IntVec3 c) => { return c.IsInside(this); });
                }
            }
            Notify_ColorChanged();
            if (extension != null && extension.sound != null)
            {
                extension.sound.PlayOneShot(new TargetInfo(Position, Map));
            }
        }

        public override void Print(SectionLayer layer)
        {
            if (isDisplay)
            {
                if (!useOpenTexture && tempStorage.Any())
                {
                    PrintOverlay(layer, this, AltitudeLayer.ItemImportant.AltitudeFor(9), LidGraphic);
                    if (GlassGraphic != null)
                    {
                        PrintOverlay(layer, this, AltitudeLayer.ItemImportant.AltitudeFor(8), GlassGraphic);
                    }
                    if (tempStorage.FirstOrDefault() != null)
                    {
                        var content = tempStorage.FirstOrDefault();

                        Graphic graphic = content.DefaultGraphic;

                        Vector2 drawsizeAltered = graphic.drawSize * displayInfo.drawSizeMult;

                        Vector3 center = this.TrueCenter() + displayInfo.drawOffset;
                        center.y = AltitudeLayer.ItemImportant.AltitudeFor(7);
                        Material material = ((!(graphic is Graphic_StackCount graphic_StackCount)) ? graphic.MatSingleFor(content) : graphic_StackCount.SubGraphicForStackCount(displayInfo.useSingleText ? 1 : content.stackCount, content.def).MatSingleFor(content));
                        Graphic.TryGetTextureAtlasReplacementInfo(material, content.def.category.ToAtlasGroup(), displayInfo.shouldFlip, vertexColors: true, out material, out var uvs, out var vertexColor);
                        Printer_Plane.PrintPlane(layer, center, drawsizeAltered, material, AngleFromRot(Rotation) + content.def.equippedAngleOffset + displayInfo.drawRot, false, uvs, new Color32[4] { vertexColor, vertexColor, vertexColor, vertexColor });
                    }
                    PrintOverlay(layer, this, def.altitudeLayer.AltitudeFor(), BottomGraphic);
                }
                else
                {
                    PrintOverlay(layer, this, def.altitudeLayer.AltitudeFor(), OpenGraphic);
                }
            }
            else
            {
                base.Print(layer);
            }
        }

        public override void Destroy(DestroyMode mode = DestroyMode.Vanish)
        {
            if (tempStorage.Count > 0 && (mode == DestroyMode.Deconstruct || mode == DestroyMode.KillFinalize))
            {
                if (mode != DestroyMode.Deconstruct)
                {
                    List<Pawn> list = new List<Pawn>();
                    foreach (Thing item2 in (IEnumerable<Thing>)tempStorage)
                    {
                        if (item2 is Pawn item)
                        {
                            list.Add(item);
                        }
                    }
                    foreach (Pawn item3 in list)
                    {
                        HealthUtility.DamageUntilDowned(item3);
                    }
                }
                tempStorage.TryDropAll(Position, Map, ThingPlaceMode.Near);
            }
            tempStorage.ClearAndDestroyContents();
            base.Destroy(mode);
        }

        public override void TickRare()
        {
            base.TickRare();
            tempStorage.ThingOwnerTickRare();
        }

        public override void Tick()
        {
            base.Tick();
            tempStorage.ThingOwnerTick();
        }

        public override string GetInspectString()
        {
            StringBuilder stringBuilder = new StringBuilder();
            if (!displayInfo.description.NullOrEmpty())
            {
                stringBuilder.AppendLine(displayInfo.description);
            }
            if (!base.GetInspectString().NullOrEmpty())
            {
                stringBuilder.AppendLine(base.GetInspectString());
            }
            string str = !useOpenTexture ? tempStorage.ContentsString : ((string)"BDshelvesEmpty".Translate());
            stringBuilder.Append("CasketContains".Translate() + ": " + str.CapitalizeFirst());
            return stringBuilder.ToString().TrimEnd();
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Deep.Look(ref displayInfo, "displayInfo");
        }

        public override IEnumerable<Gizmo> GetGizmos()
        {
            foreach (Gizmo gizmo2 in base.GetGizmos())
            {
                yield return gizmo2;
            }
        }
    }

    public class LockerExtension : DefModExtension
    {
        public float drawSizeMult = 1;
        public Vector3 drawOffset = new Vector3(0, 0, 0.15f);
        public SoundDef sound;
        public bool isDisplay;
    }
}
