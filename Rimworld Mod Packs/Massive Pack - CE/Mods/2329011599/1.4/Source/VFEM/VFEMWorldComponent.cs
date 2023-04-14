﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VFEMech
{
    using RimWorld;
    using RimWorld.Planet;
    using Verse;
    using VFEM;

    public class VFEMWorldComponent : WorldComponent
    {
        private int lastMechShipSpawn;
        private int nextMechShipSpawn;

        public VFEMWorldComponent(World world) : base(world)
        {
        }
        public override void FinalizeInit()
        {
            base.FinalizeInit();
            Faction faction = Find.FactionManager.FirstFactionOfDef(VFEMDefOf.VFE_Mechanoid);
            if (faction is null)
            {
                faction = FactionGenerator.NewGeneratedFaction(new FactionGeneratorParms(VFEMDefOf.VFE_Mechanoid));
                Find.FactionManager.Add(faction);
            }
        }

        public static List<KeyValuePair<IncidentDef, float>> cachedIncidents = new List<KeyValuePair<IncidentDef, float>>();
        public override void WorldComponentTick()
        {
            base.WorldComponentTick();

            if (!MechShipsMod.settings.totalWarIsDisabled)
            {
                if (Find.TickManager.TicksGame >= this.nextMechShipSpawn && Find.TickManager.TicksGame % 60 == 0)
                {
                    this.lastMechShipSpawn = Find.TickManager.TicksGame;

                    IncidentParms parms = StorytellerUtility.DefaultParmsNow(IncidentCategoryDefOf.ThreatBig, this.world);
                    if (cachedIncidents.Any() is false)
                    {
                        cachedIncidents = MechShipsMod.settings.mechShipIncidentChances
                            .Select(kvp => new KeyValuePair<IncidentDef, float>(DefDatabase<IncidentDef>.GetNamed(kvp.Key, false), kvp.Value))
                            .Where(kvp => kvp.Key != null).ToList();
                    }

                    IEnumerable<KeyValuePair<IncidentDef, float>> incidents = cachedIncidents.Where(kvp => kvp.Key.Worker.CanFireNow(parms));
                    if (incidents.Any() && incidents.TryRandomElementByWeight(kvp => kvp.Value, out KeyValuePair<IncidentDef, float> incident))
                    {
                        incident.Key.Worker.TryExecute(parms);
                        this.nextMechShipSpawn = Find.TickManager.TicksGame + MechShipsMod.settings.mechShipTimeInterval.RandomInRange;
                    }
                }
                else if(Find.TickManager.TicksGame % GenDate.TicksPerDay == 0 && (this.nextMechShipSpawn - this.lastMechShipSpawn) > MechShipsMod.settings.mechShipTimeInterval.max)
                {
                    this.nextMechShipSpawn = this.lastMechShipSpawn + MechShipsMod.settings.mechShipTimeInterval.RandomInRange;
                }
            }
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref this.lastMechShipSpawn,    "VFEM_" + nameof(this.lastMechShipSpawn));
            Scribe_Values.Look(ref this.nextMechShipSpawn, "VFEM_" + nameof(this.nextMechShipSpawn), 0);
        }
    }
}
