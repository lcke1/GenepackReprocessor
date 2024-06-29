using RimWorld;
using UnityEngine;
using Verse;
using Verse.Noise;

namespace GenepackReprocessor;

/// <summary>
/// The values and .xml created for settings.
/// </summary>
public class GenepackReprocessorSettings : ModSettings
{
    // Work curves, Which max out at 24 complexity.
    public enum CurveType { 
        Linear      = 0,    // Fake 'Curve', but needed to keep the max behavior.
        Log         = 1,
        Exponetial  = 2
    }

    // General settings.
    // Construction.
    public int   hp           = 600;
    public int   buildWork    = 24000;
    public bool  movable      = false;
    public int   mass         = 30;
    public float flammability = 0.5f;
    public int   skillNeeded  = 6;
    // Repocessor materials cost.
    public int costSteel = 200;
    public int costPlast = 50;
    public int costGold  = 0;
    public int costCompo = 6;
    public int costAdvCo = 1;
    // Power drain. TEMP: power settings not currently used.
    public int powerIdle = 25;
    public int powerUsin = 200;

    // Separate settings.
    public CurveType split = CurveType.Exponetial; // Work needed for x complexity.
    public bool separateEnabled     = true; // Is this work mode usable ingame?
    public bool consumeOnSplit      = true; // Destroy original genepacks?
    public float workToSplit        = 1.0f; // Work needed multiplier for each task type.
    // Separate materials cost.
    public int  separateBaseNeutroamine         = 4;
    public int  separateComplexityNeutroamine   = 2;
    public bool separateNeedsArchites           = false;

    // Duplicate settings
    public CurveType dupli = CurveType.Linear;
    public bool duplicateEnabled    = true;
    public float workToDupli        = 1.0f;
    // Duplicate materials cost.
    public int  duplicateBaseNeutroamine        = 8;
    public int  duplicateComplexityNeutroamine  = 4;
    public bool duplicateNeedsArchites          = true;

    // Merge settings
    public CurveType merge = CurveType.Log;
    public bool mergeEnabled        = true;
    public bool consumeOnMerge      = false;
    public float workToMerge        = 1.0f;
    public int genepackMergeMax     = 9;
    // Merge materials cost.
    public int  mergeBaseNeutroamine            = 6;
    public int  mergeComplexityNeutroamine      = 3;
    public bool mergeNeedsArchites              = true;

    // Archite penalty
    public float architePen = 1;

    // Worker settings TEMP: skill settings not currently used.
    public float skillImportance    = 1f;   // Multiplier on the skill's benefit/harm to work speed.
    public float skillGain          = 1f;   // Multiplier on the skill gain from creating genepacks.

    /// <summary>
    /// The part that writes our settings to a file. Note that saving is by ref. Labels cannot use spaces (xml tags)
    /// </summary>
    public override void ExposeData()
    {

        // General settings.
        // Construction.
        Scribe_Values.Look(ref hp,          "hp",           600);
        Scribe_Values.Look(ref buildWork,   "buildWork",    10000);
        Scribe_Values.Look(ref movable,     "movable",      false);
        Scribe_Values.Look(ref mass,        "mass",         30);
        Scribe_Values.Look(ref flammability,"flammability", 0.5f);
        Scribe_Values.Look(ref skillNeeded, "skillNeeded",  6);
        // Repocessor materials cost.
        Scribe_Values.Look(ref costSteel, "costSteel", 200);
        Scribe_Values.Look(ref costPlast, "costPlast", 50);
        Scribe_Values.Look(ref costGold,  "costGold",  0);
        Scribe_Values.Look(ref costCompo, "costCompo", 6);
        Scribe_Values.Look(ref costAdvCo, "costAdvCo", 1);
        // Power drain.
        Scribe_Values.Look(ref powerIdle, "powerIdle", 25);
        Scribe_Values.Look(ref powerUsin, "powerUsin", 200);

        // Separate settings.
        Scribe_Values.Look(ref split,           "split",            CurveType.Exponetial);
        Scribe_Values.Look(ref separateEnabled, "separateEnabled",  true);
        Scribe_Values.Look(ref consumeOnSplit,  "consumeOnSplit",   true);
        Scribe_Values.Look(ref workToSplit,     "workToSplit",      1.0f);
        // Separate materials cost.
        Scribe_Values.Look(ref separateBaseNeutroamine,         "separateBaseNeutroamine",          4);
        Scribe_Values.Look(ref separateComplexityNeutroamine,   "separateComplexityNeutroamine",    2);
        Scribe_Values.Look(ref separateNeedsArchites,           "separateNeedsArchites",            false);

        // Duplicate settings
        Scribe_Values.Look(ref dupli,           "dupli",            CurveType.Linear);
        Scribe_Values.Look(ref duplicateEnabled,"duplicateEnabled", true);
        Scribe_Values.Look(ref workToDupli,     "workToDupli",      1.0f);
        // Duplicate materials cost.
        Scribe_Values.Look(ref duplicateBaseNeutroamine,        "duplicateBaseNeutroamine",         8);
        Scribe_Values.Look(ref duplicateComplexityNeutroamine,  "duplicateComplexityNeutroamine",   4);
        Scribe_Values.Look(ref duplicateNeedsArchites,          "duplicateNeedsArchites",           true);

        // Merge settings
        Scribe_Values.Look(ref merge,           "merge",            CurveType.Log);
        Scribe_Values.Look(ref mergeEnabled,    "mergeEnabled",     true);
        Scribe_Values.Look(ref consumeOnMerge,  "consumeOnMerge",   false);
        Scribe_Values.Look(ref workToMerge,     "workToMerge",      1.0f);
        Scribe_Values.Look(ref genepackMergeMax,"genepackMergeMax", 9);
        // Merge materials cost.
        Scribe_Values.Look(ref mergeBaseNeutroamine,        "mergeBaseNeutroamine",         6);
        Scribe_Values.Look(ref mergeComplexityNeutroamine,  "mergeComplexityNeutroamine",   3);
        Scribe_Values.Look(ref mergeNeedsArchites,          "mergeNeedsArchites",           true);

        // Archite Penalty
        Scribe_Values.Look(ref architePen, "architePen", 1f);

        // TODO:
        // Worker settings
        Scribe_Values.Look(ref skillImportance, "skillImportance", 1f);
        Scribe_Values.Look(ref skillGain,       "skillGain", 1f);

        base.ExposeData();
    }
}

