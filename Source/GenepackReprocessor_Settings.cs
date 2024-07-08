/*
 * User: Anonemous2
 * Date: 13-06-2024
 */
using GenepackReprocessor.Properties;
using RimWorld;
using System;
using UnityEngine;
using Verse;
using Verse.Noise;
using HarmonyLib;
using Multiplayer.API;

using static GenepackReprocessor.GenepackReprocessorSettings;

namespace GenepackReprocessor;

public class GenepackImprovMod : Mod
{
    GenepackReprocessorSettings settings;

    // Var for controlling the display of settings, so it's a little nicer to navagate.
    private enum CurWindow : int
    {
        Main        = -1,
        Building    = 0,
        Work        = 1,
    }
    private CurWindow curWindow = CurWindow.Main;
    private CurWindow preWindow = CurWindow.Main;

    // An intermediate int value we'll round.
    int roundedFactor;

    // Magic numbers, for editing the layout from one spot.
    private float labelPart = 0.7f;
    private float fieldOffs = 0.1f;
    private float floatOffset = 30f;

    // Buffers for text fields!
    string bufSteel = string.Empty;
    string bufPlast = string.Empty;
    string bufGold  = string.Empty;
    string bufCompo = string.Empty;
    string bufAdvCo = string.Empty;

    string bufHP           = string.Empty;
    string bufBuildWork    = string.Empty;
    string bufMass         = string.Empty;
    string bufFlammability = string.Empty;
    string bufSkillNeeded  = string.Empty;

    string bufPowerIdle = string.Empty;
    string bufPowerUsin = string.Empty;

    string bufSeparateBaseNeutroamine       = string.Empty;
    string bufSeparateComplexityNeutroamine = string.Empty;

    string bufDuplicateBaseNeutroamine       = string.Empty;
    string bufDuplicateComplexityNeutroamine = string.Empty;

    string bufMergeBaseNeutroamine = string.Empty;
    string bufMergeComplexityNeutroamine = string.Empty;

    string bufArchitePen = string.Empty;

    /// <summary>
    /// A mandatory constructor which resolves the reference to our settings.
    /// </summary>
    /// <param name="content"></param>
    public GenepackImprovMod(ModContentPack content) : base(content)
    {
        this.settings = GetSettings<GenepackReprocessorSettings>();

        // Initalize the buffers
        bufSteel = settings.costSteel.ToString();
        bufPlast = settings.costPlast.ToString();
        bufGold  = settings.costGold.ToString();
        bufCompo = settings.costCompo.ToString();
        bufAdvCo = settings.costAdvCo.ToString();

        bufHP           = settings.hp.ToString();
        bufBuildWork    = settings.buildWork.ToString();
        bufMass         = settings.mass.ToString();
        bufFlammability = settings.flammability.ToString();
        bufSkillNeeded  = settings.skillNeeded.ToString();

        bufPowerIdle = settings.powerIdle.ToString();
        bufPowerUsin = settings.powerUsin.ToString();

        bufSeparateBaseNeutroamine       = settings.separateBaseNeutroamine.ToString();
        bufSeparateComplexityNeutroamine = settings.separateComplexityNeutroamine.ToString();

        bufDuplicateBaseNeutroamine       = settings.duplicateBaseNeutroamine.ToString();
        bufDuplicateComplexityNeutroamine = settings.duplicateComplexityNeutroamine.ToString();

        bufMergeBaseNeutroamine = settings.mergeBaseNeutroamine.ToString();
        bufMergeComplexityNeutroamine = settings.mergeComplexityNeutroamine.ToString();

        bufArchitePen = settings.architePen.ToString();
    }

    /// <summary>
    /// Default and main menu when entering our mod's settings. Should allow you to
    /// navigate to the other menus. TODO: Might not need to use it.
    /// </summary>
    /// <param name="listingStandard">Context window to add stuff into.</param>
    public void WindowContentsMain(ref Listing_Custom listingStandard)
    {

    }

