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
    public class LordJob_Joinable_Skygazing : LordJob_Joinable_DoublePawn
    {
        public LordJob_Joinable_Skygazing()
        {

        }
        public LordJob_Joinable_Skygazing(Pawn organizer, Pawn companion, IntVec3 spot, GatheringDef gatheringDef, int ticks)
                : base(organizer, companion, spot, gatheringDef, ticks)
        {

        }

        public override void ExposeData()
        {
            base.ExposeData();
            if (Scribe.mode == LoadSaveMode.PostLoadInit && gatheringDef == null)
            {
                gatheringDef = VSIE_DefOf.VSIE_Skygazing;
            }
        }
    }
}