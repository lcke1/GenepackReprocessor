using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse.AI;
using Verse;

namespace GenepackReprocessor;

public class WorkGiver_SeparateGenepack : WorkGiver_Scanner
{
    public override ThingRequest PotentialWorkThingRequest => ThingRequest.ForDef(GeneSeparator_DefOfs.GeneSeparator);

    public override bool ShouldSkip(Pawn pawn, bool forced = false)
    {
        return !ModsConfig.BiotechActive;
    }

    public override bool HasJobOnThing(Pawn pawn, Thing t, bool forced = false)
    {
        if (!(t is Building_GeneSeparator building_GeneSeparator))
        {
            return false;
        }
        if (!pawn.CanReserve(t, 1, -1, null, forced) || !pawn.CanReserveSittableOrSpot(t.InteractionCell, forced))
        {
            return false;
        }
        if (building_GeneSeparator.ArchitesRequiredNow > 0)
        {
            if (FindArchiteCapsule(pawn) == null)
            {
                JobFailReason.Is("NoIngredient".Translate(ThingDefOf.ArchiteCapsule));
                return false;
            }
            return true;
        }
        if (building_GeneSeparator.NeutroamineRequiredNow > 0)
        {
            if (FindNeutroamine(pawn) == null)
            {
                JobFailReason.Is("NoIngredient".Translate(GeneSeparator_DefOfs.Neutroamine));
                return false;
            }
            return true;
        }
        if (!building_GeneSeparator.CanBeWorkedOnNow.Accepted)
        {
            return false;
        }
        return true;
    }

    public override Job JobOnThing(Pawn pawn, Thing t, bool forced = false)
    {
        if (!(t is Building_GeneSeparator building_GeneSeparator))
        {
            return null;
        }
        if (building_GeneSeparator.ArchitesRequiredNow > 0)
        {
            Thing thing = FindArchiteCapsule(pawn);
            if (thing != null)
            {
                Job job = JobMaker.MakeJob(JobDefOf.HaulToContainer, thing, t);
                job.count = Mathf.Min(building_GeneSeparator.ArchitesRequiredNow, thing.stackCount);
                return job;
            }
        }
        if (building_GeneSeparator.NeutroamineRequiredNow > 0)
        {
            Thing thing = FindNeutroamine(pawn);
            if (thing != null)
            {
                Job job = JobMaker.MakeJob(JobDefOf.HaulToContainer, thing, t);
                job.count = Mathf.Min(building_GeneSeparator.NeutroamineRequiredNow, thing.stackCount);
                return job;
            }
        }
        return JobMaker.MakeJob(GeneSeparator_JobDefOfs.SeparateGenepack, t, 5000, checkOverrideOnExpiry: true);
    }

    // For loading in the need resources
    private Thing FindArchiteCapsule(Pawn pawn)
    {
        return GenClosest.ClosestThingReachable(pawn.Position, pawn.Map, ThingRequest.ForDef(ThingDefOf.ArchiteCapsule), PathEndMode.ClosestTouch, TraverseParms.For(pawn), 9999f, (Thing x) => !x.IsForbidden(pawn) && pawn.CanReserve(x));
    }

    private Thing FindNeutroamine(Pawn pawn)
    {
        return GenClosest.ClosestThingReachable(pawn.Position, pawn.Map, ThingRequest.ForDef(GeneSeparator_DefOfs.Neutroamine), PathEndMode.ClosestTouch, TraverseParms.For(pawn), 9999f, (Thing x) => !x.IsForbidden(pawn) && pawn.CanReserve(x));
    }

}
