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
    public class GatheringWorker_ViewingArtTogether : GatheringWorker_Dating
    {
        protected override bool ConditionsMeet(Pawn organizer)
        {
            bool result = false;
            if (JobGiver_ViewingArtTogether.TryFindArtToView(organizer, out var thing))
            {
                result = true;
            }
            return result && base.ConditionsMeet(organizer);
        }
        protected override bool TryFindGatherSpot(Pawn organizer, out IntVec3 spot)
        {
            if (JobGiver_ViewingArtTogether.TryFindArtToView(organizer, out Thing thing))
            {
                spot = thing.Position;
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
            return new LordJob_Joinable_ViewingArtTogether(organizer, companion, spot, this.def, new IntRange(2 * GenDate.TicksPerHour, 4 * GenDate.TicksPerHour).RandomInRange);
        }
    }
}