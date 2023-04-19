using System;
using UnityEngine;
using Verse;
using RimWorld;

namespace VanillaPlantsExpandedMorePlants
{
    public class Designator_SandyGrowingZone : Designator_ZoneAdd
    {
        protected override string NewZoneLabel
        {
            get
            {
                return "VCE_SandyGrowingZone".Translate();
            }
        }

        public Designator_SandyGrowingZone()
        {
           
            this.zoneTypeToPlace = typeof(Zone_GrowingSandy);
            this.defaultLabel = "VCE_SandyGrowingZone".Translate();
            this.defaultDesc = "VCE_SandyGrowingZoneDesc".Translate();
            this.icon = ContentFinder<Texture2D>.Get("UI/Designators/VCE_ZoneCreate_Sandy", true);
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
            foreach (SandyGrowZoneTerrainsDef element in DefDatabase<SandyGrowZoneTerrainsDef>.AllDefs)
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
            return new Zone_GrowingSandy(Find.CurrentMap.zoneManager);
        }
    }
}
