using System;
using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;

namespace GeneRipper;

public class Dialog_SelectGene : Window
{
    private const float HeaderHeight = 30f;


    private static readonly List<GeneDef> geneDefs = new();
    private static readonly List<Gene> xenogenes = new();
    private static readonly List<Gene> endogenes = new();
    private static float xenogenesHeight;
    private static float endogenesHeight;
    private static float scrollHeight;
    private static int gcx;
    private static int met;
    private static int arc;
    private static readonly Color CapsuleBoxColor = new(0.25f, 0.25f, 0.25f);
    private static readonly Color CapsuleBoxColorOverridden = new(0.15f, 0.15f, 0.15f);
    private static readonly CachedTexture GeneBackground_Archite = new CachedTexture("UI/Icons/Genes/GeneBackground_ArchiteGene");
    private static readonly CachedTexture GeneBackground_Xenogene = new CachedTexture("UI/Icons/Genes/GeneBackground_Xenogene");
    private static readonly CachedTexture GeneBackground_Endogene = new CachedTexture("UI/Icons/Genes/GeneBackground_Endogene");
    private readonly Action<Pawn, GeneDef>? acceptAction;
    private readonly Action? cancelAction;
    private Vector2 scrollPosition;
    private readonly Pawn target;

    public Dialog_SelectGene(Pawn target, Action<Pawn, GeneDef> acceptAction = null, Action cancelAction = null)
    {
        this.target = target;
        this.acceptAction = acceptAction;
        this.cancelAction = cancelAction;
    }

    public GeneDef? SelectedGene { get; private set; }


    public override Vector2 InitialSize => new(736f, 700f);

    public override void PostOpen()
    {
        if (!ModLister.CheckBiotech("genes viewing"))
            Close(false);
        else
            base.PostOpen();
    }

    public override void DoWindowContents(Rect inRect)
    {
        inRect.yMax -= CloseButSize.y;
        var rect = inRect;
        rect.xMin += 34f;
        Text.Font = GameFont.Medium;
        Widgets.Label(rect, "ViewGenes".Translate() + ": " + target.genes.XenotypeLabelCap);
        Text.Font = GameFont.Small;
        GUI.color = XenotypeDef.IconColor;
        GUI.DrawTexture(new Rect(inRect.x, inRect.y, 30f, 30f), target.genes.XenotypeIcon);
        GUI.color = Color.white;
        inRect.yMin += 34f;
        var zero = Vector2.zero;
        DrawGenesInfo(inRect, target, InitialSize.y, ref zero, ref scrollPosition);
        if (Widgets.ButtonText(
                new Rect(inRect.xMax - CloseButSize.x, inRect.yMax, CloseButSize.x,
                    CloseButSize.y), "Cancel".Translate()))
        {
            cancelAction?.Invoke();
            Close();
        }

        if (Widgets.ButtonText(
                new Rect(inRect.xMax - CloseButSize.x * 2 - 6, inRect.yMax, CloseButSize.x,
                    CloseButSize.y), "GeneRipper_Select".Translate(), active: SelectedGene != null))
        {
            acceptAction?.Invoke(target, SelectedGene!);
            Close();
        }
    }


    private void DrawGenesInfo(
        Rect rect,
        Thing target,
        float initialHeight,
        ref Vector2 size,
        ref Vector2 scrollPosition,
        GeneSet pregnancyGenes = null)
    {
        var rect1 = rect;
        var position = rect1.ContractedBy(10f);
        GUI.BeginGroup(position);
        var height = BiostatsTable.HeightForBiostats(arc);
        var rect2 = new Rect(0.0f, 0.0f, position.width, (float)(position.height - (double)height - 12.0));
        DrawGeneSections(rect2, target, pregnancyGenes, ref scrollPosition);
        var rect3 = new Rect(0.0f, rect2.yMax + 6f, (float)(position.width - 140.0 - 4.0), height);
        rect3.yMax = (float)(rect2.yMax + (double)height + 6.0);
        if (!(target is Pawn))
            rect3.width = position.width;
        //BiostatsTable.Draw(rect3, gcx, met, arc, false, false);
        //TryDrawXenotype(target, rect3.xMax + 4f, rect3.y + Text.LineHeight / 2f);
        if (Event.current.type == EventType.Layout)
        {
            var a = (float)(endogenesHeight + (double)xenogenesHeight + height + 12.0 + 70.0);
            size.y = a <= (double)initialHeight
                ? initialHeight
                : Mathf.Min(a, (float)(UI.screenHeight - 35 - 165.0 - 30.0));
            xenogenesHeight = 0.0f;
            endogenesHeight = 0.0f;
        }

        GUI.EndGroup();
    }

