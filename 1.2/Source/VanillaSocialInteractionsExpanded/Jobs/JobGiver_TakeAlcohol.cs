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
    public class JobGiver_TakeAlcohol : ThinkNode_JobGiver
    {
        protected override Job TryGiveJob(Pawn pawn)
        {
            var carriedThing = pawn.carryTracker.CarriedThing;
            if (VSIE_Utils.DrugValidator(pawn, carriedThing))
            {
                Log.Message("JobGiver_TakeAlcohol : ThinkNode_JobGiver - TryGiveJob - return null; - 3", true);
                return null;
            }

            var things = pawn.Map.listerThings.ThingsInGroup(ThingRequestGroup.Drug).Where(x => VSIE_Utils.DrugValidator(pawn, x)).ToList();
            Thing drug = GenClosest.ClosestThing_Global_Reachable(pawn.Position, pawn.Map, things, PathEndMode.OnCell, TraverseParms.For(pawn), 9999f, null);
            if (drug != null && GatheringsUtility.TryFindRandomCellInGatheringArea(pawn, out IntVec3 result))
            {
                Log.Message("JobGiver_TakeAlcohol : ThinkNode_JobGiver - TryGiveJob - Job job = JobMaker.MakeJob(VSIE_DefOf.VSIE_TakingBeer, drug, result); - 7", true);
                Job job = JobMaker.MakeJob(VSIE_DefOf.VSIE_TakingBeer, drug, result);
                job.count = 1;
                return job;
            }
            return null;
        }
    }
}