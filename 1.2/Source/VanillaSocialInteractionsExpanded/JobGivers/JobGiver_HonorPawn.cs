using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;
using Verse.AI;
using Verse.AI.Group;
using Verse.Grammar;

namespace VanillaSocialInteractionsExpanded
{
	public class JobGiver_HonorPawn : ThinkNode_JobGiver
	{
		protected override Job TryGiveJob(Pawn pawn)
		{
			var graves = pawn.DutyLocation().GetThingList(pawn.Map).Where(x => x is Building_Grave);
			if (graves.Any())
            {
				var grave = graves.First();
				if (pawn.Position.DistanceTo(grave.Position) > 3)
                {
					if (CellFinder.TryFindRandomCellNear(grave.Position, pawn.Map, 3, (IntVec3 cell) => grave.Position.DistanceTo(cell) > 1 && !grave.OccupiedRect().Cells.Contains(cell) 
						&& cell.Walkable(pawn.Map) && pawn.CanReserveAndReach(cell, PathEndMode.OnCell, Danger.Deadly) && GenSight.LineOfSight(grave.Position, cell, pawn.Map), out IntVec3 result))
					{
						var job = JobMaker.MakeJob(JobDefOf.Goto, result);
						job.locomotionUrgency = LocomotionUrgency.Walk;
						return job;
					}
				}
				else
                {
					return JobMaker.MakeJob(VSIE_DefOf.VSIE_HonorPawn, grave);
				}
			}
			return null;
		}
	}
}

