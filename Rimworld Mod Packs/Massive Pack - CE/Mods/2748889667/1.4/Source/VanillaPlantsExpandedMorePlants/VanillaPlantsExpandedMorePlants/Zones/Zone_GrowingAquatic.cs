
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine;
using Verse;
using RimWorld;


namespace VanillaPlantsExpandedMorePlants
{
    public class Zone_GrowingAquatic : Zone, IPlantToGrowSettable
    {
        private ThingDef plantDefToGrow = InternalDefOf.VCE_Taro;

        public bool allowSow = true;

        public bool allowCut = true;

        public override bool IsMultiselectable => true;

        private static List<Color> aquaticZoneColors = new List<Color>();

        private static int nextAquaticZoneColorIndex = 0;

        IEnumerable<IntVec3> IPlantToGrowSettable.Cells
        {
            get
            {
                return base.Cells;
            }
        }

        protected override Color NextZoneColor
        {
            get
            {

                return NextAquaticZoneColor();


            }
        }

        private static IEnumerable<Color> AquaticZoneColors()
        {
            yield return Color.Lerp(new Color(0f, 0f, 1f), Color.gray, 0.5f);
            yield return Color.Lerp(new Color(0f, 0.5f, 1f), Color.gray, 0.5f);
            yield return Color.Lerp(new Color(0f, 0.25f, 1f), Color.gray, 0.5f);
            yield return Color.Lerp(new Color(0.25f, 0.25f, 0.75f), Color.gray, 0.5f);
            yield return Color.Lerp(new Color(0.25f, 0f, 0.65f), Color.gray, 0.5f);
            yield break;
        }

        public static Color NextAquaticZoneColor()
        {
            aquaticZoneColors.Clear();
            foreach (Color color in AquaticZoneColors())
            {
                Color item = new Color(color.r, color.g, color.b, 0.09f);
                aquaticZoneColors.Add(item);

            }
            Color result = aquaticZoneColors[nextAquaticZoneColorIndex];
            nextAquaticZoneColorIndex++;
            if (nextAquaticZoneColorIndex >= aquaticZoneColors.Count)
            {
                nextAquaticZoneColorIndex = 0;
            }
            return result;
        }

        public Zone_GrowingAquatic()
        {
           
           
        }

        public Zone_GrowingAquatic(ZoneManager zoneManager) : base("VCE_AquaticGrowingZone".Translate(), zoneManager)
        {
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Defs.Look<ThingDef>(ref this.plantDefToGrow, "plantDefToGrow");
            Scribe_Values.Look<bool>(ref this.allowSow, "allowSow", true, false);
            Scribe_Values.Look(ref allowCut, "allowCut", defaultValue: true);
        }

        public override string GetInspectString()
        {
            string text = string.Empty;
            if (!base.Cells.NullOrEmpty<IntVec3>())
            {
                IntVec3 c = base.Cells.First<IntVec3>();
                if (c.UsesOutdoorTemperature(base.Map))
                {
                    string text2 = text;
                    text = string.Concat(new string[]
                    {
                        text2,
                        "OutdoorGrowingPeriod".Translate(),
                        ": ",
                        Zone_Growing.GrowingQuadrumsDescription(base.Map.Tile),
                        "\n"
                    });
                }
                if (PlantUtility.GrowthSeasonNow(c, base.Map, true))
                {
                    text += "GrowSeasonHereNow".Translate();
                }
                else
                {
                    text += "CannotGrowBadSeasonTemperature".Translate();
                }
            }
            return text;
        }

        public static string GrowingQuadrumsDescription(int tile)
        {
            List<Twelfth> list = GenTemperature.TwelfthsInAverageTemperatureRange(tile, 10f, 42f);
            if (list.NullOrEmpty<Twelfth>())
            {
                return "NoGrowingPeriod".Translate();
            }
            if (list.Count == 12)
            {
                return "GrowYearRound".Translate();
            }
            return "PeriodDays".Translate(list.Count * 5 + "/" + 60) + " (" + QuadrumUtility.QuadrumsRangeLabel(list) + ")";
        }

        public override void AddCell(IntVec3 c)
        {
            base.AddCell(c);
            foreach (Thing item in base.Map.thingGrid.ThingsListAt(c))
            {
                Designator_PlantsHarvestWood.PossiblyWarnPlayerImportantPlantDesignateCut(item);
            }
        }

        [DebuggerHidden]
        public override IEnumerable<Gizmo> GetGizmos()
        {
            foreach (Gizmo g in base.GetGizmos())
            {
                yield return g;
            }
            
                yield return PlantToGrowSettableUtility.SetPlantToGrowCommand(this);
                yield return new Command_Toggle
                {
                    defaultLabel = "CommandAllowSow".Translate(),
                    defaultDesc = "CommandAllowSowDesc".Translate(),
                    hotKey = KeyBindingDefOf.Command_ItemForbid,
                    icon = TexCommand.ForbidOff,
                    isActive = (() => this.allowSow),
                    toggleAction = delegate
                    {
                        this.allowSow = !this.allowSow;
                    }
                };
                Command_Toggle command_Toggle2 = new Command_Toggle();
                command_Toggle2.defaultLabel = "CommandAllowCut".Translate();
                command_Toggle2.defaultDesc = "CommandAllowCutDesc".Translate();
                command_Toggle2.icon = Designator_PlantsCut.IconTex;
                command_Toggle2.isActive = (() => allowCut);
                command_Toggle2.toggleAction = delegate
                {
                    allowCut = !allowCut;
                };
                yield return command_Toggle2;
            
                
        }

        [DebuggerHidden]
        public override IEnumerable<Gizmo> GetZoneAddGizmos()
        {
            yield return DesignatorUtility.FindAllowedDesignator<Designator_AquaticGrowingZone_Expand>();
        }

        public ThingDef GetPlantDefToGrow()
        {
            return plantDefToGrow;

        }

        public void SetPlantDefToGrow(ThingDef plantDef)
        {
            this.plantDefToGrow = plantDef;
        }

        public bool CanAcceptSowNow()
        {
            return true;
        }
    }
}