    private void DrawGeneSections(
        Rect rect,
        Thing target,
        GeneSet genesOverride,
        ref Vector2 scrollPosition)
    {
        RecacheGenes(target, genesOverride);
        GUI.BeginGroup(rect);
        var viewRect = new Rect(0.0f, 0.0f, rect.width - 16f, scrollHeight);
        var curY = 0.0f;
        Widgets.BeginScrollView(rect.AtZero(), ref scrollPosition, viewRect);
        var containingRect = viewRect with
        {
            y = scrollPosition.y,
            height = rect.height
        };
        if (target is Pawn)
        {
            if (endogenes.Any())
            {
                DrawSection(rect, false, endogenes.Count, ref curY, ref endogenesHeight,
                    (i, r) => DrawGene(endogenes[i], r, GeneType.Endogene), containingRect);
                curY += 12f;
            }

            DrawSection(rect, true, xenogenes.Count, ref curY, ref xenogenesHeight,
                (i, r) => DrawGene(xenogenes[i], r, GeneType.Xenogene), containingRect);
        }
        else
        {
            var geneType = genesOverride != null || target is HumanEmbryo ? GeneType.Endogene : GeneType.Xenogene;
            DrawSection(rect, geneType == GeneType.Xenogene, geneDefs.Count, ref curY, ref xenogenesHeight,
                (i, r) => DrawGeneDef(geneDefs[i], r, geneType, null), containingRect);
        }

        if (Event.current.type == EventType.Layout)
            scrollHeight = curY;
        Widgets.EndScrollView();
        GUI.EndGroup();
    }

    private void RecacheGenes(Thing target, GeneSet genesOverride)
    {
        geneDefs.Clear();
        xenogenes.Clear();
        endogenes.Clear();
        gcx = 0;
        met = 0;
        arc = 0;
        var pawn = target as Pawn;
        var geneSet = (target is GeneSetHolderBase geneSetHolderBase ? geneSetHolderBase.GeneSet : null) ??
                      genesOverride;
        if (pawn != null)
        {
            foreach (var xenogene in pawn.genes.Xenogenes)
            {
                if (!xenogene.Overridden)
                    AddBiostats(xenogene.def);
                xenogenes.Add(xenogene);
            }

            foreach (var endogene in pawn.genes.Endogenes)
                if (endogene.def.endogeneCategory != EndogeneCategory.Melanin ||
                    !pawn.genes.Endogenes.Any((Predicate<Gene>)(x => x.def.skinColorOverride.HasValue)))
                {
                    if (!endogene.Overridden)
                        AddBiostats(endogene.def);
                    endogenes.Add(endogene);
                }

            xenogenes.SortGenes();
            endogenes.SortGenes();
        }
        else
        {
            if (geneSet == null)
                return;
            foreach (var geneDef in geneSet.GenesListForReading)
                geneDefs.Add(geneDef);
            gcx = geneSet.ComplexityTotal;
            met = geneSet.MetabolismTotal;
            arc = geneSet.ArchitesTotal;
            geneDefs.SortGeneDefs();
        }

        static void AddBiostats(GeneDef gene)
        {
            gcx += gene.biostatCpx;
            met += gene.biostatMet;
            arc += gene.biostatArc;
        }
    }

    private void DrawSection(
        Rect rect,
        bool xeno,
        int count,
        ref float curY,
        ref float sectionHeight,
        Action<int, Rect> drawer,
        Rect containingRect)
    {
        Widgets.Label(10f, ref curY, rect.width,
            (string)(xeno ? "Xenogenes" : "Endogenes").Translate().CapitalizeFirst(),
            (xeno ? "XenogenesDesc" : "EndogenesDesc").Translate());
        var num1 = curY;
        var rect1 = new Rect(rect.x, curY, rect.width, sectionHeight);
        if (xeno && count == 0)
        {
            Text.Anchor = TextAnchor.UpperCenter;
            GUI.color = ColoredText.SubtleGrayColor;
            rect1.height = Text.LineHeight;
            Widgets.Label(rect1, "(" + "NoXenogermImplanted".Translate() + ")");
            GUI.color = Color.white;
            Text.Anchor = TextAnchor.UpperLeft;
            curY += 90f;
        }
        else
        {
            Widgets.DrawMenuSection(rect1);
            var num2 = (float)((rect.width - 12.0 - 630.0 - 36.0) / 2.0);
            curY += num2;
            var num3 = 0;
            var num4 = 0;
            for (var index = 0; index < count; ++index)
            {
                if (num4 >= 6)
                {
                    num4 = 0;
                    ++num3;
                }
                else if (index > 0)
                {
                    ++num4;
                }

                var other = new Rect((float)(num2 + num4 * 90.0 + num4 * 6.0), (float)(curY + num3 * 90.0 + num3 * 6.0),
                    90f, 90f);
                if (containingRect.Overlaps(other))
                    drawer(index, other);
            }

            curY += (float)((num3 + 1) * 90.0 + num3 * 6.0) + num2;
        }

        if (Event.current.type != EventType.Layout)
            return;
        sectionHeight = curY - num1;
    }

