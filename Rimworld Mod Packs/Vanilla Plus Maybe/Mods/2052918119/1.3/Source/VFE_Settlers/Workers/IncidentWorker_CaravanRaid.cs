using RimWorld;
using RimWorld.Planet;
using System.Linq;
using Verse;

namespace VFE_Settlers.Workers
{
    public class IncidentWorker_CaravanRaid : IncidentWorker
    {
        protected override bool CanFireNowSub(IncidentParms parms)
        {
            return base.CanFireNowSub(parms) && this.TryFindTile(out int num) && this.TryFindFaction(out Faction faction);
        }

        protected override bool TryExecuteWorker(IncidentParms parms)
        {
            if (!this.TryFindFaction(out Faction faction))
            {
                return false;
            }
            if (!this.TryFindTile(out int tile))
            {
                return false;
            }
            Site site = SiteMaker.MakeSite(Defs.SitePartDefOf.CaravanRaid, tile, faction);
            if (site == null)
            {
                return false;
            }
            site.sitePartsKnown = true;
            int randomInRange = SiteTuning.QuestSiteTimeoutDaysRange.RandomInRange;
            site.GetComponent<TimeoutComp>().StartTimeout(randomInRange * 60000);
            Find.WorldObjects.Add(site);
            Find.LetterStack.ReceiveLetter("CaravanRaidLabel".Translate(faction), "CaravanRaidDialogue".Translate(faction), LetterDefOf.PositiveEvent, site, faction, null);
            return true;
        }

        private bool TryFindFaction(out Faction faction)
        {
            faction = Find.FactionManager.AllFactions.Where(x => !x.def.hidden && !x.defeated && !x.IsPlayer && x.def.caravanTraderKinds.Count > 0 && x.HostileTo(Faction.OfPlayer)).RandomElement();
            return faction != null;
        }

        private bool TryFindTile(out int tile)
        {
            return TileFinder.TryFindNewSiteTile(out tile, 7, 27);
        }
    }
}