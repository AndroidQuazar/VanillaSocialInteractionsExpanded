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
    public class GatheringWorker_BuildingSnowmen : GatheringWorker_Dating
    {
        protected override bool MemberValidator(Pawn pawn)
        {
            return JoyUtility.EnjoyableOutsideNow(pawn) && base.MemberValidator(pawn);
        }
        protected override bool ConditionsMeet(Pawn organizer)
        {
            return JobGiver_BuildSnowman.CanBuildSnowman(organizer, out var cell) && base.ConditionsMeet(organizer);
        }
        protected override bool TryFindGatherSpot(Pawn organizer, out IntVec3 spot)
        {
            if (JobGiver_BuildSnowman.CanBuildSnowman(organizer, out var cell))
            {
                spot = cell;
                return true;
            }
            else
            {
                spot = IntVec3.Invalid;
                return false;
            }
        }
        protected override LordJob CreateLordJobCustom(IntVec3 spot, Pawn organizer, Pawn companion)
        {
            return new LordJob_Joinable_BuildingSnowmen(organizer, companion, spot, this.def, new IntRange(2 * GenDate.TicksPerHour, 4 * GenDate.TicksPerHour).RandomInRange);
        }
    }
}