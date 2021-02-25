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
	public class JobGiver_TalkToSecondPawn : ThinkNode_JobGiver
	{
		public IntRange ticksRange = new IntRange(300, 600);
		public override ThinkNode DeepCopy(bool resolve = true)
		{
			JobGiver_TalkToSecondPawn obj = (JobGiver_TalkToSecondPawn)base.DeepCopy(resolve);
			obj.ticksRange = ticksRange;
			return obj;
		}

		protected override Job TryGiveJob(Pawn pawn)
		{
			Pawn target = VSIE_Utils.GetSecondPawnToTalk(pawn);
			if (target != null)
            {
				Job job = JobMaker.MakeJob(VSIE_DefOf.VSIE_TalkToSecondPawn, target);
				job.expiryInterval = ticksRange.RandomInRange;
				return job;
			}
			return null;
		}
	}
}

