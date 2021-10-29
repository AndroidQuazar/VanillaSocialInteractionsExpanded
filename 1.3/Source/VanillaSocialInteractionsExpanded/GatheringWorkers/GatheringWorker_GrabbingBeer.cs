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
    public class GatheringWorker_GrabbingBeer : GatheringWorker_DoublePawn
    {
        protected override bool ConditionsMeet(Pawn organizer)
        {
            if (!VanillaSocialInteractionsExpandedSettings.EnableGroupActivities)
            {
                return false;
            }
            if (organizer.Map.listerThings.ThingsInGroup(ThingRequestGroup.Drug).Any(x => VSIE_Utils.DrugValidator(organizer, x)))
            {
                return true;
            }
            return false;
        }
        protected override bool MemberValidator(Pawn pawn)
        {
            var result = !pawn.health.hediffSet.hediffs.Any(y => y is Hediff_Alcohol x && x.Severity > 0.1f) && !VSIE_Utils.workTags.Contains(pawn.mindState.lastJobTag);
            if (result && ModsConfig.IdeologyActive && pawn.Ideo != null && pawn.Ideo.PreceptsListForReading.Any(x => x.def.defName == "DrugUse_Prohibited" || x.def.defName == "DrugUse_MedicalOnly"))
            {
                result = false;
            }
            return result;
        }
        protected override bool PawnsCanGatherTogether(Pawn organizer, Pawn companion)
        {
            return companion.relations.OpinionOf(organizer) >= 20 && organizer.relations.OpinionOf(companion) >= 20;
        }
        protected override float SortCandidatesBy(Pawn organizer, Pawn candidate)
        {
            return organizer.relations.OpinionOf(candidate);
        }
        protected override LordJob CreateLordJobCustom(IntVec3 spot, Pawn organizer, Pawn companion)
        {
            return new LordJob_Joinable_GrabbingBeer(organizer, companion, spot, this.def, new IntRange(4 * GenDate.TicksPerHour, 6 * GenDate.TicksPerHour).RandomInRange);
        }
    }
}