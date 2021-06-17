using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;
using Verse.AI;
using Verse.Grammar;

namespace VanillaSocialInteractionsExpanded
{
    public class JobGiver_BuildSnowman : ThinkNode_JobGiver
    {
        public static bool CanBuildSnowman(Pawn pawn, out IntVec3 cell)
        {
            cell = IntVec3.Invalid;
            if (pawn.WorkTypeIsDisabled(WorkTypeDefOf.Construction))
            {
                return false;
            }
            if (!JoyUtility.EnjoyableOutsideNow(pawn))
            {
                return false;
            }

            if (pawn.Map.snowGrid.TotalDepth < 200f)
            {
                return false;
            }

            cell = TryFindSnowmanBuildCell(pawn);
            if (!cell.IsValid)
            {
                return false;
            }
            return true;
        }
        protected override Job TryGiveJob(Pawn pawn)
        {
            if (CanBuildSnowman(pawn, out IntVec3 c))
            {
                return JobMaker.MakeJob(VSIE_DefOf.BuildSnowman, c);
            }
            return null;
        }

        private static IntVec3 TryFindSnowmanBuildCell(Pawn pawn)
        {
            if (!CellFinder.TryFindClosestRegionWith(pawn.GetRegion(), TraverseParms.For(pawn), (Region r) => r.Room.PsychologicallyOutdoors, 100, out Region rootReg))
            {
                return IntVec3.Invalid;
            }
            IntVec3 result = IntVec3.Invalid;
            RegionTraverser.BreadthFirstTraverse(rootReg, (Region from, Region r) => r.Room == rootReg.Room, delegate (Region r)
            {
                for (int i = 0; i < 5; i++)
                {
                    IntVec3 randomCell = r.RandomCell;
                    if (IsGoodSnowmanCell(randomCell, pawn))
                    {
                        result = randomCell;
                        return true;
                    }
                }
                return false;
            }, 30);
            return result;
        }

        private static bool IsGoodSnowmanCell(IntVec3 c, Pawn pawn)
        {
            if (pawn.Map.snowGrid.GetDepth(c) < 0.5f)
            {
                return false;
            }
            if (c.IsForbidden(pawn))
            {
                return false;
            }
            if (c.GetEdifice(pawn.Map) != null)
            {
                return false;
            }
            for (int i = 0; i < 9; i++)
            {
                IntVec3 c2 = c + GenAdj.AdjacentCellsAndInside[i];
                if (!c2.InBounds(pawn.Map))
                {
                    return false;
                }
                if (!c2.Standable(pawn.Map))
                {
                    return false;
                }
                if (pawn.Map.reservationManager.IsReservedAndRespected(c2, pawn))
                {
                    return false;
                }
            }
            List<Thing> list = pawn.Map.listerThings.ThingsOfDef(ThingDefOf.Snowman);
            for (int j = 0; j < list.Count; j++)
            {
                if (list[j].Position.InHorDistOf(c, 12f))
                {
                    return false;
                }
            }
            return true;
        }
    }
}