    public void ContentsBuildingCost(Rect inRect, ref Listing_Custom listingStandard) {
        // Create a subsection for the costs.
        Rect line = new Rect(inRect.xMin, inRect.yMin, inRect.xMax - inRect.xMin,Text.LineHeight);
        Listing_Custom sub = listingStandard.BeginSection((line.yMax - line.yMin) * 2.133f);

        line = sub.GetRectLine();
        sub.ColLabel(line, 3, 0,
            "GeneR_Materials".Translate(), "GeneR_MaterialsHelp".Translate());
        sub.NGTextFieldNumericLabeled<int>(line, 3, 1,
            "GeneR_MatComp".Translate(), ref settings.costCompo, ref bufCompo, 0f, 75f, labelPart, fieldOffs, "GeneR_MatCompHelp".Translate());
        sub.NGTextFieldNumericLabeled<int>(line, 3, 2,
            "GeneR_MatAdvComp".Translate(), ref settings.costAdvCo, ref bufAdvCo, 0f, 75f, labelPart, fieldOffs, "GeneR_MatAdvCompHelp".Translate());
        sub.Gap(); line = sub.GetRectLine();
        sub.NGTextFieldNumericLabeled<int>(line, 3, 0,
            "GeneR_MatSteel".Translate(), ref settings.costSteel, ref bufSteel, 0f, 500f, labelPart, fieldOffs, "GeneR_MatSteelHelp".Translate());
        sub.NGTextFieldNumericLabeled<int>(line, 3, 1,
            "GeneR_MatPlasteel".Translate(), ref settings.costPlast, ref bufPlast, 0f, 500f, labelPart, fieldOffs, "GeneR_MatPlasteelHelp".Translate());
        sub.NGTextFieldNumericLabeled<int>(line, 3, 2,
            "GeneR_MatGold".Translate(), ref settings.costGold,  ref bufGold, 0f, 500f, labelPart, fieldOffs, "GeneR_MatGoldHelp".Translate());
        listingStandard.EndSection(sub);
    }

    public void ContentsBuildingSettings(Rect inRect, ref Listing_Custom listingStandard) {
        // Create a subsection for the stats.
        Rect line = new Rect(inRect.xMin, inRect.yMin, inRect.xMax - inRect.xMin, Text.LineHeight);
        Listing_Custom sub = listingStandard.BeginSection((line.yMax - line.yMin) * 2.133f);

        line = sub.GetRectLine();
        sub.NGTextFieldNumericLabeled<int>(line, 3, 0,
            "GeneR_HP".Translate(), ref settings.hp, ref bufHP, 1f, 10000, labelPart, fieldOffs, "GeneR_HPHelp".Translate());
        sub.NGTextFieldNumericLabeled<int>(line, 3, 1,
            "GeneR_BuildWork".Translate(), ref settings.buildWork, ref bufBuildWork, 0f, 100000f, labelPart, fieldOffs, "GeneR_BuildWorkHelp".Translate());
        sub.NGTextFieldNumericLabeled<int>(line, 3, 2,
            "GeneR_Mass".Translate(), ref settings.mass,  ref bufMass, 1f, 100f, labelPart, fieldOffs, "GeneR_MassHelp".Translate());
        sub.Gap(); line = sub.GetRectLine();
        sub.NGTextFieldNumericLabeled<float>(line, 3, 0,
            "GeneR_Flam".Translate(), ref settings.flammability, ref bufFlammability, 0f, 1f, labelPart, fieldOffs, "GeneR_FlamHelp".Translate());
        sub.NGTextFieldNumericLabeled<int>(line, 3, 1,
            "GeneR_BuildSkill".Translate(), ref settings.skillNeeded, ref bufSkillNeeded, 0f, 20f, labelPart, fieldOffs, "GeneR_BuildSkillHelp".Translate());
        sub.NGCheckboxLabeled(line, 3, 2, "GeneR_Minify".Translate(), ref settings.movable, fieldOffs, "GeneR_MinifyHelp".Translate()); // TODO: Translate.

        listingStandard.EndSection(sub);
    }

    // TODO: Implement.
    public void ContentsBuildingPower(Rect inRect, ref Listing_Custom listingStandard)
    {
        // Create a subsection for the power drain.
        Rect line = new Rect(inRect.xMin, inRect.yMin, inRect.xMax - inRect.xMin, Text.LineHeight);
        Listing_Custom sub = listingStandard.BeginSection((line.yMax - line.yMin) * 1.1f);

        line = sub.GetRectLine();
        sub.ColLabel(line, 3, 0,
            "Building Power Usage (W)", "How many Watts of power does this building consume while working.");
        sub.NGTextFieldNumericLabeled<int>(line, 3, 1,
            "Idle Drain:", ref settings.powerIdle, ref bufPowerIdle, 1f, 10000, labelPart, fieldOffs, "How many Watts of power does this building consume while idle.");
        sub.NGTextFieldNumericLabeled<int>(line, 3, 2,
            "Working Drain:", ref settings.powerUsin, ref bufPowerUsin, 0f, 1000f, labelPart, fieldOffs, "How many Watts of power does this building consume while working.");

        listingStandard.EndSection(sub);
    }

