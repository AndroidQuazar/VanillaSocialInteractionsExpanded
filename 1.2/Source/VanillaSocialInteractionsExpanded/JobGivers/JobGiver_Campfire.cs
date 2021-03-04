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
    public class JobGiver_BuildCampfire : ThinkNode_JobGiver
    {
        protected override Job TryGiveJob(Pawn pawn)
        {
            if (pawn.WorkTypeIsDisabled(WorkTypeDefOf.Construction))
            {
                return null;
            }
            if (!JoyUtility.EnjoyableOutsideNow(pawn))
            {
                return null;
            }
            var centerCell = pawn.mindState.duty.focus.Cell;

            var campfires = pawn.Map.listerThings.ThingsOfDef(ThingDefOf.Campfire);
            var blueprintCampfires = pawn.Map.listerThings.ThingsOfDef(ThingDefOf.Campfire.blueprintDef);

            if (!blueprintCampfires.Any(x => x.Position.DistanceTo(centerCell) <= 10f) && !campfires.Any(x => x.Position.DistanceTo(centerCell) <= 10f) 
                && GatheringsUtility.TryFindRandomCellInGatheringArea(pawn, out IntVec3 result))
            {
                var blueprint = GenConstruct.PlaceBlueprintForBuild(ThingDefOf.Campfire, result, pawn.Map, Rot4.North, pawn.Faction, GenStuff.DefaultStuffFor(ThingDefOf.Campfire));
                var wrk = new WorkGiver_ConstructDeliverResourcesToBlueprints();
                wrk.def = DefDatabase<WorkGiverDef>.GetNamed("ConstructDeliverResourcesToBlueprints");
                var job = wrk.JobOnThing(pawn, blueprint, true);
                if (job != null)
                {
                    return job;
                }
                else
                {
                    blueprint.Destroy();
                }
            }
            return null;
        }
    }
}

