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
			Pawn secondPawn = VSIE_Utils.GetSecondPawnToTalk(pawn);
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

		protected override IntVec3 GetWanderRoot(Pawn pawn)
		{
			throw new NotImplementedException();
		}
	}
}

