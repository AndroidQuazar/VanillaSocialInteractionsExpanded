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
	[HarmonyPatch(typeof(Pawn), "SpawnSetup")]
	public static class SpawnSetup_Patch
	{
		private static void Postfix(Pawn __instance, Map map, bool respawningAfterLoad)
		{
			if (!respawningAfterLoad)
            {
				var pawnAge = __instance.ageTracker.AgeChronologicalYearsFloat;
				foreach (var relPawn in __instance.relations.PotentiallyRelatedPawns)
				{
					var relPawnAge = relPawn.ageTracker.AgeChronologicalYearsFloat;
					if (__instance.relations.OpinionOf(relPawn) >= 30 && relPawn.relations.OpinionOf(__instance) >= 30 && new FloatRange(-5f, 5f).Includes(pawnAge - relPawnAge))
					{
						if (Rand.ChanceSeeded(0.1f, __instance.thingIDNumber))
						{
							TaleRecorder.RecordTale(VSIE_DefOf.VSIE_HasBeenMyFriendSinceChildhood, __instance, relPawn);
							TaleRecorder.RecordTale(VSIE_DefOf.VSIE_HasBeenMyFriendSinceChildhood, relPawn, __instance);
						}
					}
				}
			}
		}
	}

	[HarmonyPatch(typeof(Pawn), "SetFaction")]
	public static class SetFaction_Patch
	{
		private static void Prefix(Pawn __instance, Faction newFaction, Pawn recruiter = null)
		{
			if (newFaction != __instance.Faction && __instance.Faction.HostileTo(newFaction))
			{
				if (Rand.Chance(0.1f))
				{
					TaleRecorder.RecordTale(VSIE_DefOf.VSIE_WasPreviouslyOurEnemy, __instance);
				}
			}
			if (__instance.IsWildMan() && recruiter != null)
            {
				if (Rand.Chance(0.1f))
				{
					TaleRecorder.RecordTale(VSIE_DefOf.VSIE_TamedMe, __instance, recruiter);
				}
			}
		}
	}

	[HarmonyPatch(typeof(Pawn), "Kill")]
	public static class Pawn_Kill_Patch
	{
		private static void Prefix(Pawn __instance, DamageInfo? dinfo, Hediff exactCulprit = null)
		{
			//if (CheckSurgeryFail_Patch._patient != null)
            //{
			//	Log.Message($"{CheckSurgeryFail_Patch._patient}, {CheckSurgeryFail_Patch._surgeon}, __instance == CheckSurgeryFail_Patch._patient: {__instance == CheckSurgeryFail_Patch._patient} " +
			//	$"&& CheckSurgeryFail_Patch._surgeon.IsColonist: {CheckSurgeryFail_Patch._surgeon.IsColonist} && __instance.IsColonist: {__instance.IsColonist}");
			//}
			//else
            //{
			//	Log.Message("Killing: " + __instance);
            //}
			if (__instance == CheckSurgeryFail_Patch._patient && CheckSurgeryFail_Patch._surgeon.IsColonist && __instance.IsColonist)
			{
				if (Rand.Chance(0.1f))
				{
					TaleRecorder.RecordTale(VSIE_DefOf.VSIE_FailedMedicalOperationAndKilled, CheckSurgeryFail_Patch._surgeon);
				}
				CheckSurgeryFail_Patch._patient = null;
				CheckSurgeryFail_Patch._surgeon = null;
			}

			if (dinfo.HasValue)
			{
				TryRecordSavedMeFromRaiders(__instance, dinfo);
				TryRecordMeleeAspiration(dinfo);
			}

			if (__instance.RaceProps.Animal && __instance.Faction is null && dinfo.Value.Instigator is Pawn killer && killer.InspirationDef == VSIE_DefOf.Frenzy_Shoot)
            {
				VSIE_Utils.SocialInteractionsManager.Notify_AspirationProgress(killer);
			}
		}

		public static void TryRecordSavedMeFromRaiders(Pawn assaulter, DamageInfo? dinfo)
        {
			Pawn target = assaulter.mindState.enemyTarget as Pawn;
			if (target != null && target?.mindState?.meleeThreat == assaulter && dinfo.Value.Instigator != null && dinfo.Value.Instigator is Pawn saviour && target != saviour)
			{
				if (Rand.Chance(0.1f))
				{
					TaleRecorder.RecordTale(VSIE_DefOf.VSIE_SavedMeFromRaiders, target, saviour, assaulter);
				}
			}
		}

		public static void TryRecordMeleeAspiration(DamageInfo? dinfo)
        {
			if (dinfo.Value.Instigator != null && dinfo.Value.Instigator is Pawn assaulter && assaulter.InspirationDef == VSIE_DefOf.VSIE_Melee_Frenzy 
				&& (dinfo.Value.Weapon == null || dinfo.Value.Weapon == ThingDefOf.Human || dinfo.Value.Weapon.IsMeleeWeapon))
            {
				VSIE_Utils.SocialInteractionsManager.Notify_AspirationProgress(assaulter);
            }
		}
	}
}
