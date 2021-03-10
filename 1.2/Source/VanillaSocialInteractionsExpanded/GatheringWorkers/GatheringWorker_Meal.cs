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
    public class GatheringWorker_Meal : GatheringWorker_Dating
    {
        protected override bool ConditionsMeet(Pawn organizer)
        {
            Log.Message("GatheringWorker_MovieNight : GatheringWorker_Dating - ConditionsMeet - return GenLocalDate.HourInteger(organizer.Map) >= 18 && GenLocalDate.HourInteger(organizer.Map) <= 24 && base.ConditionsMeet(organizer); - 1", true);
            return GenLocalDate.HourInteger(organizer.Map) >= 18 && GenLocalDate.HourInteger(organizer.Map) <= 24 && base.ConditionsMeet(organizer);
        }
        protected override bool PawnsCanGatherTogether(Pawn organizer, Pawn companion)
        {
            Log.Message("GatheringWorker_MovieNight : GatheringWorker_Dating - PawnsCanGatherTogether - return base.PawnsCanGatherTogether(organizer, companion) && TryFindTelevisionFor(organizer, companion, out Building television, out IntVec3 spot, out Building chair); - 2", true);
            return base.PawnsCanGatherTogether(organizer, companion) && TryFindTelevisionFor(organizer, companion, out Building television, out IntVec3 spot, out Building chair);
        }
        public static bool TryFindTelevisionFor(Pawn organizer, Pawn companion, out Building television, out IntVec3 spot, out Building chair)
        {
            Log.Message("GatheringWorker_MovieNight : GatheringWorker_Dating - TryFindTelevisionFor - var televisionDefs = DefDatabase<JoyGiverDef>.GetNamed(\"WatchTelevision\").thingDefs; - 3", true);
            var televisionDefs = DefDatabase<JoyGiverDef>.GetNamed("WatchTelevision").thingDefs;
            Log.Message("GatheringWorker_MovieNight : GatheringWorker_Dating - TryFindTelevisionFor - var televisions = organizer.Map.listerBuildings.allBuildingsColonist.Where(x => televisionDefs.Contains(x.def) && CanInteractWith(organizer, x) && CanInteractWith(companion, x)); - 4", true);
            var televisions = organizer.Map.listerBuildings.allBuildingsColonist.Where(x => televisionDefs.Contains(x.def) && CanInteractWith(organizer, x) && CanInteractWith(companion, x));
            Log.Message("GatheringWorker_MovieNight : GatheringWorker_Dating - TryFindTelevisionFor - foreach (var t in televisions) - 5", true);
            foreach (var t in televisions)
            {
                Log.Message("GatheringWorker_MovieNight : GatheringWorker_Dating - TryFindTelevisionFor - if (WatchBuildingUtilityDoublePawn.TryFindBestWatchCell(t, organizer, true, out IntVec3 result, out Building firstChair)) - 6", true);
                if (WatchBuildingUtilityDoublePawn.TryFindBestWatchCell(t, organizer, true, out IntVec3 result, out Building firstChair))
                {
                    Log.Message("GatheringWorker_MovieNight : GatheringWorker_Dating - TryFindTelevisionFor - if (WatchBuildingUtilityDoublePawn.TryFindBestWatchCellNear(t, companion, firstChair, true, out IntVec3 result2, out Building chair2)) - 7", true);
                    if (WatchBuildingUtilityDoublePawn.TryFindBestWatchCellNear(t, companion, firstChair, true, out IntVec3 result2, out Building chair2))
                    {
                        Log.Message("GatheringWorker_MovieNight : GatheringWorker_Dating - TryFindTelevisionFor - television = t; - 8", true);
                        television = t;
                        Log.Message("GatheringWorker_MovieNight : GatheringWorker_Dating - TryFindTelevisionFor - spot = result; - 9", true);
                        spot = result;
                        Log.Message("GatheringWorker_MovieNight : GatheringWorker_Dating - TryFindTelevisionFor - chair = firstChair; - 10", true);
                        chair = firstChair;
                        Log.Message("GatheringWorker_MovieNight : GatheringWorker_Dating - TryFindTelevisionFor - return true; - 11", true);
                        return true;
                    }
                    else
                    {
                        Log.Message("Failed to find a cell for companion: " + companion);
                    }
                }
                else
                {
                    Log.Message("Failed to find a television for organizer: " + organizer);
                }
            }
            television = null;
            Log.Message("GatheringWorker_MovieNight : GatheringWorker_Dating - TryFindTelevisionFor - spot = IntVec3.Invalid; - 15", true);
            spot = IntVec3.Invalid;
            Log.Message("GatheringWorker_MovieNight : GatheringWorker_Dating - TryFindTelevisionFor - chair = null; - 16", true);
            chair = null;
            Log.Message("GatheringWorker_MovieNight : GatheringWorker_Dating - TryFindTelevisionFor - return false; - 17", true);
            return false;
        }

        private static bool CanInteractWith(Pawn pawn, Thing t)
        {
            Log.Message("GatheringWorker_MovieNight : GatheringWorker_Dating - CanInteractWith - if (!pawn.CanReserve(t)) - 18", true);
            if (!pawn.CanReserve(t))
            {
                Log.Message("GatheringWorker_MovieNight : GatheringWorker_Dating - CanInteractWith - return false; - 19", true);
                return false;
            }
            Log.Message("GatheringWorker_MovieNight : GatheringWorker_Dating - CanInteractWith - if (t.IsForbidden(pawn)) - 20", true);
            if (t.IsForbidden(pawn))
            {
                Log.Message("GatheringWorker_MovieNight : GatheringWorker_Dating - CanInteractWith - return false; - 21", true);
                return false;
            }
            Log.Message("GatheringWorker_MovieNight : GatheringWorker_Dating - CanInteractWith - if (!t.IsSociallyProper(pawn)) - 22", true);
            if (!t.IsSociallyProper(pawn))
            {
                Log.Message("GatheringWorker_MovieNight : GatheringWorker_Dating - CanInteractWith - return false; - 23", true);
                return false;
            }
            Log.Message("GatheringWorker_MovieNight : GatheringWorker_Dating - CanInteractWith - if (!t.IsPoliticallyProper(pawn)) - 24", true);
            if (!t.IsPoliticallyProper(pawn))
            {
                Log.Message("GatheringWorker_MovieNight : GatheringWorker_Dating - CanInteractWith - return false; - 25", true);
                return false;
            }
            CompPowerTrader compPowerTrader = t.TryGetComp<CompPowerTrader>();
            Log.Message("GatheringWorker_MovieNight : GatheringWorker_Dating - CanInteractWith - if (compPowerTrader != null && !compPowerTrader.PowerOn) - 27", true);
            if (compPowerTrader != null && !compPowerTrader.PowerOn)
            {
                Log.Message("GatheringWorker_MovieNight : GatheringWorker_Dating - CanInteractWith - return false; - 28", true);
                return false;
            }
            return true;
        }
        protected override bool TryFindGatherSpot(Pawn organizer, out IntVec3 spot)
        {
            Log.Message("GatheringWorker_MovieNight : GatheringWorker_Dating - TryFindGatherSpot - spot = IntVec3.Invalid; - 30", true);
            spot = IntVec3.Invalid;
            Log.Message("GatheringWorker_MovieNight : GatheringWorker_Dating - TryFindGatherSpot - return true; - 31", true);
            return true;
        }

        protected override LordJob CreateLordJobCustom(IntVec3 spot, Pawn organizer, Pawn companion)
        {
            return new LordJob_Joinable_MovieNight(organizer, companion, spot, this.def, new IntRange(2 * GenDate.TicksPerHour, 4 * GenDate.TicksPerHour).RandomInRange);
        }
    }
}