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
	[HarmonyPatch(typeof(JobGiver_GetJoyInGatheringArea), "TryGiveJobFromJoyGiverDefDirect")]
	public class TryGiveJobFromJoyGiverDefDirect_Patch
	{
		private static void Postfix(JoyGiverDef def, Pawn pawn)
		{
			if (VanillaSocialInteractionsExpandedSettings.EnableAspirations)
			{
				if (pawn.InspirationDef == VSIE_DefOf.VSIE_Party_Frenzy)
				{
					VSIE_Utils.SocialInteractionsManager.Notify_AspirationProgress(pawn);
				}
			}
		}
	}

	[HarmonyPatch(typeof(JobGiver_EatInGatheringArea), "TryGiveJob")]
	public class TryGiveJob_Patch
	{
		private static void Postfix(Pawn pawn)
		{
			if (VanillaSocialInteractionsExpandedSettings.EnableAspirations)
			{
				if (pawn.InspirationDef == VSIE_DefOf.VSIE_Party_Frenzy)
				{
					VSIE_Utils.SocialInteractionsManager.Notify_AspirationProgress(pawn);
				}
			}
		}
	}

	[HarmonyPatch(typeof(JobGiver_StandAndBeSociallyActive), "TryGiveJob")]
	public class JobGiver_StandAndBeSociallyActive_TryGiveJob_Patch
	{
		private static void Postfix(Pawn pawn)
		{
			if (VanillaSocialInteractionsExpandedSettings.EnableAspirations)
			{
				if (pawn.InspirationDef == VSIE_DefOf.VSIE_Party_Frenzy)
				{
					VSIE_Utils.SocialInteractionsManager.Notify_AspirationProgress(pawn);
				}
			}
		}
	}
}
