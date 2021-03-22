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
    public class LordJob_Joinable_MealTogether : LordJob_Joinable_DoublePawn
    {
        public Dictionary<Pawn, int> mealsEated = new Dictionary<Pawn, int>();
        public LordJob_Joinable_MealTogether()
        {

        }
        public LordJob_Joinable_MealTogether(Pawn organizer, Pawn companion, IntVec3 spot, GatheringDef gatheringDef, int ticks)
                : base(organizer, companion, spot, gatheringDef, ticks)
        {

        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Collections.Look(ref mealsEated, "mealsEated", LookMode.Reference, LookMode.Value, ref pawnKeys, ref intValues);
            if (Scribe.mode == LoadSaveMode.PostLoadInit && gatheringDef == null)
            {
                gatheringDef = VSIE_DefOf.VSIE_MealTogether;
            }
        }

        private List<Pawn> pawnKeys;
        private List<int> intValues;
    }
}