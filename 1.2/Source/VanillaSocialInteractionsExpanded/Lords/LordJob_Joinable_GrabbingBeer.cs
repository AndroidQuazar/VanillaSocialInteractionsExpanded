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
    public class LordJob_Joinable_GrabbingBeer : LordJob_Joinable_DoublePawn
    {
        protected override ThoughtDef OrganizerThought => VSIE_DefOf.VSIE_HadNiceChatWithBeer;

        protected override ThoughtDef AttendeeThought => VSIE_DefOf.VSIE_HadNiceChatWithBeer;
        public LordJob_Joinable_GrabbingBeer()
        {

        }
        public LordJob_Joinable_GrabbingBeer(Pawn organizer, Pawn companion, IntVec3 spot, GatheringDef gatheringDef, int ticks)
        : base(organizer, companion, spot, gatheringDef, ticks)
        {

        }
        public override void ExposeData()
        {
            base.ExposeData();
            if (Scribe.mode == LoadSaveMode.PostLoadInit && gatheringDef == null)
            {
                gatheringDef = VSIE_DefOf.VSIE_GrabbingBeer;
            }
        }
    }
}