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
using Verse.AI.Group;

namespace VanillaSocialInteractionsExpanded
{
	[HarmonyPatch(typeof(Pawn_InteractionsTracker), "TryInteractWith")]
	public class TryInteractWith_Patch
	{
		private static void Postfix(bool __result, Pawn ___pawn,  Pawn recipient, InteractionDef intDef)
		{
			if (__result)
            {
				if (intDef == InteractionDefOf.Insult)
                {
					if (Rand.Chance(0.1f))
					{
						TaleRecorder.RecordTale(VSIE_DefOf.VSIE_InsultedMe, recipient, ___pawn);
					}
				}
				else if (intDef == InteractionDefOf.Chitchat || intDef == InteractionDefOf.DeepTalk)
                {
					if (VanillaSocialInteractionsExpandedSettings.EnableMemories)
                    {
						if (Rand.Chance(0.1f))
						{
							TaleRecorder.RecordTale(VSIE_DefOf.VSIE_WeHadNiceChat, recipient, ___pawn);
							TaleRecorder.RecordTale(VSIE_DefOf.VSIE_WeHadNiceChat, ___pawn, recipient);
						}
					}
					if (VanillaSocialInteractionsExpandedSettings.EnableBestFriend)
                    {
						var pawnBestFriend = ___pawn.relations.GetFirstDirectRelationPawn(VSIE_DefOf.VSIE_BestFriend);
						var recipientBestFriend = recipient.relations.GetFirstDirectRelationPawn(VSIE_DefOf.VSIE_BestFriend);
						var recipientOpinionOf = recipient.relations.OpinionOf(___pawn);
						var ___pawnOpinionOf = ___pawn.relations.OpinionOf(recipient);

						if (pawnBestFriend is null && recipient.relations.GetFirstDirectRelationPawn(VSIE_DefOf.VSIE_BestFriend) is null)
						{
							if (recipientOpinionOf >= 80 && ___pawnOpinionOf >= 80 && !recipient.relations.DirectRelationExists(VSIE_DefOf.VSIE_BestFriend, ___pawn))
							{
								recipient.relations.AddDirectRelation(VSIE_DefOf.VSIE_BestFriend, ___pawn);
							}
						}
						else if (pawnBestFriend != null && recipientBestFriend != null && (___pawnOpinionOf < 80 || recipientOpinionOf < 80)) 
						{
							recipient.relations.TryRemoveDirectRelation(VSIE_DefOf.VSIE_BestFriend, ___pawn);
							___pawn.relations.TryRemoveDirectRelation(VSIE_DefOf.VSIE_BestFriend, recipient);
						}
					}
				}
			}
		}
	}

	[HarmonyPatch(typeof(InteractionWorker_RomanceAttempt), "SuccessChance")]
	public class SuccessChance_Patch
	{
		private static void Postfix(ref float __result, Pawn initiator, Pawn recipient)
		{
			if (initiator.InspirationDef == VSIE_DefOf.VSIE_Flirting_Frenzy)
			{
				__result *= 2f;
			}
		}
	}

	[HarmonyPatch(typeof(InteractionWorker_RomanceAttempt), "RandomSelectionWeight")]
	public class InteractionWorker_RomanceAttempt_RandomSelectionWeight_Patch
	{
		private static void Postfix(ref float __result, Pawn initiator, Pawn recipient)
		{
			if (VanillaSocialInteractionsExpandedSettings.EnableDating)
            {
				if (VSIE_Utils.GetCompanion(initiator) == recipient)
				{
					__result *= 1.2f;
				}
			}

		}
	}

	[HarmonyPatch(typeof(InteractionWorker_KindWords), "RandomSelectionWeight")]
	public class InteractionWorker_KindWords_RandomSelectionWeight_Patch
	{
		private static void Postfix(ref float __result, Pawn initiator, Pawn recipient)
		{
			if (VanillaSocialInteractionsExpandedSettings.EnableDating)
			{
				if (VSIE_Utils.GetCompanion(initiator) == recipient)
				{
					__result = 0.1f * 1.2f;
				}
			}
		}
	}

	[HarmonyPatch(typeof(InteractionWorker_DeepTalk), "RandomSelectionWeight")]
	public class InteractionWorker_DeepTalk_RandomSelectionWeight_Patch
	{
		private static void Postfix(ref float __result, Pawn initiator, Pawn recipient)
		{
			if (VanillaSocialInteractionsExpandedSettings.EnableDating)
			{
				if (VSIE_Utils.GetCompanion(initiator) == recipient)
				{
					__result *= 1.2f;
				}
			}
		}
	}
	[HarmonyPatch(typeof(Pawn_InteractionsTracker), "TryInteractRandomly")]
	public class TryInteractRandomly_Patch
	{
		private static bool Prefix(Pawn_InteractionsTracker __instance, Pawn ___pawn, ref bool __result)
		{
			Pawn p = GetSpecifiedTalkerFor(___pawn);
			if (p != null)
            {
				if (__instance.InteractedTooRecentlyToInteract())
				{
					__result = false;
					return false;
				}
				if (!InteractionUtility.CanInitiateRandomInteraction(___pawn))
				{
					__result = false;
					return false;
				}
				List<InteractionDef> allDefsListForReading = DefDatabase<InteractionDef>.AllDefsListForReading;
				if (p != ___pawn && __instance.CanInteractNowWith(p) && InteractionUtility.CanReceiveRandomInteraction(p) && !___pawn.HostileTo(p) 
					&& allDefsListForReading.TryRandomElementByWeight((InteractionDef x) => (!__instance.CanInteractNowWith(p, x)) ? 0f : x.Worker.RandomSelectionWeight(___pawn, p), out InteractionDef result))
				{
					if (__instance.TryInteractWith(p, result))
					{
						__result = true;
						return false;
					}
					Log.Error(string.Concat(___pawn, " failed to interact with ", p));
				}
				__result = false;
				return false;
			}
			return true;
		}

