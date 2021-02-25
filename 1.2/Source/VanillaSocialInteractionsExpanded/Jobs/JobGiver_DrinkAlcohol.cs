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

namespace VanillaSocialInteractionsExpanded
{
    public class JobGiver_DrinkAlcohol : ThinkNode_JobGiver
    {
        protected override Job TryGiveJob(Pawn pawn)
        {
            foreach (var t in pawn.inventory.innerContainer)
            {
                Log.Message("THING: " + t);
            }
            Log.Message("JobGiver_TakeAlcohol : ThinkNode_JobGiver - TryGiveJob - var things = pawn.Map.listerThings.ThingsInGroup(ThingRequestGroup.Drug).Where(x => VSIE_Utils.DrugValidator(pawn, x)).ToList(); - 1", true);
            var things = pawn.Map.listerThings.ThingsInGroup(ThingRequestGroup.Drug).Where(x => VSIE_Utils.DrugValidator(pawn, x)).ToList();
            Log.Message("JobGiver_TakeAlcohol : ThinkNode_JobGiver - TryGiveJob - Thing drug = GenClosest.ClosestThing_Global_Reachable(pawn.Position, pawn.Map, things, PathEndMode.OnCell, TraverseParms.For(pawn), 9999f, null); - 2", true);
            Thing drug = GenClosest.ClosestThing_Global_Reachable(pawn.Position, pawn.Map, things, PathEndMode.OnCell, TraverseParms.For(pawn), 9999f, null);
            Log.Message("JobGiver_TakeAlcohol : ThinkNode_JobGiver - TryGiveJob - if (drug != null) - 3", true);
            if (drug != null)
            {
                Log.Message("JobGiver_TakeAlcohol : ThinkNode_JobGiver - TryGiveJob - if (GatheringsUtility.InGatheringArea(pawn.Position, pawn.mindState.duty.focus.Cell, pawn.Map) && GatheringsUtility.InGatheringArea(drug.Position, pawn.mindState.duty.focus.Cell, pawn.Map)) - 4", true);
                if (GatheringsUtility.InGatheringArea(pawn.Position, pawn.mindState.duty.focus.Cell, pawn.Map) && GatheringsUtility.InGatheringArea(drug.Position, pawn.mindState.duty.focus.Cell, pawn.Map))
                {
                    Log.Message("JobGiver_TakeAlcohol : ThinkNode_JobGiver - TryGiveJob - Job job = JobMaker.MakeJob(JobDefOf.Ingest, drug); - 5", true);
                    Job job = JobMaker.MakeJob(JobDefOf.Ingest, drug);
                    Log.Message("JobGiver_TakeAlcohol : ThinkNode_JobGiver - TryGiveJob - job.count = 1; - 6", true);
                    job.count = 1;
                    Log.Message("JobGiver_TakeAlcohol : ThinkNode_JobGiver - TryGiveJob - return job; - 7", true);
                    return job;
                }
                else if (GatheringsUtility.TryFindRandomCellInGatheringArea(pawn, out IntVec3 result))
                {
                    Log.Message("JobGiver_TakeAlcohol : ThinkNode_JobGiver - TryGiveJob - Job job = JobMaker.MakeJob(VSIE_DefOf.VSIE_TakingBeer, drug, result); - 9", true);
                    Job job = JobMaker.MakeJob(VSIE_DefOf.VSIE_TakingBeer, drug, result);
                    Log.Message("JobGiver_TakeAlcohol : ThinkNode_JobGiver - TryGiveJob - job.count = 1; - 10", true);
                    job.count = 1;
                    Log.Message("JobGiver_TakeAlcohol : ThinkNode_JobGiver - TryGiveJob - return job; - 11", true);
                    return job;
                }
            }
            return null;
        }
    }
}