    public void ContentsSeparting(Rect inRect, ref Listing_Custom listingStandard)
    {
        // Create a subsection.
        Rect line = new Rect(inRect.xMin, inRect.yMin, inRect.xMax - inRect.xMin, Text.LineHeight);

        // Separate settings. If it's not enabled, there's no reason to show them.
        if (!settings.separateEnabled) { 
            Listing_Custom subt = listingStandard.BeginSection((line.yMax - line.yMin) * 1.1f);
            line = subt.GetRectLine();
            subt.NGCheckboxLabeled(line, 1, 0, "GeneR_CanSeparate".Translate(), ref settings.separateEnabled, fieldOffs / 4.5f,
                "GeneR_CanSeparateHelp".Translate());
            listingStandard.EndSection(subt); 
            return;
        }
        // Else show all the settings.
        Listing_Custom sub = listingStandard.BeginSection((line.yMax - line.yMin) * 3.7f);
        line = sub.GetRectLine();
        sub.NGCheckboxLabeled(line, 1, 0, "GeneR_CanSeparate".Translate(), ref settings.separateEnabled, fieldOffs / 4.5f,
            "GeneR_CanSeparateHelp".Translate());
        // Work multiplier.
        roundedFactor = (int)(10f * sub.SliderLabeled("GeneR_WorkMultiplier".Translate() + settings.workToSplit.ToString("0.0") + "GeneR_X".Translate(),
                settings.workToSplit, 0.1f, 5, 0.25f, "GeneR_SeparateMultiplierHelp".Translate()));
        settings.workToSplit = (float)roundedFactor * 0.1f;

        // Work curve.
        line = sub.GetRectLine();
        if (listingStandard.NGRadioButton(line, 3, 0, "GeneR_Logarithmic".Translate(),
            settings.split == GenepackReprocessorSettings.CurveType.Log, fieldOffs, "GeneR_LogarithmicHelp".Translate()))
        { settings.split = CurveType.Log; }

        if (listingStandard.NGRadioButton(line, 3, 1, "GeneR_Linear".Translate(),
            settings.split == GenepackReprocessorSettings.CurveType.Linear, fieldOffs, "GeneR_LinearHelp".Translate()))
        { settings.split = CurveType.Linear; }

        if (listingStandard.NGRadioButton(line, 3, 2, "GeneR_Exponential".Translate(),
            settings.split == GenepackReprocessorSettings.CurveType.Exponetial, fieldOffs, "GeneR_ExponentialHelp".Translate()))
        { settings.split = CurveType.Exponetial; }

        listingStandard.EndSection(sub);

        // Consumption.
        Listing_Custom sub2 = listingStandard.BeginSection((line.yMax - line.yMin) * 2.133f);
        line = sub2.GetRectLine();
        
        sub2.NGCheckboxLabeled(line, 3, 0,
            "GeneR_ArchiteCapsulesSet".Translate(), ref settings.separateNeedsArchites, fieldOffs, "GeneR_ArchiteCapsulesSetHelp".Translate());
        sub2.NGTextFieldNumericLabeled<int>(line, 3, 1,
            "GeneR_NeutroamineBase".Translate(), ref settings.separateBaseNeutroamine, ref bufSeparateBaseNeutroamine, 0f, 150f, labelPart, fieldOffs, "GeneR_NeutroamineBaseHelp".Translate());
        sub2.NGTextFieldNumericLabeled<int>(line, 3, 2,
            "GeneR_NeutroamineComp".Translate(), ref settings.separateComplexityNeutroamine, ref bufSeparateComplexityNeutroamine, 0f, 150f, labelPart, fieldOffs, "GeneR_NeutroamineCompHelp".Translate());
        sub2.Gap(); line = sub2.GetRectLine();
        sub2.NGCheckboxLabeled(line, 3, 0,
            "GeneR_GenepackConsume".Translate(), ref settings.consumeOnSplit, fieldOffs, "GeneR_GenepackConsumeHelp".Translate());
        listingStandard.EndSection(sub2);
    }

