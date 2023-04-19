
using UnityEngine;
using Verse;
using RimWorld;

namespace VanillaPlantsExpandedMorePlants
{
    public class Designator_AquaticGrowingZone : Designator_ZoneAdd
    {
        protected override string NewZoneLabel
        {
            get
            {
                return "VCE_AquaticGrowingZone".Translate();
            }
        }

        public Designator_AquaticGrowingZone()
        {
           
            this.zoneTypeToPlace = typeof(Zone_GrowingAquatic);
            this.defaultLabel = "VCE_AquaticGrowingZone".Translate();
            this.defaultDesc = "VCE_AquaticGrowingZoneDesc".Translate();
            this.icon = ContentFinder<Texture2D>.Get("UI/Designators/VCE_ZoneCreate_Aquatic", true);
            this.hotKey = KeyBindingDefOf.Misc2;
           
            this.tutorTag = "ZoneAdd_Growing";
        }

        public override AcceptanceReport CanDesignateCell(IntVec3 c)
        {
            if (!base.CanDesignateCell(c).Accepted)
            {
                return false;
            }
            TerrainDef terrainDef = Map.terrainGrid.TerrainAt(c);
            foreach (AquaticGrowZoneTerrainsDef element in DefDatabase<AquaticGrowZoneTerrainsDef>.AllDefs)
            {
                foreach (string allowed in element.allowedTerrains)
                {
                    if ((allowed == terrainDef.defName) && c.Walkable(Map))
                    {
                        return true;

                    }
               

                }
            }
            return false;
        }

      

        protected override Zone MakeNewZone()
        {
            PlayerKnowledgeDatabase.KnowledgeDemonstrated(ConceptDefOf.GrowingFood, KnowledgeAmount.Total);
            return new Zone_GrowingAquatic(Find.CurrentMap.zoneManager);
        }
    }
}
