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
    public class JobGiver_DrinkAlcoholInGatheringArea : ThinkNode_JobGiver
    {
        protected override Job TryGiveJob(Pawn pawn)
        {
            var thing = FindDrug(pawn);
            if (thing != null)
            {
                Job job = JobMaker.MakeJob(JobDefOf.Ingest, thing);
                job.count = FoodUtility.WillIngestStackCountOf(pawn, thing.def, thing.def.GetStatValueAbstract(StatDefOf.Nutrition));
                Log.Message("JobGiver_DrinkAlcoholInGatheringArea : ThinkNode_JobGiver - TryGiveJob - return job; - 5", true);
                return job;
            }
            else
            {
                var things = pawn.Map.listerThings.ThingsInGroup(ThingRequestGroup.Drug).Where(x => VSIE_Utils.DrugValidator(pawn, x));
                Thing drug = GenClosest.ClosestThing_Global_Reachable(pawn.Position, pawn.Map, things, PathEndMode.OnCell, TraverseParms.For(pawn), 9999f, null);
                if (drug != null && GatheringsUtility.TryFindRandomCellInGatheringArea(pawn, out IntVec3 result))
                {
                    Job job = JobMaker.MakeJob(VSIE_DefOf.VSIE_TakingBeer, drug, result);
                    job.count = 1;
                    Log.Message("JobGiver_DrinkAlcoholInGatheringArea : ThinkNode_JobGiver - TryGiveJob - return job; - 11", true);
                    return job;
                }
                return null;
            }
        }

        private Thing FindDrug(Pawn pawn)
        {
            var carriedThing = pawn.carryTracker.CarriedThing;
            if (VSIE_Utils.DrugValidator(pawn, carriedThing))
            {
                if (pawn.carryTracker.TryDropCarriedThing(pawn.Position, ThingPlaceMode.Direct, out Thing resultingThing))
                {
                    pawn.inventory.TryAddItemNotForSale(resultingThing);
                    return resultingThing;
                }
                return carriedThing;
            }
            for (int k = 0; k < pawn.inventory.innerContainer.Count; k++)
            {
                Thing thing = pawn.inventory.innerContainer[k];
                if (VSIE_Utils.DrugValidator(pawn, thing))
                {
                    return thing;
                }
            }
            return null;
        }
    }
}