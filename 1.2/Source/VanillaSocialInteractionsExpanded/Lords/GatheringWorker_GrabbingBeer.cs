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
            if (organizer == null)
            {
                organizer = FindOrganizer(map);
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
            var companion = FindCompanion(organizer);
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
        protected override Pawn FindOrganizer(Map map)
        {
            var organizer = GatheringsUtility.FindRandomGatheringOrganizer(Faction.OfPlayer, map, def);
            Log.Message($"organizer: {organizer}");
            return organizer;
        }

        private Pawn FindCompanion(Pawn organizer)
        {
            var candidates = organizer.Map.mapPawns.SpawnedPawnsInFaction(organizer.Faction).Where(x => x != organizer && x.RaceProps.Humanlike); ;//.Where(x => !VSIE_Utils.workTags.Contains(x.mindState.lastJobTag) && x.relations.OpinionOf(organizer) >= 20 && organizer.relations.OpinionOf(x) >= 20);
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