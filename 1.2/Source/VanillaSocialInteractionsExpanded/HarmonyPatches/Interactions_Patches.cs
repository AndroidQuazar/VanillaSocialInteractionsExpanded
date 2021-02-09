﻿using HarmonyLib;
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
	[HarmonyPatch(typeof(Pawn_InteractionsTracker), "TryInteractWith")]
	public class TryInteractWith_Patch
	{
		private static void Postfix(bool __result, Pawn ___pawn,  Pawn recipient, InteractionDef intDef)
		{
			if (__result)
            {
				if (intDef == InteractionDefOf.Insult)
                {
					TaleRecorder.RecordTale(VSIE_DefOf.VSIE_InsultedMe, ___pawn, recipient);
				}
				else if (intDef == InteractionDefOf.Chitchat || intDef == InteractionDefOf.DeepTalk)
                {
					TaleRecorder.RecordTale(VSIE_DefOf.VSIE_WeHadNiceChat, ___pawn, recipient);
					TaleRecorder.RecordTale(VSIE_DefOf.VSIE_WeHadNiceChat, recipient, ___pawn);
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
		private static void Postfix(Pawn recruiter, Pawn recruitee)
		{
			if (recruitee.def == ThingDefOf.Thrumbo)
			{
				TaleRecorder.RecordTale(VSIE_DefOf.VSIE_TamedThrumbo, recruiter);
			}
		}
	}

	[HarmonyPatch(typeof(InteractionWorker_Breakup), "Interacted")]
	public class Interacted_Patch
	{
		private static void Postfix(Pawn initiator, Pawn recipient)
		{
			TaleRecorder.RecordTale(VSIE_DefOf.VSIE_InsultedMe, recipient, initiator);
		}
	}
}
