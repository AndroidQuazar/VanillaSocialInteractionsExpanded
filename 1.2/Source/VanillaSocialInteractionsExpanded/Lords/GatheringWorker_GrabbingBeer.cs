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
    public class GatheringWorker_GrabbingBeer : GatheringWorker
    {
        private bool EnoughDrinks(Pawn organizer)
        {
            Log.Message("GatheringWorker_GrabbingBeer : GatheringWorker - EnoughDrinks - if (organizer.Map.listerThings.ThingsInGroup(ThingRequestGroup.Drug).Any(x => VSIE_Utils.DrugValidator(organizer, x))) - 1", true);
            if (organizer.Map.listerThings.ThingsInGroup(ThingRequestGroup.Drug).Any(x => VSIE_Utils.DrugValidator(organizer, x)))
            {
                Log.Message("GatheringWorker_GrabbingBeer : GatheringWorker - EnoughDrinks - return true; - 2", true);
                return true;
            }
            return false;
        }
        public override bool CanExecute(Map map, Pawn organizer = null)
        {
            Log.Message("GatheringWorker_GrabbingBeer : GatheringWorker - CanExecute - if (organizer == null) - 4", true);
            if (organizer == null)
            {
                organizer = FindOrganizerCustom(map, out var companion);
                if (organizer is null || companion is null)
                {
                    Log.Message("GatheringWorker_GrabbingBeer : GatheringWorker - CanExecute - return false; - 7", true);
                    return false;
                }
            }
            if (organizer == null)
            {
                Log.Message("GatheringWorker_GrabbingBeer : GatheringWorker - CanExecute - return false; - 9", true);
                return false;
            }
            if (!TryFindGatherSpot(organizer, out IntVec3 _))
            {
                Log.Message("GatheringWorker_GrabbingBeer : GatheringWorker - CanExecute - return false; - 11", true);
                return false;
            }
            if (!GatheringsUtility.PawnCanStartOrContinueGathering(organizer))
            {
                Log.Message("GatheringWorker_GrabbingBeer : GatheringWorker - CanExecute - return false; - 13", true);
                return false;
            }
            if (organizer is null || FindCompanion(organizer, this.def) is null || !EnoughDrinks(organizer))
            {
                Log.Message("GatheringWorker_GrabbingBeer : GatheringWorker - CanExecute - return false; - 15", true);
                return false;
            }

            return true;
        }

        public override bool TryExecute(Map map, Pawn organizer = null)
        {
            Log.Message("GatheringWorker_GrabbingBeer : GatheringWorker - TryExecute - Pawn companion = null; - 17", true);
            Pawn companion = null;
            Log.Message("GatheringWorker_GrabbingBeer : GatheringWorker - TryExecute - if (organizer == null) - 18", true);
            if (organizer == null)
            {
                Log.Message("GatheringWorker_GrabbingBeer : GatheringWorker - TryExecute - organizer = FindOrganizerCustom(map, out companion); - 19", true);
                organizer = FindOrganizerCustom(map, out companion);
            }

            Log.Message("GatheringWorker_GrabbingBeer : GatheringWorker - TryExecute - if (organizer == null) - 20", true);
            if (organizer == null)
            {
                Log.Message("GatheringWorker_GrabbingBeer : GatheringWorker - TryExecute - return false; - 21", true);
                return false;
            }
            Log.Message("GatheringWorker_GrabbingBeer : GatheringWorker - TryExecute - if (!TryFindGatherSpot(organizer, out IntVec3 spot)) - 22", true);
            if (!TryFindGatherSpot(organizer, out IntVec3 spot))
            {
                Log.Message("GatheringWorker_GrabbingBeer : GatheringWorker - TryExecute - return false; - 23", true);
                return false;
            }
            Log.Message("GatheringWorker_GrabbingBeer : GatheringWorker - TryExecute - if (!GatheringsUtility.PawnCanStartOrContinueGathering(organizer)) - 24", true);
            if (!GatheringsUtility.PawnCanStartOrContinueGathering(organizer))
            {
                Log.Message("GatheringWorker_GrabbingBeer : GatheringWorker - TryExecute - return false; - 25", true);
                return false;
            }
            Log.Message("GatheringWorker_GrabbingBeer : GatheringWorker - TryExecute - if (organizer is null || companion is null || !EnoughDrinks(organizer)) - 26", true);
            if (organizer is null || companion is null || !EnoughDrinks(organizer))
            {
                Log.Message("GatheringWorker_GrabbingBeer : GatheringWorker - TryExecute - return false; - 27", true);
                return false;
            }

            LordJob lordJob = CreateLordJobCustom(spot, organizer, companion);
            Log.Message("GatheringWorker_GrabbingBeer : GatheringWorker - TryExecute - List<Pawn> pawns = new List<Pawn> { organizer, companion }; - 29", true);
            List<Pawn> pawns = new List<Pawn> { organizer, companion };
            Log.Message("GatheringWorker_GrabbingBeer : GatheringWorker - TryExecute - var lord = LordMaker.MakeNewLord(organizer.Faction, lordJob, organizer.Map, pawns); - 30", true);
            var lord = LordMaker.MakeNewLord(organizer.Faction, lordJob, organizer.Map, pawns);
            Log.Message("Making new lord: " + lord + " - lordJob: " + lord.LordJob);
            Log.Message("GatheringWorker_GrabbingBeer : GatheringWorker - TryExecute - SendLetter(spot, organizer); - 32", true);
            SendLetter(spot, organizer);
            Log.Message("GatheringWorker_GrabbingBeer : GatheringWorker - TryExecute - return true; - 33", true);
            return true;
        }
        private Pawn FindOrganizerCustom(Map map, out Pawn companion)
        {

            var organizer = FindRandomGatheringOrganizer(Faction.OfPlayer, map, def, out companion);
            if (organizer is null)
            {
                companion = null;
                return null;
            }
            return organizer;
        }

        private bool BasePawnValidator(Pawn pawn, GatheringDef gatheringDef)
        {
            var value = pawn.RaceProps.Humanlike && !pawn.InBed() && !pawn.InMentalState && pawn.GetLord() == null
            && GatheringsUtility.ShouldPawnKeepGathering(pawn, gatheringDef) && !pawn.Drafted && (gatheringDef.requiredTitleAny == null || gatheringDef.requiredTitleAny.Count == 0
            || (pawn.royalty != null && pawn.royalty.AllTitlesInEffectForReading.Any((RoyalTitle t) => gatheringDef.requiredTitleAny.Contains(t.def))));
            Log.Message($"{pawn} - basePawnValidator: " + value);
            return value;
        }
        public Pawn FindRandomGatheringOrganizer(Faction faction, Map map, GatheringDef gatheringDef, out Pawn companion)
        {
            Predicate<Pawn> v = (Pawn organizer) => BasePawnValidator(organizer, gatheringDef)
                && !organizer.health.hediffSet.hediffs.Any(hediff => hediff is Hediff_Alcohol) && FindCompanion(organizer, gatheringDef) != null;

            if (map.mapPawns.SpawnedPawnsInFaction(faction).Where(x => v(x)).TryRandomElement(out Pawn result))
            {
                companion = FindCompanion(result, gatheringDef);
                Log.Message($"Organizer: {result}, companion: {companion}");
                return result;
            }
            companion = null;
            return null;
        }

        private Pawn FindCompanion(Pawn organizer, GatheringDef gatheringDef)
        {
            Log.Message("GatheringWorker_GrabbingBeer : GatheringWorker - FindCompanion - var candidates = organizer.Map.mapPawns.SpawnedPawnsInFaction(organizer.Faction).Where(x => x != organizer && x.RaceProps.Humanlike); - 45", true);
            var candidates = organizer.Map.mapPawns.SpawnedPawnsInFaction(organizer.Faction).Where(x => x != organizer && BasePawnValidator(x, gatheringDef)
            && !VSIE_Utils.workTags.Contains(x.mindState.lastJobTag) && x.relations.OpinionOf(organizer) >= 20 && organizer.relations.OpinionOf(x) >= 20);
            //&& !x.health.hediffSet.hediffs.Any(y => y is Hediff_Alcohol));
            Log.Message("GatheringWorker_GrabbingBeer : GatheringWorker - FindCompanion - if (candidates.Any() && candidates.TryRandomElementByWeight(x => organizer.relations.OpinionOf(x), out var companion)) - 47", true);
            if (candidates.Any() && candidates.TryRandomElementByWeight(x => organizer.relations.OpinionOf(x), out var companion))
            {
                Log.Message("GatheringWorker_GrabbingBeer : GatheringWorker - FindCompanion - return companion; - 48", true);
                return companion;
            }
            Log.Message("Companion is not found for alcohol drinking");
            Log.Message("GatheringWorker_GrabbingBeer : GatheringWorker - FindCompanion - return null; - 50", true);
            return null;
        }
        protected LordJob CreateLordJobCustom(IntVec3 spot, Pawn organizer, Pawn companion)
        {
            return new LordJob_Joinable_GrabbingBeer(organizer, companion, spot, this.def, new IntRange(4 * GenDate.TicksPerHour, 6 * GenDate.TicksPerHour).RandomInRange);
        }

        protected override bool TryFindGatherSpot(Pawn organizer, out IntVec3 spot)
        {
            Log.Message("GatheringWorker_GrabbingBeer : GatheringWorker - TryFindGatherSpot - return RCellFinder.TryFindGatheringSpot_NewTemp(organizer, def, ignoreRequiredColonistCount: false, out spot); - 52", true);
            return RCellFinder.TryFindGatheringSpot_NewTemp(organizer, def, ignoreRequiredColonistCount: false, out spot);
        }

        protected override LordJob CreateLordJob(IntVec3 spot, Pawn organizer) // we don't use it, CreateLordJobCustom is used instead with custom arguments;
        {
            Log.Message("GatheringWorker_GrabbingBeer : GatheringWorker - TryFindGatherSpot - throw new NotImplementedException(); - 54", true);
            throw new NotImplementedException();
        }
    }
}