using HarmonyLib;
using RimWorld;
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
	[HarmonyPatch(typeof(Pawn), "SpawnSetup")]
	public static class SpawnSetup_Patch
	{
		private static void Postfix(Pawn __instance, Map map, bool respawningAfterLoad)
		{
			if (VanillaSocialInteractionsExpandedSettings.EnableMemories)
			{
				if (!respawningAfterLoad && __instance.ageTracker != null && __instance.relations != null)
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
				if (__instance.Faction == Faction.OfPlayer)
				{
					VSIE_Utils.TryRegisterNewColonist(__instance, __instance.Faction);
				}
			}
		}
	}

	[HarmonyPatch(typeof(Pawn_AgeTracker), "BirthdayBiological")]
	public static class BirthdayBiological_Patch
	{
		private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
		{
			var codes = instructions.ToList();
			bool found = false;
			for (var i = 0; i < codes.Count; i++)
			{
				if (!found && codes[i].Is(OpCodes.Ldstr, "BirthdayBiologicalAgeInjuries"))
				{
					found = true;
					yield return new CodeInstruction(OpCodes.Ldarg_0, null);
					yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(BirthdayBiological_Patch), "OldAgeBirthday", null, null));
					yield return codes[i];
				}
				else
				{
					yield return codes[i];
				}
			}
			yield break;
		}

		private static void OldAgeBirthday(Pawn_AgeTracker __instance)
		{
			var pawn = Traverse.Create(__instance).Field("pawn").GetValue<Pawn>();
			if (pawn.RaceProps.Humanlike)
            {
				if (Rand.Chance(0.1f))
				{
					if (VanillaSocialInteractionsExpandedSettings.EnableObtainingNewTraits)
					{
						VSIE_Utils.TryDevelopNewTrait(pawn, "VSIE.BirthdayEvent");
					}
				}
				if (pawn.Faction.IsPlayer)
                {
					VSIE_Utils.SocialInteractionsManager.birthdays[pawn] = Find.TickManager.TicksGame;
				}
			}
		}
	}

	[HarmonyPatch(typeof(Pawn), "SetFaction")]
	public static class SetFaction_Patch
	{
		private static void Prefix(Pawn __instance, Faction newFaction, Pawn recruiter = null)
		{
			if (VanillaSocialInteractionsExpandedSettings.EnableMemories)
			{
				if (GenTicks.TicksAbs > 0)
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
					if (newFaction == Faction.OfPlayer)
                    {
						VSIE_Utils.TryRegisterNewColonist(__instance, newFaction);
                    }
				}
			}
		}
	}

	[HarmonyPatch(typeof(Pawn), "Kill")]
	public static class Pawn_Kill_Patch
	{
		private static void Prefix(Pawn __instance, DamageInfo? dinfo, Hediff exactCulprit = null)
		{
			if (__instance == CheckSurgeryFail_Patch._patient && CheckSurgeryFail_Patch._surgeon.IsColonist && __instance.IsColonist)
			{
				if (VanillaSocialInteractionsExpandedSettings.EnableMemories)
				{
					if (Rand.Chance(0.1f))
					{
						TaleRecorder.RecordTale(VSIE_DefOf.VSIE_FailedMedicalOperationAndKilled, CheckSurgeryFail_Patch._surgeon);
					}
				}
				CheckSurgeryFail_Patch._patient = null;
				CheckSurgeryFail_Patch._surgeon = null;
			}

			if (dinfo.HasValue)
			{
				if (VanillaSocialInteractionsExpandedSettings.EnableMemories)
				{
					TryRecordSavedMeFromRaiders(__instance, dinfo);
				}
				if (VanillaSocialInteractionsExpandedSettings.EnableAspirations)
				{
					TryRecordMeleeAspiration(dinfo);
				}
			}

			if (VanillaSocialInteractionsExpandedSettings.EnableAspirations)
			{
				if (dinfo.HasValue && __instance.RaceProps.Animal && __instance.Faction is null && dinfo.Value.Instigator is Pawn killer && killer.InspirationDef == VSIE_DefOf.Frenzy_Shoot)
				{
					VSIE_Utils.SocialInteractionsManager.Notify_AspirationProgress(killer);
				}
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

	[HarmonyPatch(typeof(Pawn), "Destroy")]
	public static class Destroy_Patch
	{
		private static void Prefix(Pawn __instance)
		{
			VSIE_Utils.SocialInteractionsManager.RemoveDestroyedPawn(__instance);
		}
	}
}
