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
    public class GatheringWorker_GoingForAWalk : GatheringWorker_DoublePawn
    {
        protected override bool MemberValidator(Pawn pawn)
        {
            var value = !VSIE_Utils.workTags.Contains(pawn.mindState.lastJobTag);
            return value;
        }
        protected override bool PawnsCanGatherTogether(Pawn organizer, Pawn companion)
        {
			return BasicLovePartnerRelationGenerationChance(organizer, companion) != 0f && companion.relations.OpinionOf(organizer) >= 0 && organizer.relations.OpinionOf(companion) >= 0 
                && organizer.relations.CompatibilityWith(companion) >= 1f && companion.relations.CompatibilityWith(organizer) >= 1f;
        }

		public float BasicLovePartnerRelationGenerationChance(Pawn generated, Pawn other)
		{
			if (generated.ageTracker.AgeBiologicalYearsFloat < 14f)
			{
				return 0f;
			}
			if (other.ageTracker.AgeBiologicalYearsFloat < 14f)
			{
				return 0f;
			}
			if (generated.gender == other.gender && (!other.story.traits.HasTrait(TraitDefOf.Gay)))
			{
				return 0f;
			}
			if (generated.gender != other.gender && other.story.traits.HasTrait(TraitDefOf.Gay))
			{
				return 0f;
			}
			return 1f;
		}
		protected override float SortCandidatesBy(Pawn organizer, Pawn candidate)
        {
            return organizer.relations.OpinionOf(candidate);
        }
        protected override LordJob CreateLordJobCustom(IntVec3 spot, Pawn organizer, Pawn companion)
        {
            return new LordJob_Joinable_GoingForAWalk(organizer, companion, spot, this.def, new IntRange(2 * GenDate.TicksPerHour, 4 * GenDate.TicksPerHour).RandomInRange);
        }
    }
}