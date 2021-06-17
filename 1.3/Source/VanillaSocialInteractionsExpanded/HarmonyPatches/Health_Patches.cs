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
	[HarmonyPatch(typeof(Pawn_HealthTracker), "MakeDowned")]
	public static class MakeDowned_Patch
	{
		private static void Prefix(Pawn ___pawn, DamageInfo? dinfo, Hediff hediff)
		{
			if (VanillaSocialInteractionsExpandedSettings.EnableMemories)
            {
				if (InCombat(___pawn) && GenAI.EnemyIsNear(___pawn, 25f))
				{
					if (Rand.Chance(0.1f))
					{
						TaleRecorder.RecordTale(VSIE_DefOf.VSIE_WasBadlyInjured, ___pawn);
					}
				}
			}

			if (dinfo.HasValue)
            {
				if (VanillaSocialInteractionsExpandedSettings.EnableMemories)
                {
					Pawn_Kill_Patch.TryRecordSavedMeFromRaiders(___pawn, dinfo);
                }
				if (VanillaSocialInteractionsExpandedSettings.EnableAspirations)
				{
					Pawn_Kill_Patch.TryRecordMeleeAspiration(dinfo);
				}
			}
		}

		public static HashSet<JobDef> combatJobs = new HashSet<JobDef>
													{
														JobDefOf.AttackMelee,
														JobDefOf.AttackStatic,
														JobDefOf.FleeAndCower,
														JobDefOf.ManTurret,
														JobDefOf.Wait_Combat,
														JobDefOf.Flee
													};
		private static bool InCombat(Pawn pawn)
		{
			if (combatJobs.Contains(pawn.CurJobDef))
			{
				return true;
			}
			else if (pawn.mindState.duty?.def.alwaysShowWeapon ?? false)
			{
				return true;
			}
			else if (pawn.CurJobDef?.alwaysShowWeapon ?? false)
			{
				return true;
			}
			else if (pawn.mindState.lastEngageTargetTick > Find.TickManager.TicksGame - 1000)
            {
				return true;
            }
			else if (pawn.mindState.lastAttackTargetTick > Find.TickManager.TicksGame - 1000)
			{
				return true;
			}
			return false;
		}
	}

	[HarmonyPatch(typeof(ImmunityRecord), "ImmunityTick")]
	public static class ImmunityTick_Patch
	{
		public static Dictionary<Pawn, int> checkedPawns = new Dictionary<Pawn, int>();
		private static void Postfix(ImmunityRecord __instance, Pawn pawn, bool sick, Hediff diseaseInstance)
		{
			if (VanillaSocialInteractionsExpandedSettings.EnableObtainingNewTraits)
            {
				if (__instance.hediffDef.lethalSeverity == 1f && __instance.immunity == 1f)
				{
					if (checkedPawns.TryGetValue(pawn, out int checkedTicks) && Find.TickManager.TicksGame < checkedTicks + 60000) return;
					if (Rand.Chance(0.1f))
					{
						VSIE_Utils.TryDevelopNewTrait(pawn, "VSIE.RecoveredAfterDeadlyDisease");
					}
					checkedPawns[pawn] = Find.TickManager.TicksGame;
				}
			}
		}
	}


	[HarmonyPatch(typeof(TendUtility), "DoTend")]
	public static class DoTend_Patch
	{
		private static void Prefix(Pawn doctor, Pawn patient, Medicine medicine, out bool __state)
		{
			if (patient.health.HasHediffsNeedingTend())
            {
				__state = true;
            }
			else
            {
				__state = false;
            }
		}

		private static void Postfix(Pawn doctor, Pawn patient, Medicine medicine, bool __state)
		{
			if (VanillaSocialInteractionsExpandedSettings.EnableMemories)
            {
				if (__state && !patient.health.HasHediffsNeedingTend() && doctor != null)
				{
					if (Rand.Chance(0.1f))
					{
						TaleRecorder.RecordTale(VSIE_DefOf.VSIE_SavedMeFromMyWounds, patient, doctor);
					}
					foreach (var pawn in patient.relations.PotentiallyRelatedPawns)
					{
						if (pawn.relations.OpinionOf(patient) >= 20f)
						{
							if (Rand.Chance(0.1f))
							{
								TaleRecorder.RecordTale(VSIE_DefOf.VSIE_CuredMyFriend, pawn, doctor, patient);
							}
						}
					}
				}
			}
		}
	}
}
