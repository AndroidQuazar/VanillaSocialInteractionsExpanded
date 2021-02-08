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
		private static void Prefix(Pawn ___pawn)
		{
			if (InCombat(___pawn))
			{
				TaleRecorder.RecordTale(VSIE_DefOf.VSIE_WasBadlyInjured, ___pawn);
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
}
