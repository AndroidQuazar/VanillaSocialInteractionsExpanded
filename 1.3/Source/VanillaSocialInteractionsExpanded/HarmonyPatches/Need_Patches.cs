using HarmonyLib;
using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using Verse;
using Verse.AI;

namespace VanillaSocialInteractionsExpanded
{
	[HarmonyPatch(typeof(Need_Joy), "GainJoy")]
	public static class GainJoy_Patch
	{
		private static void Prefix(Pawn ___pawn, ref float amount, JoyKindDef joyKind)
        {
			if (___pawn.InspirationDef == VSIE_DefOf.VSIE_Party_Frenzy && amount > 0)
            {
				amount *= 2f;
            }
		}
	}
}