    public void ContentsDuplicate(Rect inRect, ref Listing_Custom listingStandard)
    {
        // Create a subsection.
        Rect line = new Rect(inRect.xMin, inRect.yMin, inRect.xMax - inRect.xMin, Text.LineHeight);

        // Duplicate settings. If it's not enabled, there's no reason to show them.
        if (!settings.duplicateEnabled)
        {
            Listing_Custom subt = listingStandard.BeginSection((line.yMax - line.yMin) * 1.1f);
            line = subt.GetRectLine();
            subt.NGCheckboxLabeled(line, 1, 0, "GeneR_CanDuplicate".Translate(), ref settings.duplicateEnabled, fieldOffs / 4.5f,
                "GeneR_CanDuplicateHelp".Translate());
            listingStandard.EndSection(subt);
            return;
        }
        // Else show all the settings.
        Listing_Custom sub = listingStandard.BeginSection((line.yMax - line.yMin) * 3.7f);
        line = sub.GetRectLine();
        sub.NGCheckboxLabeled(line, 1, 0, "GeneR_CanDuplicate".Translate(), ref settings.duplicateEnabled, fieldOffs / 4.5f,
            "GeneR_CanDuplicateHelp".Translate());
        // Work multiplier.
        roundedFactor = (int)(10f * sub.SliderLabeled("GeneR_WorkMultiplier".Translate() + settings.workToDupli.ToString("0.0") + "GeneR_X".Translate(),
                settings.workToDupli, 0.1f, 5, 0.25f, "GeneR_DuplicateMultiplierHelp".Translate()));
        settings.workToDupli = (float)roundedFactor * 0.1f;

        // Work curve.
        line = sub.GetRectLine();
        if (listingStandard.NGRadioButton(line, 3, 0, "GeneR_Logarithmic".Translate(),
            settings.dupli == GenepackReprocessorSettings.CurveType.Log, fieldOffs, "GeneR_LogarithmicHelp".Translate()))
        { settings.dupli = CurveType.Log; }

        if (listingStandard.NGRadioButton(line, 3, 1, "GeneR_Linear".Translate(),
            settings.dupli == GenepackReprocessorSettings.CurveType.Linear, fieldOffs, "GeneR_LinearHelp".Translate()))
        { settings.dupli = CurveType.Linear; }

        if (listingStandard.NGRadioButton(line, 3, 2, "GeneR_Exponential".Translate(),
            settings.dupli == GenepackReprocessorSettings.CurveType.Exponetial, fieldOffs, "GeneR_ExponentialHelp".Translate()))
        { settings.dupli = CurveType.Exponetial; }

        listingStandard.EndSection(sub);

        // Consumption.
        Listing_Custom sub2 = listingStandard.BeginSection((line.yMax - line.yMin) * 1.1f);
        line = sub2.GetRectLine();

        sub2.NGCheckboxLabeled(line, 3, 0,
            "GeneR_ArchiteCapsulesSet".Translate(), ref settings.duplicateNeedsArchites, fieldOffs, "GeneR_ArchiteCapsulesSetHelp".Translate());
        sub2.NGTextFieldNumericLabeled<int>(line, 3, 1,
            "GeneR_NeutroamineBase".Translate(), ref settings.duplicateBaseNeutroamine, ref bufDuplicateBaseNeutroamine, 0f, 150f, labelPart, fieldOffs, "GeneR_NeutroamineBaseHelp".Translate());
        sub2.NGTextFieldNumericLabeled<int>(line, 3, 2,
            "GeneR_NeutroamineComp".Translate(), ref settings.duplicateComplexityNeutroamine, ref bufDuplicateComplexityNeutroamine, 0f, 150f, labelPart, fieldOffs, "GeneR_NeutroamineCompHelp".Translate());
        listingStandard.EndSection(sub2);
    }

