using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;
using Verse.AI.Group;
using Verse.Grammar;

namespace VanillaSocialInteractionsExpanded
{
    public class LordJob_Joinable_MovieNight : LordJob_Joinable_DoublePawn
    {
        public LordJob_Joinable_MovieNight()
        {

        }
        public LordJob_Joinable_MovieNight(Pawn organizer, Pawn companion, IntVec3 spot, GatheringDef gatheringDef, int ticks)
                : base(organizer, companion, spot, gatheringDef, ticks)
        {

        }
        protected override void ApplyOutcome(LordToil_Party toil)
        {
            base.ApplyOutcome(toil);
            var organizerPartner = organizer.GetSpouseOrLoverOrFiance();
            if (organizerPartner != null && secondPawn != organizerPartner)
            {
                Thought_Memory thought_Memory = (Thought_Memory)ThoughtMaker.MakeThought(VSIE_DefOf.VSIE_JealouslyMyPartnerWatchedTVWithSomeoneElse);
                organizerPartner.needs.mood.thoughts.memories.TryGainMemory(thought_Memory, organizer);
            }
            var secondPawnPartner = secondPawn.GetSpouseOrLoverOrFiance();
            if (secondPawnPartner != null && organizer != secondPawnPartner)
            {
                Thought_Memory thought_Memory = (Thought_Memory)ThoughtMaker.MakeThought(VSIE_DefOf.VSIE_JealouslyMyPartnerWatchedTVWithSomeoneElse);
                secondPawnPartner.needs.mood.thoughts.memories.TryGainMemory(thought_Memory, secondPawn);
            }
        }
        public override void ExposeData()
        {
            base.ExposeData();
            if (Scribe.mode == LoadSaveMode.PostLoadInit && gatheringDef == null)
            {
                gatheringDef = VSIE_DefOf.VSIE_MovieNight;
            }
        }
    }
}