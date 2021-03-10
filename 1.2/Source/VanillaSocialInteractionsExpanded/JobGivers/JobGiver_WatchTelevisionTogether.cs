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
    public class JobGiver_WatchTelevisionTogether : ThinkNode_JobGiver
    {
		protected override Job TryGiveJob(Pawn pawn)
		{
			var companion = VSIE_Utils.GetCompanion(pawn);
			if (companion.CurJobDef == VSIE_DefOf.VSIE_WatchTelevisionTogether)
			{
				if (WatchBuildingUtilityDoublePawn.TryFindBestWatchCellNear(companion.CurJob.targetA.Thing, pawn, companion.CurJob.targetC.Thing as Building, 
					true, out IntVec3 result, out Building chair))
                {
					return JobMaker.MakeJob(VSIE_DefOf.VSIE_WatchTelevisionTogether, companion.CurJob.targetA.Thing, result, chair);
				}
			}
			else if (GatheringWorker_MovieNight.TryFindTelevisionFor(pawn, companion, out Building television, out IntVec3 spot, out Building chair))
			{
				return JobMaker.MakeJob(VSIE_DefOf.VSIE_WatchTelevisionTogether, television, spot, chair);
			}
			return null;
		}
	}
}