		private static Pawn GetSpecifiedTalkerFor(Pawn pawn)
        {
			var lord = pawn.GetLord();
			if (lord != null && lord.LordJob is LordJob_Joinable_DoublePawn lordJob)
            {
				if (pawn == lordJob.Organizer)
                {
					return lordJob.secondPawn;
                }
				else
                {
					return lordJob.Organizer;
                }
            }
			if (pawn.CurJobDef == VSIE_DefOf.VSIE_TalkToSecondPawn)
            {
				return pawn.CurJob.targetA.Thing as Pawn;
            }
			return null;
        }
	}

	[HarmonyPatch(typeof(InteractionWorker_RecruitAttempt), "Interacted")]
	public class InteractionWorker_RecruitAttempt_Interacted_Patch
	{
		private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
		{
			FieldInfo mindStateInfo = AccessTools.Field(typeof(Pawn), "mindState");
			FieldInfo inspirationHandlerInfo = AccessTools.Field(typeof(Pawn_MindState), "inspirationHandler");
			FieldInfo inspired_TamingInfo = AccessTools.Field(typeof(InspirationDefOf), "Inspired_Taming");

			var codes = instructions.ToList();
			bool found = false;
			for (var i = 0; i < codes.Count; i++)
			{
				if (!found && codes[i].OperandIs(mindStateInfo) && codes[i + 1].OperandIs(inspirationHandlerInfo) && codes[i + 2].OperandIs(inspired_TamingInfo))
				{
					found = true;
					yield return new CodeInstruction(OpCodes.Ldarg_1, null);
					yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(InteractionWorker_RecruitAttempt_Interacted_Patch), "Notify_Progress", null, null));
					yield return codes[i];
				}
				else
				{
					yield return codes[i];
				}
			}
			yield break;
		}

		public static void Notify_Progress(Pawn pawn)
		{
			if (VanillaSocialInteractionsExpandedSettings.EnableAspirations)
			{
				if (pawn.InspirationDef == VSIE_DefOf.Inspired_Taming)
				{
					VSIE_Utils.SocialInteractionsManager.Notify_AspirationProgress(pawn);
				}
			}
		}
	}


	[HarmonyPatch(typeof(InteractionWorker_RecruitAttempt), "DoRecruit", new Type[] 
	{
		typeof(Pawn), typeof(Pawn), typeof(float), typeof(string), typeof(string), typeof(bool), typeof(bool)
	}, new ArgumentType[] 
	{
		ArgumentType.Normal, ArgumentType.Normal, ArgumentType.Normal, ArgumentType.Out, ArgumentType.Out, ArgumentType.Normal, ArgumentType.Normal
	})]
	public class DoRecruit_Patch
	{
		private static void Prefix(Pawn recruiter, Pawn recruitee)
        {
			if (VanillaSocialInteractionsExpandedSettings.EnableObtainingNewTraits)
			{
				if (recruitee.IsPrisoner && recruiter != null)
				{
					if (Rand.Chance(0.1f))
					{
						VSIE_Utils.TryDevelopNewTrait(recruitee, "VSIE.TraitChangePrisonerRecruitedText");
					}
				}
			}

			if (VanillaSocialInteractionsExpandedSettings.EnableAspirations)
			{
				if (recruitee.IsPrisoner && recruiter?.InspirationDef == VSIE_DefOf.Inspired_Recruitment)
				{
					VSIE_Utils.SocialInteractionsManager.Notify_AspirationProgress(recruiter);
				}
			}
        }
		private static void Postfix(Pawn recruiter, Pawn recruitee)
		{
			if (VanillaSocialInteractionsExpandedSettings.EnableMemories)
			{
				if (recruitee.def == ThingDefOf.Thrumbo)
				{
					if (Rand.Chance(0.1f))
					{
						TaleRecorder.RecordTale(VSIE_DefOf.VSIE_TamedThrumbo, recruiter);
					}
				}
			}
		}
	}

	[HarmonyPatch(typeof(InteractionWorker_Breakup), "Interacted")]
	public class Interacted_Patch
	{
		private static void Postfix(Pawn initiator, Pawn recipient)
		{
			if (VanillaSocialInteractionsExpandedSettings.EnableMemories)
			{
				if (Rand.Chance(0.1f))
				{
					TaleRecorder.RecordTale(VSIE_DefOf.VSIE_BrokeUpWithMe, recipient, initiator);
				}
			}
		}
	}
}
