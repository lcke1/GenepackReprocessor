using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Jobs.LowLevel.Unsafe;
using Verse;
using Verse.AI;

namespace GenepackReprocessor;

[DefOf]
public static class GeneSeparator_JobDefOfs
{
    public static JobDef SeparateGenepack;
}

public class JobDriver_SeparateGenepack : JobDriver
{
    // Settings for mod
    private static GenepackReprocessorSettings? _settings;
    public static GenepackReprocessorSettings Settings => _settings ??= LoadedModManager.GetMod<GenepackImprovMod>().GetSettings<GenepackReprocessorSettings>();

    private const int JobEndInterval = 4000;

    private Building_GeneSeparator Geneticist => (Building_GeneSeparator)base.TargetThingA;

    public override bool TryMakePreToilReservations(bool errorOnFailed)
    {
        if (pawn.Reserve(Geneticist, job, 1, -1, null, errorOnFailed))
        {
            return pawn.ReserveSittableOrSpot(Geneticist.InteractionCell, job, errorOnFailed);
        }
        return false;
    }

    protected override IEnumerable<Toil> MakeNewToils()
    {
        this.FailOnDespawnedNullOrForbidden(TargetIndex.A);
        this.FailOn(() => !Geneticist.CanBeWorkedOnNow.Accepted);
        yield return Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.InteractionCell);
        Toil toil = ToilMaker.MakeToil("MakeNewToils");
        toil.tickAction = delegate
        {
            float workAmount = pawn.GetStatValue(GeneSeparator_DefOfs.GenepackCreationSpeed) * Geneticist.GetStatValue(StatDefOf.AssemblySpeedFactor);
            Geneticist.DoWork(workAmount);
            pawn.skills.Learn(SkillDefOf.Intellectual, 0.1f * Settings.skillGain);
            pawn.GainComfortFromCellIfPossible(chairsOnly: true);
            if (Geneticist.ProgressPercent >= 1f)
            {
                Geneticist.Finish();
                pawn.jobs.EndCurrentJob(JobCondition.Succeeded);
            }
        };
        toil.FailOnCannotTouch(TargetIndex.A, PathEndMode.InteractionCell);
        toil.WithEffect(EffecterDefOf.GeneAssembler_Working, TargetIndex.A);
        toil.WithProgressBar(TargetIndex.A, () => Geneticist.ProgressPercent, interpolateBetweenActorAndTarget: false, 0f);
        toil.defaultCompleteMode = ToilCompleteMode.Never;
        toil.defaultDuration = 4000;
        toil.activeSkill = () => SkillDefOf.Intellectual;
        yield return toil;
    }
}