    public void ContentsMerge(Rect inRect, ref Listing_Custom listingStandard)
    {
        // Create a subsection.
        Rect line = new Rect(inRect.xMin, inRect.yMin, inRect.xMax - inRect.xMin, Text.LineHeight);

        // Merge settings. If it's not enabled, there's no reason to show them.
        if (!settings.mergeEnabled)
        {
            Listing_Custom subt = listingStandard.BeginSection((line.yMax - line.yMin) * 1.1f);
            line = subt.GetRectLine();
            subt.NGCheckboxLabeled(line, 1, 0, "GeneR_CanMerge".Translate(), ref settings.mergeEnabled, fieldOffs / 4.5f,
                "GeneR_CanMergeHelp".Translate());
            listingStandard.EndSection(subt);
            return;
        }
        // Else show all the settings.
        Listing_Custom sub = listingStandard.BeginSection((line.yMax - line.yMin) * 3.7f);
        line = sub.GetRectLine();
        sub.NGCheckboxLabeled(line, 1, 0, "GeneR_CanMerge".Translate(), ref settings.mergeEnabled, fieldOffs / 4.5f,
            "GeneR_CanMergeHelp".Translate());
        // Work multiplier.
        roundedFactor = (int)(10f * sub.SliderLabeled("GeneR_WorkMultiplier".Translate() + settings.workToMerge.ToString("0.0") + "GeneR_X".Translate(),
                settings.workToMerge, 0.1f, 5, 0.25f, "GeneR_MergeMultiplierHelp".Translate()));
        settings.workToMerge = (float)roundedFactor * 0.1f;

        // Work curve.
        line = sub.GetRectLine();
        if (listingStandard.NGRadioButton(line, 3, 0, "GeneR_Logarithmic".Translate(),
            settings.merge == GenepackReprocessorSettings.CurveType.Log, fieldOffs, "GeneR_LogarithmicHelp".Translate()))
        { settings.merge = CurveType.Log; }

        if (listingStandard.NGRadioButton(line, 3, 1, "GeneR_Linear".Translate(),
            settings.merge == GenepackReprocessorSettings.CurveType.Linear, fieldOffs, "GeneR_LinearHelp".Translate()))
        { settings.merge = CurveType.Linear; }

        if (listingStandard.NGRadioButton(line, 3, 2, "GeneR_Exponential".Translate(),
            settings.merge == GenepackReprocessorSettings.CurveType.Exponetial, fieldOffs, "GeneR_ExponentialHelp".Translate()))
        { settings.merge = CurveType.Exponetial; }

        listingStandard.EndSection(sub);

        // Consumption.
        Listing_Custom sub2 = listingStandard.BeginSection((line.yMax - line.yMin) * 2.133f);
        line = sub2.GetRectLine();

        sub2.NGCheckboxLabeled(line, 3, 0,
            "GeneR_ArchiteCapsulesSet".Translate(), ref settings.mergeNeedsArchites, fieldOffs, "GeneR_ArchiteCapsulesSetHelp".Translate());
        sub2.NGTextFieldNumericLabeled<int>(line, 3, 1,
            "GeneR_NeutroamineBase".Translate(), ref settings.mergeBaseNeutroamine, ref bufMergeBaseNeutroamine, 0f, 150f, labelPart, fieldOffs, "GeneR_NeutroamineBaseHelp".Translate());
        sub2.NGTextFieldNumericLabeled<int>(line, 3, 2,
            "GeneR_NeutroamineComp".Translate(), ref settings.mergeComplexityNeutroamine, ref bufMergeComplexityNeutroamine, 0f, 150f, labelPart, fieldOffs, "GeneR_NeutroamineCompHelp".Translate());
        sub2.Gap(); line = sub2.GetRectLine();
        sub2.NGCheckboxLabeled(line, 3, 0,
            "GeneR_GenepackConsume".Translate(), ref settings.consumeOnMerge, fieldOffs, "GeneR_GenepackConsumeHelp".Translate());
        listingStandard.EndSection(sub2);
    }

    // TODO: Implement. Also add translations after.
    public void ContentsArchiteSetting(Rect inRect, ref Listing_Custom listingStandard)
    {
        // Create a subsection for the costs
        Rect line = new Rect(inRect.xMin, inRect.yMin, (inRect.xMax - inRect.xMin) / 2f, Text.LineHeight);

        // Else show all the settings.
        Listing_Custom sub = listingStandard.CBeginSection(line, (line.yMax - line.yMin) * 1.1f);

        sub.NGTextFieldNumericLabeled<float>(line, 1, 0,
            "Archite Penalty Multiplier:", ref settings.architePen, ref bufArchitePen, 0f, 5f, labelPart, fieldOffs, "Multiples the penalty for Archite genes.");
        roundedFactor = (int)(100f * settings.architePen);
        settings.architePen = (float)roundedFactor * 0.01f;

        listingStandard.EndSection(sub);
    }

