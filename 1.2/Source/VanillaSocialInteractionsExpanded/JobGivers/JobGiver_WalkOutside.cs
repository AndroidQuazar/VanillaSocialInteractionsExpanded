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
	public class JobGiver_WalkOutside : ThinkNode_JobGiver
	{
		public IntRange ticksRange = new IntRange(300, 600);
		public override ThinkNode DeepCopy(bool resolve = true)
		{
			JobGiver_WalkOutside obj = (JobGiver_WalkOutside)base.DeepCopy(resolve);
			obj.ticksRange = ticksRange;
			return obj;
		}

		protected override Job TryGiveJob(Pawn pawn)
		{
			Pawn target = VSIE_Utils.GetCompanion(pawn);
			if (target != null)
            {
				if (target.CurJobDef == VSIE_DefOf.VSIE_GotoWith)
                {
					return JobMaker.MakeJob(VSIE_DefOf.VSIE_Follow, target);
				}
				else if (RCellFinder.TryFindRandomSpotJustOutsideColony(pawn.Position, pawn.Map, pawn, out var result, (IntVec3 cell) => 
					!cell.Roofed(pawn.Map) && pawn.Position.DistanceTo(cell) > 50 && pawn.Rotation.FacingCell.DistanceTo(cell) < pawn.Position.DistanceTo(cell)))
				{
					Job job = JobMaker.MakeJob(VSIE_DefOf.VSIE_GotoWith, target, result);
					job.expiryInterval = ticksRange.RandomInRange;
					return job;
				}
			}
			return null;
		}
	}
}

