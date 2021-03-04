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
    public class GatheringWorker_Skygazing : GatheringWorker_Dating
    {
        protected override bool ConditionsMeet(Pawn organizer)
        {
            float num = AggregateSkyGazeChanceFactor(organizer.Map.gameConditionManager, organizer.Map);
            if (num < 1)
            {
                return false;
            }
            if (!JoyUtility.EnjoyableOutsideNow(organizer) || organizer.Map.weatherManager.curWeather.rainRate > 0.1f)
            {
                return false;
            }
            if (!RCellFinder.TryFindSkygazeCell(organizer.Position, organizer, out IntVec3 result))
            {
                return false;
            }
            return base.ConditionsMeet(organizer);
        }
        public static float AggregateSkyGazeChanceFactor(GameConditionManager manager, Map map)
        {
            float num = 1f;
            for (int i = 0; i < manager.ActiveConditions.Count; i++)
            {
                num *= manager.ActiveConditions[i].SkyGazeChanceFactor(map);
            }
            return num;
        }
        protected override bool TryFindGatherSpot(Pawn organizer, out IntVec3 spot)
        {
            if (RCellFinder.TryFindSkygazeCell(organizer.Position, organizer, out spot))
            {
                return true;
            }
            spot = IntVec3.Invalid;
            return false;
        }
        protected override LordJob CreateLordJobCustom(IntVec3 spot, Pawn organizer, Pawn companion)
        {
            return new LordJob_Joinable_Skygazing(organizer, companion, spot, this.def, new IntRange(2 * GenDate.TicksPerHour, 4 * GenDate.TicksPerHour).RandomInRange);
        }
    }
}