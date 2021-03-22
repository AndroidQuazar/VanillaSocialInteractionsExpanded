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
	public class JobGiver_WanderAroundSecondPawn : JobGiver_Wander
	{
		protected override IntVec3 GetExactWanderDest(Pawn pawn)
		{
			Pawn secondPawn = VSIE_Utils.GetCompanion(pawn);
			if (secondPawn != null && GatheringsUtility.InGatheringArea(secondPawn.Position, pawn.DutyLocation(), pawn.Map))
            {
				return RCellFinder.RandomWanderDestFor(pawn, secondPawn.Position, wanderRadius, wanderDestValidator, PawnUtility.ResolveMaxDanger(pawn, maxDanger));
			}

			if (!GatheringsUtility.TryFindRandomCellInGatheringArea(pawn, out IntVec3 result))
			{
				return IntVec3.Invalid;
			}
			return result;
		}
		protected override Job TryGiveJob(Pawn pawn)
		{
			bool flag = pawn.CurJob != null && pawn.CurJob.def == JobDefOf.GotoWander;
			bool nextMoveOrderIsWait = pawn.mindState.nextMoveOrderIsWait;
			if (!flag)
			{
				pawn.mindState.nextMoveOrderIsWait = !pawn.mindState.nextMoveOrderIsWait;
			}
			if (nextMoveOrderIsWait && !flag)
			{
				Job job = JobMaker.MakeJob(JobDefOf.Wait_Wander);
				job.expiryInterval = ticksBetweenWandersRange.RandomInRange;
				return job;
			}
			IntVec3 exactWanderDest = GetExactWanderDest(pawn);
			if (!exactWanderDest.IsValid)
			{
				pawn.mindState.nextMoveOrderIsWait = false;
				return null;
			}
			Job job2 = JobMaker.MakeJob(VSIE_DefOf.VSIE_GotoWith, exactWanderDest, VSIE_Utils.GetCompanion(pawn));
			job2.locomotionUrgency = locomotionUrgency;
			job2.expiryInterval = expiryInterval;
			job2.checkOverrideOnExpire = true;
			return job2;
		}
		protected override IntVec3 GetWanderRoot(Pawn pawn)
		{
			throw new NotImplementedException();
		}
	}
}

