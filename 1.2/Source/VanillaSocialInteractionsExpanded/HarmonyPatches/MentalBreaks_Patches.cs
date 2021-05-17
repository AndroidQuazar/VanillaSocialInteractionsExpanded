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
	[HarmonyPatch(typeof(MentalBreakWorker_RunWild), "TryStart")]
	public class MentalBreakWorker_RunWild_Patch
	{
		private static void Postfix(Pawn pawn, string reason, bool causedByMood)
        {
			if (VanillaSocialInteractionsExpandedSettings.EnableMemories)
			{
				if (Rand.Chance(0.1f))
				{
					TaleRecorder.RecordTale(VSIE_DefOf.VSIE_RanWild, pawn);
				}
			}
		}
	}

	[HarmonyPatch(typeof(MentalBreakWorker_Catatonic), "TryStart")]
	public class MentalBreakWorker_Catatonic_Patch
	{
		private static void Postfix(Pawn pawn, string reason, bool causedByMood)
		{
			if (VanillaSocialInteractionsExpandedSettings.EnableObtainingNewTraits)
			{
				if (Rand.Chance(0.1f))
				{
					VSIE_Utils.TryDevelopNewTrait(pawn, "VSIE.TraitChangePawnHasCatatonicBreakdown");
				}
			}
		}
	}

	[HarmonyPatch(typeof(MentalState), "RecoverFromState")]
	public class RecoverFromState_Patch
	{
		private static void Prefix(MentalState __instance)
		{
			if (VanillaSocialInteractionsExpandedSettings.EnableObtainingNewTraits)
			{
				if (__instance.def.IsExtreme)
				{
					if (Rand.Chance(0.1f))
					{
						VSIE_Utils.TryDevelopNewTrait(__instance.pawn, "VSIE.CatarhisAfterMentalBreak");
					}
				}
			}
		}
	}

	[HarmonyPatch(typeof(MentalState_Slaughterer), "Notify_SlaughteredAnimal")]
	public class Notify_SlaughteredAnimal_Patch
	{
		private static void Postfix(Pawn ___pawn)
		{
			if (VanillaSocialInteractionsExpandedSettings.EnableMemories)
			{
				if (Rand.Chance(0.1f))
				{
					TaleRecorder.RecordTale(VSIE_DefOf.VSIE_SlaughteredAnimalInRage, ___pawn);
				}
			}
		}
	}

	[HarmonyPatch(typeof(MentalState_Jailbreaker), "Notify_InducedPrisonerToEscape")]
	public class Notify_InducedPrisonerToEscape_Patch
	{
		private static void Postfix(Pawn ___pawn)
		{
			if (VanillaSocialInteractionsExpandedSettings.EnableMemories)
			{
				if (Rand.Chance(0.1f))
				{
					TaleRecorder.RecordTale(VSIE_DefOf.VSIE_InducedPrisonerToEscape, ___pawn);
				}
			}
		}
	}

	[HarmonyPatch(typeof(MentalState_SocialFighting), "PostEnd")]
	public class PostEnd_Patch
	{
		private static void Postfix(MentalState_SocialFighting __instance)
		{
			if (VanillaSocialInteractionsExpandedSettings.EnableMemories)
			{
				if (Rand.Chance(0.1f))
				{
					TaleRecorder.RecordTale(VSIE_DefOf.VSIE_WeHadSocialFight, __instance.pawn, __instance.otherPawn);
					TaleRecorder.RecordTale(VSIE_DefOf.VSIE_WeHadSocialFight, __instance.otherPawn, __instance.pawn);
				}
			}
		}
	}

	[HarmonyPatch(typeof(MentalState_SocialFighting), "IsOtherPawnSocialFightingWithMe", MethodType.Getter)]
	public class IsOtherPawnSocialFightingWithMe_Patch
	{
		private static void Postfix(MentalState_SocialFighting __instance, ref bool __result)
		{
			if (VanillaSocialInteractionsExpandedSettings.EnableDiscord)
			{
				if (!__result && __instance.otherPawn.InMentalState)
				{
					var socialManager = VSIE_Utils.SocialInteractionsManager;
					if (socialManager.angryWorkers != null
						&& socialManager.angryWorkers.TryGetValue(__instance.pawn, out int lastTick) && lastTick + (GenDate.TicksPerHour * 12) > Find.TickManager.TicksGame
						&& socialManager.angryWorkers.TryGetValue(__instance.otherPawn, out int lastTick2) && lastTick2 + (GenDate.TicksPerHour * 12) > Find.TickManager.TicksGame)
					{
						__result = true;
					}
				}
			}
		}
	}

	[HarmonyPatch(typeof(MentalStateHandler), "TryStartMentalState")]
	public class TryStartMentalState_Patch
	{
		private static bool Prefix(MentalStateHandler __instance, Pawn ___pawn, bool __result, MentalStateDef stateDef, string reason = null, bool forceWake = false, bool causedByMood = false, Pawn otherPawn = null, bool transitionSilently = false)
        {
			if (VanillaSocialInteractionsExpandedSettings.EnableVenting)
			{
				if (causedByMood)
				{
					var friendsToVent = VSIE_Utils.GetFriendsFor(___pawn).Where(x => x.relations != null && x.Map == ___pawn.Map && x.Position.DistanceTo(___pawn.Position) <= 30);
					if (friendsToVent.Any())
					{
						var friendToVent = friendsToVent.RandomElementByWeight(x => x.relations.OpinionOf(___pawn));
						var job = JobMaker.MakeJob(VSIE_DefOf.VSIE_VentToFriend, friendToVent);
						if (___pawn.jobs?.TryTakeOrderedJob(job) ?? false)
						{
							__result = false;
							return false;
						}
					}
				}
				if (VSIE_Utils.SocialInteractionsManager.postRaidPeriodTicks > Find.TickManager.TicksGame)
                {
					__result = false;
					return false;
				}
			}
			return true;
        }
		private static void Postfix(MentalStateHandler __instance, Pawn ___pawn, bool __result, MentalStateDef stateDef, string reason = null, bool forceWake = false, bool causedByMood = false, Pawn otherPawn = null, bool transitionSilently = false)
		{
			if (VanillaSocialInteractionsExpandedSettings.EnableMemories)
			{
				if (__result)
				{
					if (stateDef == MentalStateDefOf.Wander_OwnRoom)
					{
						if (Rand.Chance(0.1f))
						{
							TaleRecorder.RecordTale(VSIE_DefOf.VSIE_HideInRoom, ___pawn);
						}
					}
					else if (stateDef == MentalStateDefOf.Wander_Sad)
					{
						if (Rand.Chance(0.1f))
						{
							TaleRecorder.RecordTale(VSIE_DefOf.VSIE_WanderedInSaddness, ___pawn);
						}
					}
					else if (stateDef == VSIE_DefOf.SadisticRage)
					{
						if (Rand.Chance(0.1f))
						{
							TaleRecorder.RecordTale(VSIE_DefOf.VSIE_WentIntoSadisticRage, ___pawn);
						}
					}
					else if (stateDef == VSIE_DefOf.Tantrum || stateDef == VSIE_DefOf.BedroomTantrum || stateDef == VSIE_DefOf.TargetedTantrum)
					{
						if (Rand.Chance(0.1f))
						{
							TaleRecorder.RecordTale(VSIE_DefOf.VSIE_ThrewTantrum, ___pawn);
						}
					}
					else if (stateDef == MentalStateDefOf.Berserk)
					{
						if (Rand.Chance(0.1f))
						{
							TaleRecorder.RecordTale(VSIE_DefOf.VSIE_WentBerserk, ___pawn);
						}
					}
					else if (stateDef == VSIE_DefOf.FireStartingSpree)
					{
						if (Rand.Chance(0.1f))
						{
							TaleRecorder.RecordTale(VSIE_DefOf.VSIE_WentOnFireStartingSpree, ___pawn);
						}
					}
					else if (stateDef == VSIE_DefOf.MurderousRage)
					{
						if (Rand.Chance(0.1f))
						{
							TaleRecorder.RecordTale(VSIE_DefOf.VSIE_WentOnMurderousRage, ___pawn);
						}
					}
				}
			}
		}
	}
}
