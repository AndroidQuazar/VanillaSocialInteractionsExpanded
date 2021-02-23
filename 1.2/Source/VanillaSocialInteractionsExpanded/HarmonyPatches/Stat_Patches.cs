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
                var pawns = pawn2.Map.mapPawns.SpawnedPawnsInFaction(pawn2.Faction).Where(x => x != pawn2 && x.RaceProps.Humanlike && x.CurJobDef == pawn2.CurJobDef && x.Position.DistanceTo(pawn2.Position) < 10);
                var pawnCount = pawns.Count();
                if (pawnCount > 0)
                {
                    var bonus = pawnCount * 0.1f;
                    var oldResult = __result;
                    __result += bonus;
                    //Log.Message($"{pawn2} gets bonus to work due to other pawns in the same room: {pawns.Count()}, bonus: {bonus}, old result: {oldResult}, new result: {__result}");
                }
            }
        }
    }
}