    private void TryDrawXenotype(Thing target, float x, float y)
    {
        var sourcePawn = target as Pawn;
        if (sourcePawn == null)
            return;
        var rect = new Rect(x, y, 140f, Text.LineHeight);
        Text.Anchor = TextAnchor.UpperCenter;
        Widgets.Label(rect, sourcePawn.genes.XenotypeLabelCap);
        Text.Anchor = TextAnchor.UpperLeft;
        var position = new Rect(rect.center.x - 17f, rect.yMax + 4f, 34f, 34f);
        GUI.color = XenotypeDef.IconColor;
        GUI.DrawTexture(position, sourcePawn.genes.XenotypeIcon);
        GUI.color = Color.white;
        rect.yMax = position.yMax;
        if (Mouse.IsOver(rect))
        {
            Widgets.DrawHighlight(rect);
            TooltipHandler.TipRegion(rect,
                (Func<string>)(() =>
                    ("Xenotype".Translate() + ": " + sourcePawn.genes.XenotypeLabelCap).Colorize(ColoredText
                        .TipSectionTitleColor) + "\n\n" + sourcePawn.genes.XenotypeDescShort), 883938493);
        }

        if (!Widgets.ButtonInvisible(rect) || sourcePawn.genes.UniqueXenotype)
            return;
        Find.WindowStack.Add(new Dialog_InfoCard(sourcePawn.genes.Xenotype));
    }

    public void DrawGene(
        Gene gene,
        Rect geneRect,
        GeneType geneType,
        bool doBackground = true)
    {
        DrawGeneBasics(gene.def, geneRect, geneType, doBackground, !gene.Active);
        if (!Mouse.IsOver(geneRect))
            return;
        var tip = gene.LabelCap.Colorize(ColoredText.TipSectionTitleColor) + "\n\n" + gene.def.DescriptionFull;
        if (gene.Overridden)
        {
            var str = tip + "\n\n";
            tip = gene.overriddenByGene.def != gene.def
                ? str + ("OverriddenByGene".Translate() + ": " + gene.overriddenByGene.LabelCap).Colorize(ColorLibrary
                    .RedReadable)
                : str + ("OverriddenByIdenticalGene".Translate() + ": " + gene.overriddenByGene.LabelCap).Colorize(
                    ColorLibrary.RedReadable);
        }

        TooltipHandler.TipRegion(geneRect, (TipSignal)tip);
    }

    public void DrawGeneDef(
        GeneDef gene,
        Rect geneRect,
        GeneType geneType,
        string extraTooltip,
        bool doBackground = true,
        bool overridden = false)
    {
        DrawGeneBasics(gene, geneRect, geneType, doBackground, overridden);
        if (!Mouse.IsOver(geneRect))
            return;
        TooltipHandler.TipRegion(geneRect, (Func<string>)(() =>
        {
            var str = gene.LabelCap.Colorize(ColoredText.TipSectionTitleColor) + "\n\n" + gene.DescriptionFull;
            if (!extraTooltip.NullOrEmpty())
                str = str + "\n\n" + extraTooltip.Colorize(ColorLibrary.RedReadable);
            return str;
        }), 316238373);
    }

    private void DrawGeneBasics(
        GeneDef gene,
        Rect geneRect,
        GeneType geneType,
        bool doBackground,
        bool overridden)
    {
        GUI.BeginGroup(geneRect);
        var rect1 = geneRect.AtZero();
        if (doBackground)
        {
            Widgets.DrawHighlight(rect1);
            GUI.color = new Color(1f, 1f, 1f, 0.05f);
            Widgets.DrawBox(rect1);
            GUI.color = Color.white;
        }

        var num = rect1.width - Text.LineHeight;
        var rect2 = new Rect((float)(geneRect.width / 2.0 - num / 2.0), 0.0f, num, num);
        var iconColor = gene.IconColor;
        if (overridden)
        {
            iconColor.a = 0.75f;
            GUI.color = ColoredText.SubtleGrayColor;
        }

        var cachedTexture = GeneBackground_Archite;
        if (gene.biostatArc == 0)
            switch (geneType)
            {
                case GeneType.Endogene:
                    cachedTexture = GeneBackground_Endogene;
                    break;
                case GeneType.Xenogene:
                    cachedTexture = GeneBackground_Xenogene;
                    break;
            }

        GUI.DrawTexture(rect2, (Texture)cachedTexture.Texture);
        Widgets.DefIcon(rect2, (Def)gene, scale: 0.9f, color: new Color?(iconColor));
        Text.Font = GameFont.Tiny;
        float height = Text.CalcHeight((string)gene.LabelCap, rect1.width);
        Rect rect3 = new Rect(0.0f, rect1.yMax - height, rect1.width, height);
        GUI.DrawTexture(new Rect(rect3.x, rect3.yMax - height, rect3.width, height), (Texture)TexUI.GrayTextBG);
        Text.Anchor = TextAnchor.LowerCenter;
        if (overridden)
            GUI.color = ColoredText.SubtleGrayColor;
        if (doBackground && height < (Text.LineHeight - 2.0) * 2.0)
            rect3.y -= 3f;
        Widgets.Label(rect3, gene.LabelCap);
        GUI.color = Color.white;
        Text.Anchor = TextAnchor.UpperLeft;
        Text.Font = GameFont.Small;

        if (Widgets.ButtonInvisible(rect1))
            SelectedGene = gene;
        if (string.Equals(gene.defName, SelectedGene?.defName))
            Widgets.DrawHighlight(rect1);
        GUI.EndGroup();
    }
}