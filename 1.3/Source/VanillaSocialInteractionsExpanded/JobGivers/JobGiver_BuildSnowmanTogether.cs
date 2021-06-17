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
    public class JobGiver_BuildSnowmanTogether : JobGiver_BuildSnowman
	{
		protected override Job TryGiveJob(Pawn pawn)
		{
			var companion = VSIE_Utils.GetCompanion(pawn);
			if (companion.CurJobDef == VSIE_DefOf.VSIE_BuildSnowmanTogether)
            {
				return JobMaker.MakeJob(VSIE_DefOf.VSIE_BuildSnowmanTogether, companion.CurJob.targetA, companion);
			}
			else if (CanBuildSnowman(pawn, out IntVec3 c))
            {
				return JobMaker.MakeJob(VSIE_DefOf.VSIE_BuildSnowmanTogether, c, companion);
            }
			return null;
		}
	}
}

