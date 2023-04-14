using HarmonyLib;
using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;
using Verse.AI;
using Verse.Grammar;

namespace Rimdeed
{
    public class PawnTrader : IExposable, ITrader, IThingHolder
    {
        private ThingOwner things;

        private List<Pawn> soldPrisoners = new List<Pawn>();

        private int randomPriceFactorSeed = -1;
        public int Silver => CountHeldOf(ThingDefOf.Silver);
        public TradeCurrency TradeCurrency => TraderKind.tradeCurrency;
        public IThingHolder ParentHolder => null;

        public TraderKindDef TraderKind => RD_DefOf.RD_PawnTrader;

        public int RandomPriceFactorSeed => randomPriceFactorSeed;

        public float TradePriceImprovementOffsetForPlayer => 0f;

        public IEnumerable<Thing> Goods
        {
            get
            {
                for (int i = 0; i < things.Count; i++)
                {
                    yield return things[i];
                }
            }
        }

        public string TraderName => "RimDeed.TraderName".Translate();
        public bool CanTradeNow => true;
        public Faction Faction => null;

        public int lastStockTick = 0;
        public PawnTrader()
        {
            things = new ThingOwner<Thing>(this);
            randomPriceFactorSeed = Rand.RangeInclusive(1, 10000000);
        }

        public void StartTrade(Pawn negotiator)
        {
            if (lastStockTick == 0 || Find.TickManager.TicksGame > lastStockTick + (GenDate.TicksPerDay * 14))
            {
                things.ClearAndDestroyContents();
                GenerateThings(negotiator);
                lastStockTick = Find.TickManager.TicksGame;
            }
        }
        public IEnumerable<Thing> ColonyThingsWillingToBuy(Pawn playerNegotiator)
        {
            foreach (Thing item in TradeUtility.AllLaunchableThingsForTrade(playerNegotiator.Map, this))
            {
                yield return item;
            }
            foreach (Pawn item2 in AllSellableColonyPawns(playerNegotiator))
            {
                yield return item2;
            }
        }

        public IEnumerable<Pawn> AllSellableColonyPawns(Pawn negotiator)
        {
            foreach (Pawn item in negotiator.Map.mapPawns.PrisonersOfColonySpawned)
            {
                if (item.guest?.PrisonerIsSecure ?? false)
                {
                    item.guest.joinStatus = JoinStatus.JoinAsColonist;
                    yield return item;
                }
            }

            foreach (Pawn item2 in negotiator.Map.mapPawns.SpawnedPawnsInFaction(Faction.OfPlayer))
            {
                if (negotiator != item2 && (item2.IsColonistPlayerControlled && item2.HostFaction == null && !item2.InMentalState && !item2.Downed || item2.IsSlave))
                {
                    if (item2.guest != null)
                    {
                        item2.guest.joinStatus = JoinStatus.JoinAsColonist;
                    }
                    yield return item2;
                }
            }
        }

        public void GenerateThings(Pawn playerNegotiator)
        {
            ThingSetMakerParams parms = default(ThingSetMakerParams);
            parms.traderDef = TraderKind;
            parms.tile = playerNegotiator.Map.Tile;
            int count = 0;
            while (count < 100)
            {
                count++;
                try
                {
                    var generatedThings = ThingSetMakerDefOf.TraderStock.root.Generate(parms);
                    foreach (var pawn in generatedThings.OfType<Pawn>())
                    {
                        for (int num = pawn.health.hediffSet.hediffs.Count - 1; num >= 0; num--)
                        {
                            if (pawn.health.hediffSet.hediffs[num].def.isBad)
                            {
                                pawn.health.hediffSet.hediffs.RemoveAt(num);
                            }
                        }
                        pawn.guest.joinStatus = JoinStatus.JoinAsColonist;
                    }
                    things.TryAddRangeOrTransfer(generatedThings);
                    break;
                }
                catch(Exception ex)
                {
                    Log.Message("Error when generating trader stock: " + ex);
                }
            }

        }