    /// <summary>
    /// The (optional) GUI part to set your settings.
    /// </summary>
    /// <param name="inRect">A Unity Rect with the size of the settings window.</param>
    public override void DoSettingsWindowContents(Rect inRect)
    {
        // Create the generic listing, which we'll fill with our settings.
        Listing_Custom listingStandard = new Listing_Custom();
        listingStandard.Begin(inRect);

        // TODO: Add Reset and Hard buttons to the top of the window

        ContentsBuildingCost(inRect, ref listingStandard);
        ContentsBuildingSettings(inRect, ref listingStandard);
        // ContentsBuildingPower(inRect, ref listingStandard); TEMP: Not used.

        // Work modes.
        //listingStandard.Label("Work Modes");
        listingStandard.Gap(); listingStandard.Gap(); listingStandard.Gap(); 
        ContentsSeparting(inRect, ref listingStandard);

        listingStandard.Gap(); listingStandard.Gap(); listingStandard.Gap(); 
        ContentsDuplicate(inRect, ref listingStandard);

        listingStandard.Gap(); listingStandard.Gap(); listingStandard.Gap(); 
        ContentsMerge(inRect, ref listingStandard);

        // Draw some buttons below the Rect.
        Rect bottom = new Rect(inRect.xMin - 10f, inRect.yMax - 80f, inRect.xMax - inRect.xMin, 40f);

        listingStandard.Gap(); listingStandard.Gap(); listingStandard.Gap();
        // ContentsArchiteSetting(inRect, ref listingStandard);

        if (listingStandard.CButtonText(bottom, 6, 4, "GeneR_SetDefault".Translate(), null, "GeneR_SetDefaultHelp".Translate()))
        {
            ResetToDefaults();
            Messages.Message("GeneR_SetDefaultMes".Translate(), null, MessageTypeDefOf.TaskCompletion, historical: false);
        }
        if (listingStandard.CButtonText(bottom, 6, 5, "GeneR_SetSimple".Translate(), null, "GeneR_SetSimpleHelp".Translate()))
        {
            ResetToSimple();
            Messages.Message("GeneR_SetSimpleMes".Translate(), null, MessageTypeDefOf.TaskCompletion, historical: false);
        }

        base.DoSettingsWindowContents(inRect);
        listingStandard.End();
    }

    // Clear buffers.
    public void ClearBuffers() {
        // Reset Buffers.
        bufSteel = settings.costSteel.ToString();
        bufPlast = settings.costPlast.ToString();
        bufGold = settings.costGold.ToString();
        bufCompo = settings.costCompo.ToString();
        bufAdvCo = settings.costAdvCo.ToString();

        bufHP = settings.hp.ToString();
        bufBuildWork = settings.buildWork.ToString();
        bufMass = settings.mass.ToString();
        bufFlammability = settings.flammability.ToString();
        bufSkillNeeded = settings.skillNeeded.ToString();

        bufPowerIdle = settings.powerIdle.ToString();
        bufPowerUsin = settings.powerUsin.ToString();

        bufSeparateBaseNeutroamine = settings.separateBaseNeutroamine.ToString();
        bufSeparateComplexityNeutroamine = settings.separateComplexityNeutroamine.ToString();

        bufDuplicateBaseNeutroamine = settings.duplicateBaseNeutroamine.ToString();
        bufDuplicateComplexityNeutroamine = settings.duplicateComplexityNeutroamine.ToString();

        bufMergeBaseNeutroamine = settings.mergeBaseNeutroamine.ToString();
        bufMergeComplexityNeutroamine = settings.mergeComplexityNeutroamine.ToString();

        bufArchitePen = settings.architePen.ToString();
    }

