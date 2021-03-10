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
    public class GatheringWorker_DoublePawn : GatheringWorker
    {

        public override bool CanExecute(Map map, Pawn organizer = null)
        {
            Log.Message("GatheringWorker_DoublePawn : GatheringWorker - CanExecute - if (organizer == null) - 1", true);
            if (organizer == null)
            {
                Log.Message("GatheringWorker_DoublePawn : GatheringWorker - CanExecute - organizer = FindOrganizerCustom(map, out var companion); - 2", true);
                organizer = FindOrganizerCustom(map, out var companion);
                Log.Message("GatheringWorker_DoublePawn : GatheringWorker - CanExecute - if (organizer is null) - 3", true);
                if (organizer is null)
                {
                    Log.Message($"{this.def.defName} - organizer isn't found");
                    Log.Message("GatheringWorker_DoublePawn : GatheringWorker - CanExecute - return false; - 5", true);
                    return false;
                }
                else if (companion is null)
                {
                    Log.Message($"{this.def.defName} - companion isn't found");
                    Log.Message("GatheringWorker_DoublePawn : GatheringWorker - CanExecute - return false; - 8", true);
                    return false;
                }
            }
            Log.Message("GatheringWorker_DoublePawn : GatheringWorker - CanExecute - if (!TryFindGatherSpot(organizer, out IntVec3 _)) - 9", true);
            if (!TryFindGatherSpot(organizer, out IntVec3 _))
            {
                Log.Message($"{this.def.defName} - unable to find gather spot");
                Log.Message("GatheringWorker_DoublePawn : GatheringWorker - CanExecute - return false; - 11", true);
                return false;
            }
            Log.Message("GatheringWorker_DoublePawn : GatheringWorker - CanExecute - if (!GatheringsUtility.PawnCanStartOrContinueGathering(organizer)) - 12", true);
            if (!GatheringsUtility.PawnCanStartOrContinueGathering(organizer))
            {
                Log.Message($"{this.def.defName} - {organizer} can't start or continue gathering");
                Log.Message("GatheringWorker_DoublePawn : GatheringWorker - CanExecute - return false; - 14", true);
                return false;
            }
            else if (FindCompanion(organizer, this.def) is null)
            {
                Log.Message($"{this.def.defName} - can't find a companion for {organizer}");
                Log.Message("GatheringWorker_DoublePawn : GatheringWorker - CanExecute - return false; - 17", true);
                return false;
            }
            else if (!ConditionsMeet(organizer))
            {
                Log.Message($"{this.def.defName} - outside conditions aren't meet");
                Log.Message("GatheringWorker_DoublePawn : GatheringWorker - CanExecute - return false; - 20", true);
                return false;
            }
            return true;
        }

        public override bool TryExecute(Map map, Pawn organizer = null)
        {
            Log.Message("GatheringWorker_DoublePawn : GatheringWorker - TryExecute - Pawn companion = null; - 22", true);
            Pawn companion = null;
            Log.Message("GatheringWorker_DoublePawn : GatheringWorker - TryExecute - if (organizer == null) - 23", true);
            if (organizer == null)
            {
                Log.Message("GatheringWorker_DoublePawn : GatheringWorker - TryExecute - organizer = FindOrganizerCustom(map, out companion); - 24", true);
                organizer = FindOrganizerCustom(map, out companion);
            }

            Log.Message("GatheringWorker_DoublePawn : GatheringWorker - TryExecute - if (organizer == null) - 25", true);
            if (organizer == null)
            {
                Log.Message("GatheringWorker_DoublePawn : GatheringWorker - TryExecute - return false; - 26", true);
                return false;
            }
            Log.Message("GatheringWorker_DoublePawn : GatheringWorker - TryExecute - if (!TryFindGatherSpot(organizer, out IntVec3 spot)) - 27", true);
            if (!TryFindGatherSpot(organizer, out IntVec3 spot))
            {
                Log.Message("GatheringWorker_DoublePawn : GatheringWorker - TryExecute - return false; - 28", true);
                return false;
            }
            Log.Message("GatheringWorker_DoublePawn : GatheringWorker - TryExecute - if (!GatheringsUtility.PawnCanStartOrContinueGathering(organizer)) - 29", true);
            if (!GatheringsUtility.PawnCanStartOrContinueGathering(organizer))
            {
                Log.Message("GatheringWorker_DoublePawn : GatheringWorker - TryExecute - return false; - 30", true);
                return false;
            }
            Log.Message("GatheringWorker_DoublePawn : GatheringWorker - TryExecute - if (organizer is null || companion is null || !ConditionsMeet(organizer)) - 31", true);
            if (organizer is null || companion is null || !ConditionsMeet(organizer))
            {
                Log.Message("GatheringWorker_DoublePawn : GatheringWorker - TryExecute - return false; - 32", true);
                return false;
            }
            LordJob lordJob = CreateLordJobCustom(spot, organizer, companion);
            Log.Message("GatheringWorker_DoublePawn : GatheringWorker - TryExecute - List<Pawn> pawns = new List<Pawn> { organizer, companion }; - 34", true);
            List<Pawn> pawns = new List<Pawn> { organizer, companion };
            Log.Message("GatheringWorker_DoublePawn : GatheringWorker - TryExecute - LordMaker.MakeNewLord(organizer.Faction, lordJob, organizer.Map, pawns); - 35", true);
            LordMaker.MakeNewLord(organizer.Faction, lordJob, organizer.Map, pawns);
            Log.Message("GatheringWorker_DoublePawn : GatheringWorker - TryExecute - SendLetterCustom(spot, organizer, companion); - 36", true);
            SendLetterCustom(spot, organizer, companion);
            Log.Message("GatheringWorker_DoublePawn : GatheringWorker - TryExecute - return true; - 37", true);
            return true;
        }

        protected virtual void SendLetterCustom(IntVec3 spot, Pawn organizer, Pawn companion)
        {
            Find.LetterStack.ReceiveLetter(def.letterTitle, def.letterText.Formatted(organizer.Named("ORGANIZER"), companion.Named("COMPANION")), LetterDefOf.PositiveEvent,
                new List<Pawn> { organizer, companion });
        }
        private Pawn FindOrganizerCustom(Map map, out Pawn companion)
        {
            Log.Message("GatheringWorker_DoublePawn : GatheringWorker - FindOrganizerCustom - var organizer = FindRandomGatheringOrganizer(Faction.OfPlayer, map, def, out companion); - 39", true);
            var organizer = FindRandomGatheringOrganizer(Faction.OfPlayer, map, def, out companion);
            Log.Message("GatheringWorker_DoublePawn : GatheringWorker - FindOrganizerCustom - if (organizer is null) - 40", true);
            if (organizer is null)
            {
                Log.Message("GatheringWorker_DoublePawn : GatheringWorker - FindOrganizerCustom - companion = null; - 41", true);
                companion = null;
                Log.Message("GatheringWorker_DoublePawn : GatheringWorker - FindOrganizerCustom - return null; - 42", true);
                return null;
            }
            return organizer;
        }

        private bool BasePawnValidator(Pawn pawn, GatheringDef gatheringDef)
        {
            var value = pawn.RaceProps.Humanlike && !pawn.InBed() && !pawn.InMentalState && pawn.GetLord() == null
            && GatheringsUtility.ShouldPawnKeepGathering(pawn, gatheringDef) && !pawn.Drafted && (gatheringDef.requiredTitleAny == null || gatheringDef.requiredTitleAny.Count == 0
            || (pawn.royalty != null && pawn.royalty.AllTitlesInEffectForReading.Any((RoyalTitle t) => gatheringDef.requiredTitleAny.Contains(t.def))));
            Log.Message("GatheringWorker_DoublePawn : GatheringWorker - BasePawnValidator - return value; - 45", true);
            return value;
        }
        public Pawn FindRandomGatheringOrganizer(Faction faction, Map map, GatheringDef gatheringDef, out Pawn companion)
        {
            Predicate<Pawn> v = (Pawn organizer) => BasePawnValidator(organizer, gatheringDef) && MemberValidator(organizer);// && FindCompanion(organizer, gatheringDef) != null;
            Log.Message("GatheringWorker_DoublePawn : GatheringWorker - FindRandomGatheringOrganizer - if (map.mapPawns.SpawnedPawnsInFaction(faction).Where(x => v(x)).TryRandomElement(out Pawn result)) - 47", true);
            if (map.mapPawns.SpawnedPawnsInFaction(faction).Where(x => v(x)).TryRandomElement(out Pawn result))
            {
                Log.Message("GatheringWorker_DoublePawn : GatheringWorker - FindRandomGatheringOrganizer - companion = FindCompanion(result, gatheringDef); - 48", true);
                companion = FindCompanion(result, gatheringDef);
                Log.Message("GatheringWorker_DoublePawn : GatheringWorker - FindRandomGatheringOrganizer - return result; - 49", true);
                return result;
            }
            companion = null;
            Log.Message("GatheringWorker_DoublePawn : GatheringWorker - FindRandomGatheringOrganizer - return null; - 51", true);
            return null;
        }

        protected Pawn FindCompanion(Pawn organizer, GatheringDef gatheringDef)
        {
            var candidates = organizer.Map.mapPawns.SpawnedPawnsInFaction(organizer.Faction).Where(candidate => candidate != organizer && BasePawnValidator(candidate, gatheringDef)
                && MemberValidator(candidate) && PawnsCanGatherTogether(organizer, candidate));
            Log.Message("GatheringWorker_DoublePawn : GatheringWorker - FindCompanion - if (candidates.Any() && candidates.TryRandomElementByWeight(x => SortCandidatesBy(organizer, x), out var companion)) - 53", true);
            if (candidates.Any() && candidates.TryRandomElementByWeight(x => SortCandidatesBy(organizer, x), out var companion))
            {
                Log.Message("GatheringWorker_DoublePawn : GatheringWorker - FindCompanion - return companion; - 54", true);
                return companion;
            }
            return null;
        }

        protected virtual float SortCandidatesBy(Pawn organizer, Pawn candidate)
        {
            Log.Message("GatheringWorker_DoublePawn : GatheringWorker - SortCandidatesBy - return 0f; - 56", true);
            return 0f;
        }
        protected virtual LordJob CreateLordJobCustom(IntVec3 spot, Pawn organizer, Pawn companion)
        {
            Log.Message("GatheringWorker_DoublePawn : GatheringWorker - CreateLordJobCustom - return null; - 57", true);
            return null;
        }
        protected virtual bool MemberValidator(Pawn pawn)
        {
            Log.Message("GatheringWorker_DoublePawn : GatheringWorker - MemberValidator - return true; - 58", true);
            return true;
        }
        protected virtual bool PawnsCanGatherTogether(Pawn organizer, Pawn companion)
        {
            Log.Message("GatheringWorker_DoublePawn : GatheringWorker - PawnsCanGatherTogether - return true; - 59", true);
            return true;
        }
        protected virtual bool ConditionsMeet(Pawn organizer)
        {
            Log.Message("GatheringWorker_DoublePawn : GatheringWorker - ConditionsMeet - return true; - 60", true);
            return true;
        }
        protected override bool TryFindGatherSpot(Pawn organizer, out IntVec3 spot)
        {
            Log.Message("GatheringWorker_DoublePawn : GatheringWorker - TryFindGatherSpot - return RCellFinder.TryFindGatheringSpot_NewTemp(organizer, def, ignoreRequiredColonistCount: false, out spot); - 61", true);
            return RCellFinder.TryFindGatheringSpot_NewTemp(organizer, def, ignoreRequiredColonistCount: false, out spot);
        }

        protected override LordJob CreateLordJob(IntVec3 spot, Pawn organizer) // we don't use it, CreateLordJobCustom is used instead with custom arguments;
        {
            Log.Message("GatheringWorker_DoublePawn : GatheringWorker - TryFindGatherSpot - throw new NotImplementedException(); - 63", true);
            throw new NotImplementedException();
        }
    }
}