        public void TraderTick()
        {
            for (int num = things.Count - 1; num >= 0; num--)
            {
                Pawn pawn = things[num] as Pawn;
                if (pawn != null)
                {
                    pawn.Tick();
                    if (pawn.Dead)
                    {
                        things.Remove(pawn);
                    }
                }
            }
        }

        public void ExposeData()
        {
            Scribe_Deep.Look(ref things, "things", this);
            Scribe_Collections.Look(ref soldPrisoners, "soldPrisoners", LookMode.Reference);
            Scribe_Values.Look(ref randomPriceFactorSeed, "randomPriceFactorSeed", 0);
            Scribe_Values.Look(ref lastStockTick, "lastStockTick");
            if (Scribe.mode == LoadSaveMode.PostLoadInit)
            {
                soldPrisoners.RemoveAll((Pawn x) => x == null);
            }
        }
        public int CountHeldOf(ThingDef thingDef, ThingDef stuffDef = null)
        {
            return HeldThingMatching(thingDef, stuffDef)?.stackCount ?? 0;
        }

        public void GiveSoldThingToTrader(Thing toGive, int countToGive, Pawn playerNegotiator)
        {
            Thing thing = toGive.SplitOff(countToGive);
            thing.PreTraded(TradeAction.None, playerNegotiator, this);
            Thing thing2 = TradeUtility.ThingFromStockToMergeWith(this, thing);
            if (thing2 != null)
            {
                if (!thing2.TryAbsorbStack(thing, respectStackLimit: false))
                {
                    thing.Destroy();
                }
                return;
            }
            Pawn pawn = thing as Pawn;
            if (pawn != null && pawn.RaceProps.Humanlike)
            {
                soldPrisoners.Add(pawn);
                foreach (Pawn otherPawn in PawnsFinder.AllMapsCaravansAndTravelingTransportPods_Alive_FreeColonistsAndPrisoners)
                {
                    if (pawn != otherPawn && otherPawn.needs.mood != null)
                    {
                        otherPawn.needs.mood.thoughts.memories.TryGainMemory(RD_DefOf.RD_TradedInToRimdeed);
                    }
                }
            }
            things.TryAddOrTransfer(thing, canMergeWithExistingStacks: false);
        }

        public void GiveSoldThingToPlayer(Thing toGive, int countToGive, Pawn playerNegotiator)
        {
            Thing thing = toGive.SplitOff(countToGive);
            thing.PreTraded(TradeAction.None, playerNegotiator, this);
            Pawn pawn = thing as Pawn;
            if (pawn != null)
            {
                pawn.needs.mood.thoughts.memories.TryGainMemory(RD_DefOf.RD_TradedInFromRimdeed);
                pawn.SetFaction(Faction.OfPlayer);
                soldPrisoners.Remove(pawn);
            }
            TradeUtility.SpawnDropPod(DropCellFinder.TradeDropSpot(playerNegotiator.Map), playerNegotiator.Map, thing);
        }

        private Thing HeldThingMatching(ThingDef thingDef, ThingDef stuffDef)
        {
            for (int i = 0; i < things.Count; i++)
            {
                if (things[i].def == thingDef && things[i].Stuff == stuffDef)
                {
                    return things[i];
                }
            }
            return null;
        }

        public void ChangeCountHeldOf(ThingDef thingDef, ThingDef stuffDef, int count)
        {
            Thing thing = HeldThingMatching(thingDef, stuffDef);
            if (thing == null)
            {
                Log.Error("Changing count of thing trader doesn't have: " + thingDef);
            }
            thing.stackCount += count;
        }

        public ThingOwner GetDirectlyHeldThings()
        {
            return things;
        }

        public void GetChildHolders(List<IThingHolder> outChildren)
        {
            ThingOwnerUtility.AppendThingHoldersFromThings(outChildren, GetDirectlyHeldThings());
        }
    }
}