    // Defaults reset.
    public void ResetToDefaults() {
        settings.hp = 600;
        settings.buildWork = 24000;
        settings.movable = false;
        settings.mass = 30;
        settings.flammability = 0.5f;
        settings.skillNeeded = 6;
        // Repocessor materials cost.
        settings.costSteel = 200;
        settings.costPlast = 50;
        settings.costGold = 0;
        settings.costCompo = 6;
        settings.costAdvCo = 1;
        // Power drain.
        settings.powerIdle = 25;
        settings.powerUsin = 200;

        // Separate settings.
        settings.split = CurveType.Exponetial; // Work needed for x complexity.
        settings.separateEnabled = true; // Is this work mode usable ingame?
        settings.consumeOnSplit = true; // Destroy original genepacks?
        settings.workToSplit = 1.0f; // Work needed multiplier for each task type.
                                     // Separate materials cost.
        settings.separateBaseNeutroamine = 4;
        settings.separateComplexityNeutroamine = 2;
        settings.separateNeedsArchites = false;

        // Duplicate settings
        settings.dupli = CurveType.Linear;
        settings.duplicateEnabled = true;
        settings.workToDupli = 1.0f;
        // Duplicate materials cost.
        settings.duplicateBaseNeutroamine = 8;
        settings.duplicateComplexityNeutroamine = 4;
        settings.duplicateNeedsArchites = true;

        // Merge settings
        settings.merge = CurveType.Log;
        settings.mergeEnabled = true;
        settings.consumeOnMerge = false;
        settings.workToMerge = 1.0f;
        settings.genepackMergeMax = 9;
        // Merge materials cost.
        settings.mergeBaseNeutroamine = 6;
        settings.mergeComplexityNeutroamine = 3;
        settings.mergeNeedsArchites = true;

        settings.architePen = 1f;

    // TODO:
    // Worker settings
    settings.skillImportance = 1f;   // Multiplier on the skill's benefit/harm to work speed.
        settings.skillGain = 1f;   // Multiplier on the skill gain from creating genepacks.

        // Reset Buffers
        ClearBuffers();
    }

    // Simple settings reset
    public void ResetToSimple()
    {
        // Reset everything to defaults settings, then only update simple mode settings.
        ResetToDefaults();

        // Separate settings.
        settings.separateBaseNeutroamine = 0;
        settings.separateComplexityNeutroamine = 0;

        // Duplicate settings
        settings.duplicateEnabled = false;

        // Merge settings
        settings.mergeEnabled = false;

        // Reset Buffers
        ClearBuffers();
    }

    public override void WriteSettings()
    {
        curWindow = CurWindow.Main;         // Reset the settings window to main.
        base.WriteSettings();
        Messages.Message("GeneR_SetsSaved".Translate(), null, MessageTypeDefOf.TaskCompletion, historical: false);
        Log.Warning("Genepack reprocessor settings saved.");
        GenepackReprocessor_OnDefsLoaded.ApplySettingsToDefs();
    }

    /// <summary>
    /// Override SettingsCategory to show up in the list of settings.
    /// Using .Translate() is optional, but does allow for localisation.
    /// </summary>
    /// <returns>The (translated) mod name.</returns>
    public override string SettingsCategory()
    {
        return "GeneR_GR".Translate();
    }
}

[StaticConstructorOnStartup] // this makes the static constructor get called AFTER defs are loaded.
public class GenepackReprocessor_OnDefsLoaded
{
    // Settings for mod
    private static GenepackReprocessorSettings? _settings;
    public static GenepackReprocessorSettings Settings => _settings ??= LoadedModManager.GetMod<GenepackImprovMod>().GetSettings<GenepackReprocessorSettings>();


    static GenepackReprocessor_OnDefsLoaded()
    {
        // Apply settings to defs now that defs are loaded:
        ApplySettingsToDefs();

        // Sticking MP compat here!
        try
        {
            ((Action)(() =>
            {
                if (LoadedModManager.RunningModsListForReading.Any(x => x.Name == "Multiplayer"))
                {
                    // It's loading resources, must patch later.
                    LongEventHandler.ExecuteWhenFinished(() =>
                    {
                        var type = AccessTools.TypeByName("GenepackReprocessor.Building_GeneSeparator");
                        // Dialog separate
                        MP.RegisterSyncMethod(type, "StartSplit");
                        // Dialog duplicate
                        MP.RegisterSyncMethod(type, "StartDuplicate");
                        // Dialog merge
                        MP.RegisterSyncMethod(type, "StartMerge");
                        // Command finish, debug
                        MP.RegisterSyncMethod(type, "Finish");
                        // Command cancel
                        MP.RegisterSyncMethod(type, "Reset");
                        // Command fill, debug
                        MP.RegisterSyncMethod(type, "DevFill");
                    });
                }
            }))();
        }
        catch (TypeLoadException ex)
        {
            Log.Warning("Genepack multiplayer patch exception.");
        }
    }

