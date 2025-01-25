/*
 * User: Anonemous2
 * Date: 13-06-2024
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using GenepackReprocessor.Properties;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace GenepackReprocessor;
public class Dialog_MergeGenepack : GeneCreationDialogBase
{
    // Settings for mod (I could just pull this out of the geneSeparator, but I'm lazy)
    private static GenepackReprocessorSettings? _settings;
    public static GenepackReprocessorSettings Settings => _settings ??= LoadedModManager.GetMod<GenepackImprovMod>().GetSettings<GenepackReprocessorSettings>();

    private Building_GeneSeparator geneSeparator;

    private List<Genepack> libraryGenepacks = new List<Genepack>();


    private List<Genepack> unpoweredGenepacks = new List<Genepack>();

    private List<Genepack> selectedGenepacks = new List<Genepack>();

    private HashSet<Genepack> matchingGenepacks = new HashSet<Genepack>();

    private readonly Color UnpoweredColor = new Color(0.5f, 0.5f, 0.5f, 0.5f);

    private List<GeneDef> tmpGenes = new List<GeneDef>();

    public override Vector2 InitialSize => new Vector2(1016f, UI.screenHeight);

    protected override string Header => "GeneR_MergeGenes".Translate();

    protected override string AcceptButtonLabel => "GeneR_AcceptGenes".Translate();

    // Added
    private Genepack selectedGenepack;
    protected Genepack SelectedGenepack => selectedGenepack;

    // Here just for interface
    protected override List<GeneDef> SelectedGenes
    {
        get
        {
            tmpGenes.Clear();
            foreach (Genepack selectedGenepack in selectedGenepacks)
            {
                foreach (GeneDef item in selectedGenepack.GeneSet.GenesListForReading)
                {
                    tmpGenes.Add(item);
                }
            }
            return tmpGenes;
        }
    }

    public Dialog_MergeGenepack(Building_GeneSeparator geneSeparator)
    {
        this.geneSeparator = geneSeparator;
        libraryGenepacks.AddRange(geneSeparator.GetGenepacks(includePowered: true, includeUnpowered: true));
        unpoweredGenepacks.AddRange(geneSeparator.GetGenepacks(includePowered: false, includeUnpowered: true));
        closeOnAccept = false;
        forcePause = true;
        absorbInputAroundWindow = true;
        searchWidgetOffsetX = GeneCreationDialogBase.ButSize.x * 2f + 4f;
        libraryGenepacks.SortGenepacks();
        unpoweredGenepacks.SortGenepacks();
    }

    public override void PostOpen()
    {
        if (!ModLister.CheckBiotech("gene viewing"))
        {
            Close(doCloseSound: false);
        }
        else
        {
            base.PostOpen();
        }
    }

    // Remove the xenotype name requirement.
    protected override bool CanAccept()
    {
        List<GeneDef> selectedGenes = SelectedGenes;
        int mergeLimit = Building_GeneSeparator.Settings.genepackMergeMax;
        if (selectedGenes.Count > mergeLimit) { // TODO: make this value dependant on settings
            Messages.Message("GeneR_MessageTooManyGenes".Translate(mergeLimit + 1).CapitalizeFirst(), null, MessageTypeDefOf.RejectInput, historical: false);
            return false;
        }
        // foreach (GeneDef selectedGene in SelectedGenes)
        // {
        //    if (selectedGene.prerequisite != null && !selectedGenes.Contains(selectedGene.prerequisite))
        //    {
        //        Messages.Message("MessageGeneMissingPrerequisite".Translate(selectedGene.label).CapitalizeFirst() + ": " + selectedGene.prerequisite.LabelCap, null, MessageTypeDefOf.RejectInput, historical: false);
        //        return false;
        //    }
        // }
        if (selectedGenepacks.Count < 2)
        {
            Messages.Message("GeneR_MessageSelectMoreGenepacks".Translate(), null, MessageTypeDefOf.RejectInput, historical: false);
            return false;
        }
        return true;
    }

    protected override void Accept()
    {
        if (geneSeparator.Working)
        {
            Find.WindowStack.Add(Dialog_MessageBox.CreateConfirmation("GeneR_ConfirmGenepack".Translate(), StartMerge, destructive: true));
        }
        else
        {
            StartMerge();
        }
    }

    private void StartMerge()
    {
        geneSeparator.StartMerge(selectedGenepacks, arc);
        SoundDefOf.StartRecombining.PlayOneShotOnCamera();
        Close(doCloseSound: false);
    }

    // Here for overiding
    public override void DoWindowContents(Rect rect)
    {
        Rect rect2 = rect;
        rect2.yMax -= ButSize.y + 4f;
        Rect rect3 = new Rect(rect2.x, rect2.y, rect2.width, 35f);
        Text.Font = GameFont.Medium;
        Widgets.Label(rect3, Header);
        Text.Font = GameFont.Small;
        DrawSearchRect(rect);

        rect2.yMin += 39f;
        float num = rect.width * 0.25f - Margin - 10f;
        float num2 = num - 24f - 10f;
        float num3 = BiostatsTable.HeightForBiostats(alwaysUseFullBiostatsTableHeight ? 1 : arc);

        Rect rect4 = new Rect(rect2.x + Margin, rect2.y, rect2.width - Margin * 2f, rect2.height - num3 - 8f);
        DrawGenes(rect4);
        float num4 = rect4.yMax + 4f;
        Rect rect5 = new Rect(rect2.x + Margin + 10f, num4, rect.width * 0.75f - Margin * 3f - 10f, num3);
        rect5.yMax = rect4.yMax + num3 + 4f;
        BiostatsTable.Draw(rect5, gcx, met, arc, drawMax: true, ignoreRestrictions, maxGCX);

        Rect rect12 = rect;
        rect12.yMin = rect12.yMax - ButSize.y;
        DoBottomButtons(rect12);
    }

    protected override void DrawGenes(Rect rect)
    {
        GUI.BeginGroup(rect);
        Rect rect2 = new Rect(0f, 0f, rect.width - 16f, scrollHeight);
        float curY = 0f;
        Widgets.BeginScrollView(rect.AtZero(), ref scrollPosition, rect2);
        Rect containingRect = rect2;
        containingRect.y = scrollPosition.y;
        containingRect.height = rect.height;
        DrawSection(rect, selectedGenepacks, "SelectedGenepacks".Translate(), ref curY, ref selectedHeight, adding: false, containingRect);
        curY += 8f;
        DrawSection(rect, libraryGenepacks, "GenepackLibrary".Translate(), ref curY, ref unselectedHeight, adding: true, containingRect);
        if (Event.current.type == EventType.Layout)
        {
            scrollHeight = curY;
        }
        Widgets.EndScrollView();
        GUI.EndGroup();
    }

    private void DrawSection(Rect rect, List<Genepack> genepacks, string label, ref float curY, ref float sectionHeight, bool adding, Rect containingRect)
    {
        float curX = 4f;
        Rect rect2 = new Rect(10f, curY, rect.width - 16f - 10f, Text.LineHeight);
        Widgets.Label(rect2, label);
        if (!adding)
        {
            Text.Anchor = TextAnchor.UpperRight;
            GUI.color = ColoredText.SubtleGrayColor;
            Widgets.Label(rect2, "ClickToAddOrRemove".Translate());
            GUI.color = Color.white;
            Text.Anchor = TextAnchor.UpperLeft;
        }
        curY += Text.LineHeight + 3f;
        float num = curY;
        Rect rect3 = new Rect(0f, curY, rect.width, sectionHeight);
        Widgets.DrawRectFast(rect3, Widgets.MenuSectionBGFillColor);
        curY += 4f;
        if (!genepacks.Any())
        {
            Text.Anchor = TextAnchor.MiddleCenter;
            GUI.color = ColoredText.SubtleGrayColor;
            Widgets.Label(rect3, "(" + "NoneLower".Translate() + ")");
            GUI.color = Color.white;
            Text.Anchor = TextAnchor.UpperLeft;
        }
        else
        {
            for (int i = 0; i < genepacks.Count; i++)
            {
                Genepack genepack = genepacks[i];
                if (quickSearchWidget.filter.Active && (!matchingGenepacks.Contains(genepack) || (adding && selectedGenepacks.Contains(genepack))))
                {
                    continue;
                }
                float num2 = 34f + GeneCreationDialogBase.GeneSize.x * (float)genepack.GeneSet.GenesListForReading.Count + 4f * (float)(genepack.GeneSet.GenesListForReading.Count + 2);
                if (curX + num2 > rect.width - 16f)
                {
                    curX = 4f;
                    curY += GeneCreationDialogBase.GeneSize.y + 8f + 14f;
                }
                if (adding && selectedGenepacks.Contains(genepack))
                {
                    Widgets.DrawLightHighlight(new Rect(curX, curY, num2, GeneCreationDialogBase.GeneSize.y + 8f));
                    curX += num2 + 14f;
                }
                else if (DrawGenepack(genepack, ref curX, curY, num2, containingRect))
                {
                    if (adding)
                    {
                        SoundDefOf.Tick_High.PlayOneShotOnCamera();
                        selectedGenepacks.Add(genepack);
                    }
                    else
                    {
                        SoundDefOf.Tick_Low.PlayOneShotOnCamera();
                        selectedGenepacks.Remove(genepack);
                    }
                    OnGenesChanged();
                    break;
                }
            }
        }
        curY += GeneCreationDialogBase.GeneSize.y + 12f;
        if (Event.current.type == EventType.Layout)
        {
            sectionHeight = curY - num;
        }
    }

    private bool DrawGenepack(Genepack genepack, ref float curX, float curY, float packWidth, Rect containingRect)
    {
        bool result = false;
        if (genepack.GeneSet == null || genepack.GeneSet.GenesListForReading.NullOrEmpty())
        {
            return result;
        }
        Rect rect = new Rect(curX, curY, packWidth, GeneCreationDialogBase.GeneSize.y + 8f);
        if (!containingRect.Overlaps(rect))
        {
            curX = rect.xMax + 14f;
            return false;
        }
        Widgets.DrawHighlight(rect);
        GUI.color = GeneCreationDialogBase.OutlineColorUnselected;
        Widgets.DrawBox(rect);
        GUI.color = Color.white;
        curX += 4f;
        GeneUIUtility.DrawBiostats(genepack.GeneSet.ComplexityTotal, genepack.GeneSet.MetabolismTotal, genepack.GeneSet.ArchitesTotal, ref curX, curY, 4f);
        List<GeneDef> genesListForReading = genepack.GeneSet.GenesListForReading;
        for (int i = 0; i < genesListForReading.Count; i++)
        {
            GeneDef gene = genesListForReading[i];
            if (quickSearchWidget.filter.Active && matchingGenes.Contains(gene))
            {
                matchingGenepacks.Contains(genepack);
            }
            else
                _ = 0;
            bool overridden = leftChosenGroups.Any((GeneLeftChosenGroup x) => x.overriddenGenes.Contains(gene));
            Rect geneRect = new Rect(curX, curY + 4f, GeneCreationDialogBase.GeneSize.x, GeneCreationDialogBase.GeneSize.y);
            string extraTooltip = null;
            if (leftChosenGroups.Any((GeneLeftChosenGroup x) => x.leftChosen == gene))
            {
                extraTooltip = GroupInfo(leftChosenGroups.FirstOrDefault((GeneLeftChosenGroup x) => x.leftChosen == gene));
            }
            else if (cachedOverriddenGenes.Contains(gene))
            {
                extraTooltip = GroupInfo(leftChosenGroups.FirstOrDefault((GeneLeftChosenGroup x) => x.overriddenGenes.Contains(gene)));
            }
            else if (randomChosenGroups.ContainsKey(gene))
            {
                extraTooltip = ("GeneWillBeRandomChosen".Translate() + ":\n" + randomChosenGroups[gene].Select((GeneDef x) => x.label).ToLineList("  - ", capitalizeItems: true)).Colorize(ColoredText.TipSectionTitleColor);
            }
            GeneUIUtility.DrawGeneDef(genesListForReading[i], geneRect, GeneType.Xenogene, () => extraTooltip, doBackground: false, clickable: false, overridden);
            curX += GeneCreationDialogBase.GeneSize.x + 4f;
        }
        Widgets.InfoCardButton(rect.xMax - 24f, rect.y + 2f, genepack);
        if (unpoweredGenepacks.Contains(genepack))
        {
            Widgets.DrawBoxSolid(rect, UnpoweredColor);
            TooltipHandler.TipRegion(rect, "GenepackUnusableGenebankUnpowered".Translate().Colorize(ColorLibrary.RedReadable));
        }
        if (Mouse.IsOver(rect))
        {
            Widgets.DrawHighlight(rect);
        }
        if (Event.current.type == EventType.MouseDown && Mouse.IsOver(rect) && Event.current.button == 1)
        {
            List<FloatMenuOption> list = new List<FloatMenuOption>();
            list.Add(new FloatMenuOption("EjectGenepackFromGeneBank".Translate(), delegate
            {
                CompGenepackContainer geneBankHoldingPack = geneSeparator.GetGeneBankHoldingPack(genepack);
                if (geneBankHoldingPack != null)
                {
                    ThingWithComps parent = geneBankHoldingPack.parent;
                    if (geneBankHoldingPack.innerContainer.TryDrop(genepack, parent.def.hasInteractionCell ? parent.InteractionCell : parent.Position, parent.Map, ThingPlaceMode.Near, 1, out var _))
                    {
                        if (selectedGenepacks.Contains(genepack))
                        {
                            selectedGenepacks.Remove(genepack);
                        }
                        tmpGenes.Clear();
                        libraryGenepacks.Clear();
                        unpoweredGenepacks.Clear();
                        matchingGenepacks.Clear();
                        libraryGenepacks.AddRange(geneSeparator.GetGenepacks(includePowered: true, includeUnpowered: true));
                        unpoweredGenepacks.AddRange(geneSeparator.GetGenepacks(includePowered: false, includeUnpowered: true));
                        libraryGenepacks.SortGenepacks();
                        unpoweredGenepacks.SortGenepacks();
                        OnGenesChanged();
                    }
                }
            }));
            Find.WindowStack.Add(new FloatMenu(list));
        }
        else if (Widgets.ButtonInvisible(rect))
        {
            result = true;
        }
        curX = Mathf.Max(curX, rect.xMax + 14f);
        return result;
        static string GroupInfo(GeneLeftChosenGroup group)
        {
            if (group == null)
            {
                return null;
            }
            return ("GeneOneActive".Translate() + ":\n  - " + group.leftChosen.LabelCap + " (" + "Active".Translate() + ")" + "\n" + group.overriddenGenes.Select((GeneDef x) => (x.label + " (" + "Suppressed".Translate() + ")").Colorize(ColorLibrary.RedReadable)).ToLineList("  - ", capitalizeItems: true)).Colorize(ColoredText.TipSectionTitleColor);
        }
    }

    protected override void DrawSearchRect(Rect rect)
    {
        base.DrawSearchRect(rect);
    }

    protected override void DoBottomButtons(Rect rect)
    {
        base.DoBottomButtons(rect);
        if (selectedGenepacks.Any())
        {
            // Estimate
            float totalWorkRequired = 0;
            switch (Settings.merge)
            {
                case GenepackReprocessorSettings.CurveType.Linear:
                    totalWorkRequired = GenepackLinCurves.ComplexityToCreationHoursCurve.Evaluate(gcx);
                    break;
                case GenepackReprocessorSettings.CurveType.Exponetial:
                    totalWorkRequired = GenepackExpCurves.ComplexityToCreationHoursCurve.Evaluate(gcx);
                    break;
                default:
                    totalWorkRequired = GenepackLogCurve.ComplexityToCreationHoursCurve.Evaluate(gcx);
                    break;
            }
            totalWorkRequired *= 4000f * Settings.workToMerge;
            totalWorkRequired *= (1 + arc); // Penalty for archites in the genepack
            int numTicks = Mathf.RoundToInt((float)Mathf.RoundToInt(totalWorkRequired / geneSeparator.GetStatValue(StatDefOf.AssemblySpeedFactor)));

            Rect rect2 = new Rect(rect.center.x, rect.y, rect.width / 2f - GeneCreationDialogBase.ButSize.x - 10f, GeneCreationDialogBase.ButSize.y);
            TaggedString label;
            TaggedString taggedString; 
            if (arc > 0 && !ResearchProjectDefOf.Archogenetics.IsFinished)
            {
                label = ("MissingRequiredResearch".Translate() + ": " + ResearchProjectDefOf.Archogenetics.LabelCap).Colorize(ColorLibrary.RedReadable);
                taggedString = "MustResearchProject".Translate(ResearchProjectDefOf.Archogenetics);
            }
            {
                label = "GeneR_GenepackDuration".Translate() + ": " + numTicks.ToStringTicksToPeriod();
                taggedString = "GeneR_GenepackDurationDesc".Translate();
            }
            Text.Anchor = TextAnchor.MiddleLeft;
            Widgets.Label(rect2, label);
            Text.Anchor = TextAnchor.UpperLeft;
            if (Mouse.IsOver(rect2))
            {
                Widgets.DrawHighlight(rect2);
                TooltipHandler.TipRegion(rect2, taggedString);
            }
        }
    }

    protected override void UpdateSearchResults()
    {
        quickSearchWidget.noResultsMatched = false;
        matchingGenepacks.Clear();
        matchingGenes.Clear();
        if (!quickSearchWidget.filter.Active)
        {
            return;
        }
        foreach (Genepack selectedGenepack in selectedGenepacks)
        {
            List<GeneDef> genesListForReading = selectedGenepack.GeneSet.GenesListForReading;
            for (int i = 0; i < genesListForReading.Count; i++)
            {
                if (quickSearchWidget.filter.Matches(genesListForReading[i].label))
                {
                    matchingGenepacks.Add(selectedGenepack);
                    matchingGenes.Add(genesListForReading[i]);
                }
            }
        }
        foreach (Genepack libraryGenepack in libraryGenepacks)
        {
            if (selectedGenepacks.Contains(libraryGenepack))
            {
                continue;
            }
            List<GeneDef> genesListForReading2 = libraryGenepack.GeneSet.GenesListForReading;
            for (int j = 0; j < genesListForReading2.Count; j++)
            {
                if (quickSearchWidget.filter.Matches(genesListForReading2[j].label))
                {
                    matchingGenepacks.Add(libraryGenepack);
                    matchingGenes.Add(genesListForReading2[j]);
                }
            }
        }
        quickSearchWidget.noResultsMatched = !matchingGenepacks.Any();
    }
}
