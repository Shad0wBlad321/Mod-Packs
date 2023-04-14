using System.Runtime;
using UnityEngine;
using Verse;

namespace GeneRipper;

public class GeneRipperMod : Mod
{

    private readonly GeneRipperSettings _settings;

    public GeneRipperMod(ModContentPack content) : base(content)
    {
        _settings = GetSettings<GeneRipperSettings>();

    }

    public override void DoSettingsWindowContents(Rect inRect)
    {
        Widgets.Label(inRect with{width = inRect.width / 2, height = 24}, "GeneRipper_ExtractionHours".Translate());
        _settings.ExtractionHours = (int)Widgets.HorizontalSlider(inRect with { x = inRect.width / 2, width = inRect.width / 2, height = 24 }, _settings.ExtractionHours, 24, 72, true, $"{_settings.ExtractionHours} h", "24 h", "72 h", 1f);
        TooltipHandler.TipRegion(inRect with {height = 24}, "GeneRipper_ExtractionHoursTooltip".Translate());

        Widgets.Label(inRect with { y = inRect.y + 30, width = inRect.width / 2, height = 24 }, "GeneRipper_BlendingChance".Translate());
        _settings.BlendingChance = Widgets.HorizontalSlider(inRect with { x = inRect.width / 2, y = inRect.y + 30, width = inRect.width / 2, height = 24 }, _settings.BlendingChance, 0.5f, 1f, true, $"{_settings.BlendingChance * 100f}%", "50%", "100%", 0.1f);
        TooltipHandler.TipRegion(inRect with {y = inRect.y + 30, height = 24}, "GeneRipper_BlendingChanceTooltip".Translate());

        base.DoSettingsWindowContents(inRect);
    }


    public override string SettingsCategory()
    {
        return "GeneRipper".Translate();
    }
}