    public static void ApplySettingsToDefs()
    {

        // ThingDef that we might want to change, best to took them up once.
        // It might be worth taking a note of what defs got changed, then only look them up if there's
        // a performance hit.
        ThingDef reprocessor = DefDatabase<ThingDef>.GetNamed("GeneSeparator");
        if (reprocessor != null)
        {
            // Update costList.
            reprocessor.costList.Clear();
            if (Settings.costSteel > 0)
            {
                reprocessor.costList.Add(new ThingDefCountClass(DefDatabase<ThingDef>.GetNamed("Steel"), Settings.costSteel));
            }
            if (Settings.costPlast > 0)
            {
                reprocessor.costList.Add(new ThingDefCountClass(DefDatabase<ThingDef>.GetNamed("Plasteel"), Settings.costPlast));
            }
            if (Settings.costGold > 0)
            {
                reprocessor.costList.Add(new ThingDefCountClass(DefDatabase<ThingDef>.GetNamed("Gold"), Settings.costGold));
            }
            if (Settings.costCompo > 0)
            {
                reprocessor.costList.Add(new ThingDefCountClass(DefDatabase<ThingDef>.GetNamed("ComponentIndustrial"), Settings.costCompo));
            }
            if (Settings.costAdvCo > 0)
            {
                reprocessor.costList.Add(new ThingDefCountClass(DefDatabase<ThingDef>.GetNamed("ComponentSpacer"), Settings.costAdvCo));
                // Add the help message about getting advanced components
                ResearchProjectDef process = DefDatabase<ResearchProjectDef>.GetNamed("GeneProcessor");
                process.discoveredLetterTitle = "GeneR_ResearchMes".Translate();
                process.discoveredLetterText  = "GeneR_ResearchMesDes".Translate();
            }
            // Update Stats.
            reprocessor.statBases.Clear();

            StatModifier hp = new StatModifier();
            hp.stat = DefDatabase<StatDef>.GetNamed("MaxHitPoints");
            hp.value = Settings.hp;
            reprocessor.statBases.Add(hp);

            StatModifier work = new StatModifier();
            work.stat = DefDatabase<StatDef>.GetNamed("WorkToBuild");
            work.value = Settings.buildWork;
            reprocessor.statBases.Add(work);

            StatModifier mass = new StatModifier();
            mass.stat = DefDatabase<StatDef>.GetNamed("Mass");
            mass.value = Settings.mass;
            reprocessor.statBases.Add(mass);

            StatModifier flammability = new StatModifier();
            flammability.stat = DefDatabase<StatDef>.GetNamed("Flammability");
            flammability.value = Settings.flammability;
            reprocessor.statBases.Add(flammability);

            reprocessor.constructionSkillPrerequisite = Settings.skillNeeded;

            if (!Settings.movable)
            {
                reprocessor.minifiedDef = null;
                reprocessor.thingCategories.Clear();
            }
        }

        /* TEMP: Code that doesn't work yet. Don't worry about it.
        // Modify Stats for gene creation
        GeneSeparator_DefOfs.GenepackCreationSpeed.skillNeedFactors.Clear();
        GeneSeparator_DefOfs.GenepackCreationSpeed.skillNeedFactors.Add(new SkillNeed_BaseBonus() { skill = DefDatabase<SkillDef>.GetNamed("Intellectual"),
            baseValue = ,
            bonusPerLevel = 
        });*/


        /* TODO: Figure out comps
        // Genebank
        foreach (CompProperties comp in reprocessor.comps)
        {
            if (comp is CompProperties_Power)
            {
                reprocessor.comps.Remove(comp);
                flag = true;
                break;
            }
        }
        if (flag)
        {
            // CompProperties = 
            CompPowerTrader tempIn = new CompPowerTrader();
            tempIn 

            CompProperties_Power tempC = new CompProperties_Power();
            tempC.compClass = tempIn;
        }

        if (flag) {
        CompProperties_GenepackContainer tempC = new CompProperties_GenepackContainer();
        tempC.maxCapacity = Settings.genebankCapacity;
            genebank.comps.Add(tempC);
        }
        flag = false;

        // Genepack
        StatModifier stat = new StatModifier();
        stat.stat = StatDef.Named("SellPriceFactor");
        stat.value = (float)Settings.geneSellFactor / 100f;
        foreach (StatModifier sta in genepack.statBases)
        {
            if (sta.GetHashCode == stat.GetHashCode)
            {
                genepack.statBases.Remove(sta);
                break;
            }
        }
        genepack.statBases.Add(stat);
        */
    }
}