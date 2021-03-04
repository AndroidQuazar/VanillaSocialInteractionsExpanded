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
	public class GatheringWorker_GoingForAWalk : GatheringWorker_Dating
	{
        protected override bool MemberValidator(Pawn pawn)
        {
            return JoyUtility.EnjoyableOutsideNow(pawn) && base.MemberValidator(pawn);
        }
        protected override LordJob CreateLordJobCustom(IntVec3 spot, Pawn organizer, Pawn companion)
        {
            return new LordJob_Joinable_GoingForAWalk(organizer, companion, spot, this.def, new IntRange(2 * GenDate.TicksPerHour, 4 * GenDate.TicksPerHour).RandomInRange);
        }
    }
}