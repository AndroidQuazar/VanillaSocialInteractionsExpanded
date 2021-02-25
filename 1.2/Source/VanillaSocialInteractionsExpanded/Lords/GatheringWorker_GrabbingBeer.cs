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
            if (organizer == null)
            {
                organizer = FindOrganizer(map);
            }
            if (organizer == null)
            {
                Log.Message("GatheringWorker_GrabbingBeer : GatheringWorker - CanExecute - return false; - 7", true);
                return false;
            }
            if (!TryFindGatherSpot(organizer, out IntVec3 _))
            {
                Log.Message("GatheringWorker_GrabbingBeer : GatheringWorker - CanExecute - return false; - 9", true);
                return false;
            }
            if (!GatheringsUtility.PawnCanStartOrContinueGathering(organizer))
            {
                Log.Message("GatheringWorker_GrabbingBeer : GatheringWorker - CanExecute - return false; - 11", true);
                return false;
            }
            if (organizer is null || FindCompanion(organizer) is null || !EnoughDrinks(organizer))
            {
                Log.Message("GatheringWorker_GrabbingBeer : GatheringWorker - CanExecute - return false; - 13", true);
                return false;
            }
            return true;
        }

        public override bool TryExecute(Map map, Pawn organizer = null)
        {
            Pawn companion = null;
            if (organizer == null)
            {
                organizer = FindOrganizerCustom(map, out companion);
            }

            if (organizer == null)
            {
                return false;
            }
            if (!TryFindGatherSpot(organizer, out IntVec3 spot))
            {
                Log.Message("GatheringWorker_GrabbingBeer : GatheringWorker - TryExecute - return false; - 20", true);
                return false;
            }
            if (!GatheringsUtility.PawnCanStartOrContinueGathering(organizer))
            {
                Log.Message("GatheringWorker_GrabbingBeer : GatheringWorker - CanExecute - return false; - 11", true);
                return false;
            }
            if (organizer is null || companion is null || !EnoughDrinks(organizer))
            {
                Log.Message("GatheringWorker_GrabbingBeer : GatheringWorker - CanExecute - return false; - 13", true);
                return false;
            }

            LordJob lordJob = CreateLordJobCustom(spot, organizer, companion);
            List<Pawn> pawns = new List<Pawn> { organizer, companion };
            var lord = LordMaker.MakeNewLord(organizer.Faction, lordJob, organizer.Map, pawns);
            Log.Message("Making new lord: " + lord + " - lordJob: " + lord.LordJob);
            SendLetter(spot, organizer);
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

        public Pawn FindRandomGatheringOrganizer(Faction faction, Map map, GatheringDef gatheringDef, out Pawn companion)
        {
            Predicate<Pawn> v = (Pawn organizer) => organizer.RaceProps.Humanlike && !organizer.InBed() && !organizer.InMentalState && organizer.GetLord() == null 
            && GatheringsUtility.ShouldPawnKeepGathering(organizer, gatheringDef) && !organizer.Drafted && (gatheringDef.requiredTitleAny == null || gatheringDef.requiredTitleAny.Count == 0 
            || (organizer.royalty != null && organizer.royalty.AllTitlesInEffectForReading.Any((RoyalTitle t) => gatheringDef.requiredTitleAny.Contains(t.def)))) 
            && organizer.health.hediffSet.hediffs.Any(hediff => hediff is Hediff_Alcohol)
            && organizer.Map.mapPawns.SpawnedPawnsInFaction(organizer.Faction).Any(companionCandidate => companionCandidate != organizer && companionCandidate.RaceProps.Humanlike
            && !VSIE_Utils.workTags.Contains(companionCandidate.mindState.lastJobTag) && companionCandidate.relations.OpinionOf(organizer) >= 20 && organizer.relations.OpinionOf(companionCandidate) >= 20
            && !companionCandidate.health.hediffSet.hediffs.Any(hediff => hediff is Hediff_Alcohol));
            
            if ((from x in map.mapPawns.SpawnedPawnsInFaction(faction)
                 where v(x)
                 select x).TryRandomElement(out Pawn result))
            {
                companion = FindCompanion(result);
                return result;
            }
            companion = null;
            return null;
        }

        private Pawn FindCompanion(Pawn organizer)
        {

            var candidates = organizer.Map.mapPawns.SpawnedPawnsInFaction(organizer.Faction).Where(x => x != organizer && x.RaceProps.Humanlike 
            && !VSIE_Utils.workTags.Contains(x.mindState.lastJobTag) && x.relations.OpinionOf(organizer) >= 20 && organizer.relations.OpinionOf(x) >= 20
            && !x.health.hediffSet.hediffs.Any(y => y is Hediff_Alcohol));
            if (candidates.Any() && candidates.TryRandomElementByWeight(x => organizer.relations.OpinionOf(x), out var companion))
            {
                Log.Message("GatheringWorker_GrabbingBeer : GatheringWorker - FindCompanion - return candidates.RandomElementByWeight(x => organizer.relations.OpinionOf(x)); - 30", true);
                return companion;
            }
            Log.Message("Companion is not found for alcohol drinking");
            return null;
        }
        protected LordJob CreateLordJobCustom(IntVec3 spot, Pawn organizer, Pawn companion)
        {
            return new LordJob_Joinable_GrabbingBeer(organizer, companion, spot, this.def, new IntRange(4 * GenDate.TicksPerHour, 6 * GenDate.TicksPerHour).RandomInRange);
        }

        protected override bool TryFindGatherSpot(Pawn organizer, out IntVec3 spot)
        {
            Log.Message("GatheringWorker_GrabbingBeer : GatheringWorker - TryFindGatherSpot - return RCellFinder.TryFindGatheringSpot_NewTemp(organizer, def, ignoreRequiredColonistCount: false, out spot); - 36", true);
            return RCellFinder.TryFindGatheringSpot_NewTemp(organizer, def, ignoreRequiredColonistCount: false, out spot);
        }

        protected override LordJob CreateLordJob(IntVec3 spot, Pawn organizer) // we don't use it, CreateLordJobCustom is used instead with custom arguments;
        {
            throw new NotImplementedException();
        }
    }
}