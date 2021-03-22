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
	public class JobGiver_GotoTalkToSecondPawn : ThinkNode_JobGiver
	{
		protected override Job TryGiveJob(Pawn pawn)
		{
			Pawn target = VSIE_Utils.GetCompanion(pawn);
			if (target != null)
            {
				if (target.Position.DistanceTo(pawn.Position) > 10)
                {
					var job = JobMaker.MakeJob(VSIE_DefOf.VSIE_GotoTalkToSecondPawn, target); 
					return job;
				}
			}
			return null;
		}
	}
}

