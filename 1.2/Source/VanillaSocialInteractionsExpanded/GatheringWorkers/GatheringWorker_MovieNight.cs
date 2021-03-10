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
    public class GatheringWorker_MovieNight : GatheringWorker_Dating
    {
        protected override bool ConditionsMeet(Pawn organizer)
        {
            return GenLocalDate.HourInteger(organizer.Map) >= 18 && GenLocalDate.HourInteger(organizer.Map) <= 24 && base.ConditionsMeet(organizer);
        }
        protected override bool PawnsCanGatherTogether(Pawn organizer, Pawn companion)
        {
            return base.PawnsCanGatherTogether(organizer, companion) && TryFindTelevisionFor(organizer, companion, out Building television, out IntVec3 spot, out Building chair);
        }
        public static bool TryFindTelevisionFor(Pawn organizer, Pawn companion, out Building television, out IntVec3 spot, out Building chair)
        {
            var televisionDefs = DefDatabase<JoyGiverDef>.GetNamed("WatchTelevision").thingDefs;
            var televisions = organizer.Map.listerBuildings.allBuildingsColonist.Where(x => televisionDefs.Contains(x.def) && CanInteractWith(organizer, x) && CanInteractWith(companion, x));
            foreach (var t in televisions)
            {
                if (WatchBuildingUtilityDoublePawn.TryFindBestWatchCell(t, organizer, true, out IntVec3 result, out Building firstChair))
                {
                    if (WatchBuildingUtilityDoublePawn.TryFindBestWatchCellNear(t, companion, firstChair, true, out IntVec3 result2, out Building chair2))
                    {
                        television = t;
                        spot = result;
                        chair = firstChair;
                        return true;
                    }
                    else
                    {
                    }
                }
                else
                {
                }
            }
            television = null;
            spot = IntVec3.Invalid;
            chair = null;
            return false;
        }

        private static bool CanInteractWith(Pawn pawn, Thing t)
        {
            if (!pawn.CanReserve(t))
            {
                return false;
            }
            if (t.IsForbidden(pawn))
            {
                return false;
            }
            if (!t.IsSociallyProper(pawn))
            {
                return false;
            }
            if (!t.IsPoliticallyProper(pawn))
            {
                return false;
            }
            CompPowerTrader compPowerTrader = t.TryGetComp<CompPowerTrader>();
            if (compPowerTrader != null && !compPowerTrader.PowerOn)
            {
                return false;
            }
            return true;
        }
        protected override bool TryFindGatherSpot(Pawn organizer, out IntVec3 spot)
        {
            spot = IntVec3.Invalid;
            return true;
        }

        protected override LordJob CreateLordJobCustom(IntVec3 spot, Pawn organizer, Pawn companion)
        {
            return new LordJob_Joinable_MovieNight(organizer, companion, spot, this.def, new IntRange(2 * GenDate.TicksPerHour, 4 * GenDate.TicksPerHour).RandomInRange);
        }
    }
}