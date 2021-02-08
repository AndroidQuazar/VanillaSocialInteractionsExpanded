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
	[HarmonyPatch(typeof(Pawn), "SetFaction")]
	public static class SetFaction_Patch
	{
		private static void Prefix(Pawn __instance, Faction newFaction, Pawn recruiter = null)
		{
			if (newFaction != __instance.Faction && __instance.Faction.HostileTo(newFaction))
			{
				TaleRecorder.RecordTale(VSIE_DefOf.VSIE_WasPreviouslyOurEnemy, __instance);
			}
		}
	}
}
