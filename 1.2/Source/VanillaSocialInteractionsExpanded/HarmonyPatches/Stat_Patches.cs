using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using Verse.AI;

namespace VanillaSocialInteractionsExpanded
{
	[HarmonyPatch(typeof(StatExtension), nameof(StatExtension.GetStatValue))]
	public static class GetStatValue_Patch
    {
		private static void Postfix(Thing thing, StatDef stat, bool applyPostProcess, ref float __result)
        {
			if (stat == StatDefOf.ResearchSpeed && thing is Pawn pawn && pawn.InspirationDef == VSIE_DefOf.VSIE_Inspired_Research)
            {
				__result *= 5;
            }
            else if (stat == StatDefOf.WorkSpeedGlobal && thing is Pawn pawn2)
            {
                var pawnCurJob = pawn2.jobs?.curJob?.def;
                if (pawn2.Map != null && pawnCurJob != null && pawn2.Faction != null)
                {
                    var pawnPosition = pawn2.Position;
                    var pawns = pawn2.Map.mapPawns.SpawnedPawnsInFaction(pawn2.Faction);
                    var nearestPawnCount = 0;
                    var totalPawnCount = pawns.Count;
                    for (var i = 0; i < totalPawnCount; i++)
                    {
                        var x = pawns[i];
                        if (x != pawn2 && x.jobs?.curJob?.def == pawnCurJob && x.RaceProps.Humanlike && x.Position.DistanceTo(pawnPosition) < 10)
                        {
                            nearestPawnCount++;
                        }
                    }

                    if (nearestPawnCount > 0)
                    {
                        var bonus = nearestPawnCount * 0.1f;
                        var oldResult = __result;
                        __result += bonus;
                        //Log.Message($"{pawn2} gets bonus to work due to other pawns in the same room: {pawns.Count()}, bonus: {bonus}, old result: {oldResult}, new result: {__result}");
                    }
                }
            }
        }
    }
}
