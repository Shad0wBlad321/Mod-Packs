using RimWorld;
using RimWorld.Planet;
using UnityEngine;
using Verse;


namespace VanillaPlantsExpandedMorePlants
{
    class Plant_FasterInTropics : Plant
    {

        public override float GrowthRate
		{
			get
			{
				if (Blighted)
				{
					return 0f;
				}
				if (base.Spawned && !PlantUtility.GrowthSeasonNow(base.Position, base.Map))
				{
					return 0f;
				}
				return GrowthRateFactor_Fertility * GrowthRateFactor_Temperature * GrowthRateFactor_Light * GrowthRateFactor_Latitude;
			}
		}

		public float GrowthRateFactor_Latitude
		{
			get
			{
				if(LatitudeSectionUtility.GetReportedLatitudeSection(Find.WorldGrid.LongLatOf(this.Map.Tile).y)== LatitudeSection.Equatorial)
                {
					return 1.3f;
                }
                else
				{
					return 1f;
				}

			}
		}

        public override string GetInspectString()
        {
            if (GrowthRateFactor_Latitude == 1f)
            {
				return base.GetInspectString() + "\n"+"VCE_NotInEquator".Translate(LatitudeSection.Equatorial.GetMaxLatitude());
			}
            else
			{
				return base.GetInspectString() + "\n"+ "VCE_InEquator".Translate();
			}
		}
    }
}
