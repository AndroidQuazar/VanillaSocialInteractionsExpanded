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
			if (__instance.IsWildMan() && recruiter != null)
            {
				TaleRecorder.RecordTale(VSIE_DefOf.VSIE_TamedMe, recruiter, __instance);
            }
		}
	}

	[HarmonyPatch(typeof(Pawn), "Kill")]
	public static class Pawn_Kill_Patch
	{
		private static void Prefix(Pawn __instance)
		{
			if (CheckSurgeryFail_Patch._patient != null)
            {
				Log.Message($"{CheckSurgeryFail_Patch._patient}, {CheckSurgeryFail_Patch._surgeon}, __instance == CheckSurgeryFail_Patch._patient: {__instance == CheckSurgeryFail_Patch._patient} " +
				$"&& CheckSurgeryFail_Patch._surgeon.IsColonist: {CheckSurgeryFail_Patch._surgeon.IsColonist} && __instance.IsColonist: {__instance.IsColonist}");
			}
			else
            {
				Log.Message("Killing: " + __instance);
            }
			if (__instance == CheckSurgeryFail_Patch._patient && CheckSurgeryFail_Patch._surgeon.IsColonist && __instance.IsColonist)
			{
				TaleRecorder.RecordTale(VSIE_DefOf.VSIE_FailedMedicalOperationAndKilled, CheckSurgeryFail_Patch._surgeon);
				CheckSurgeryFail_Patch._patient = null;
				CheckSurgeryFail_Patch._surgeon = null;
			}
		}
	}
}
