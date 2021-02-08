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
	[HarmonyPatch(typeof(MemoryThoughtHandler), "TryGainMemory", new Type[]
	{
		typeof(Thought_Memory),
		typeof(Pawn)
	})]
	public static class TryGainMemory_Patch
	{
		private static void Postfix(MemoryThoughtHandler __instance, Thought_Memory newThought, Pawn otherPawn)
		{
			if (newThought.def == ThoughtDefOf.RebuffedMyRomanceAttempt)
            {
				TaleRecorder.RecordTale(VSIE_DefOf.VSIE_RebuffedMe, otherPawn, __instance.pawn);
            }
		}
	}
}
