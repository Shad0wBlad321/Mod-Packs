using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;
using Verse.AI;
using Verse.Grammar;
using Verse.Sound;
using static Verse.Widgets;

namespace Rimdeed
{
	public static class RimdeedTools
	{
        public static TaggedString GenerateTextFromRule(RulePackDef rule, int seed = -1)
        {
            if (seed != -1)
            {
                Rand.PushState();
                Rand.Seed = seed;
            }
            string rootKeyword = rule.RulesPlusIncludes.Where(x => x.keyword == "r_logentry").RandomElement().keyword;
            GrammarRequest request = default(GrammarRequest);
            request.Includes.Add(rule);
            string str = GrammarResolver.Resolve(rootKeyword, request);
            if (seed != -1)
            {
                Rand.PopState();
            }
            return str;
        }
    }
}
