using Verse;
using UnityEngine;
using System.Collections.Generic;
using RimWorld;
using System;
using System.Xml;
using HarmonyLib;

namespace VPEAutoPlow
{
	public class VPEAutoPlowSettings : ModSettings
	{
		public bool disableCompatSettings = true;

		public override void ExposeData()
		{
			Scribe_Values.Look(ref disableCompatSettings, "disableCompatSettings");
			base.ExposeData();
		}
	}

	public class VPEAutoPlowMod : Mod
	{
		private readonly BackCompatibilityConverter_VPE_AutoPlow converter;

		public VPEAutoPlowMod(ModContentPack content) : base(content)
		{
			this.converter = new BackCompatibilityConverter_VPE_AutoPlow();
			var conversionChain = AccessTools.StaticFieldRefAccess<List<BackCompatibilityConverter>>(typeof(BackCompatibility), "conversionChain");
			conversionChain.Add(this.converter);
			new Harmony(this.Content.PackageIdPlayerFacing).PatchAll(); // "Archoran.Utils.VPEAutoPlow"
		}

		public override void DoSettingsWindowContents(Rect inRect)
		{
			VPEAutoPlowSettings settings = GetSettings<VPEAutoPlowSettings>();
			Listing_Standard listingStandard = new Listing_Standard();
			listingStandard.Begin(inRect);
			listingStandard.CheckboxLabeled("VPEAutoPlow.CompatLabel".Translate(), ref settings.disableCompatSettings, "VPEAutoPlow.CompatTooltip".Translate());
			listingStandard.End();
			base.DoSettingsWindowContents(inRect);
		}

		public override void WriteSettings()
		{
			VPEAutoPlowSettings settings = GetSettings<VPEAutoPlowSettings>();
			this.converter.enabled = !settings.disableCompatSettings;
			base.WriteSettings();
		}

		public override string SettingsCategory()
		{
			return "VPEAutoPlow.ModName".Translate();
		}


		internal class BackCompatibilityConverter_VPE_AutoPlow : BackCompatibilityConverter
		{
			public bool enabled = true;
			public override bool AppliesToVersion(int majorVer, int minorVer)
			{
				return enabled;
			}

			public override string BackCompatibleDefName(Type defType, string defName, bool forDefInjections = false, XmlNode node = null)
			{
				return null;
			}

			public override Type GetBackCompatibleType(Type baseType, string providedClassName, XmlNode node)
			{
				if (providedClassName == "VPE_AutoPlow.Zone_Growing")
				{
					return typeof(Zone_Growing);
				}
				return null;
			}

			public override void PostExposeData(object obj)
			{
			}
		}
	}

	[DefOf]
	public static class VPEAutoPlowDefOf
	{
		public static TerrainDef VCE_TilledSoil;
		static VPEAutoPlowDefOf()
		{
			DefOfHelper.EnsureInitializedInCtor(typeof(TerrainDefOf));
		}
	}

	[StaticConstructorOnStartup]
	public static class VPEAutoPlowTextures
	{
		public static readonly Texture2D VCE_Plow = ContentFinder<Texture2D>.Get("UI/VCE_Plow");
	}
}
