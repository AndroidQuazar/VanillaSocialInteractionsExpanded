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
    public class JobGiver_TakeABeer : ThinkNode_JobGiver
    {
        protected override Job TryGiveJob(Pawn pawn)
        {
            var alcohol = pawn.health.hediffSet.hediffs.FirstOrDefault(x => x is Hediff_Alcohol);
            if (alcohol != null && alcohol.Severity > 0.1f)
            {
                return null;
            }
            Thing drug = pawn.inventory.innerContainer.FirstOrDefault(x => VSIE_Utils.DrugValidator(pawn, x));
            if (drug is null)
            {
                var things = pawn.Map.listerThings.ThingsInGroup(ThingRequestGroup.Drug).Where(x => VSIE_Utils.DrugValidator(pawn, x)).ToList();
                drug = GenClosest.ClosestThing_Global_Reachable(pawn.Position, pawn.Map, things, PathEndMode.OnCell, TraverseParms.For(pawn), 9999f, null);
            }
            if (drug != null)
            {
                if (GatheringsUtility.TryFindRandomCellInGatheringArea(pawn, out IntVec3 result))
                {
                    Job job = JobMaker.MakeJob(VSIE_DefOf.VSIE_TakingBeer, drug, result);
                    job.count = 1;
                    return job;
                }
            }
            return null;
        }
    }
}

