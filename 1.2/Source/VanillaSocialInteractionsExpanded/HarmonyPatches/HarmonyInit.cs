using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace VanillaSocialInteractionsExpanded
{
	[StaticConstructorOnStartup]
	internal static class HarmonyInit
	{
		public static Harmony harmony;
		static HarmonyInit()
		{
			harmony = new Harmony("OskarPotocki.VanillaSocialInteractionsExpanded");
			harmony.PatchAll();
		}
	}


	[HarmonyPatch(typeof(TaleRecorder), "RecordTale")]
	public class RecordTale_Patch
	{
		private static void Postfix(Tale __result, TaleDef def, params object[] args)
		{
			if (__result != null)
			{
				Log.Message($"{def.defName} Recording new tale: {__result}");
			}
			else
            {
				Log.Message("Couldn't create new tale: " + def + " - " + args);
            }
		}
	}

	[HarmonyPatch(typeof(Pawn_RelationsTracker), "AddDirectRelation")]
	public class Pawn_RelationsTracker_AddDirectRelation_Patchfff
	{
		private static void Postfix(Pawn ___pawn, PawnRelationDef def, Pawn otherPawn)
		{
			Log.Message($"{___pawn} developed {def.defName} relationship with {otherPawn}");
		}
	}
	
	[HarmonyPatch(typeof(Pawn_RelationsTracker), "TryRemoveDirectRelation")]
	public class TryRemoveDirectRelation_Patch
	{
		private static void Postfix(bool __result, Pawn ___pawn, PawnRelationDef def, Pawn otherPawn)
		{
			if (__result)
            {
				Log.Message($"{___pawn} abandoned {def.defName} relationship with {otherPawn}");
            }
		}
	}

	[HarmonyPatch(typeof(Pawn_InteractionsTracker), "TryInteractWith")]
	public class TryInteractWith_Patch2
	{
		private static void Postfix(bool __result, Pawn ___pawn, Pawn recipient, InteractionDef intDef)
		{
			if (__result)
			{
				Log.Message($"{___pawn} is interacting ({intDef.defName}) with {recipient}");
			}
		}
	}

	[HarmonyPatch(typeof(MemoryThoughtHandler), "TryGainMemory", new Type[]
	{
		typeof(Thought_Memory),
		typeof(Pawn)
	})]
	public static class TryGainMemory_Patch2
	{
		private static void Postfix(MemoryThoughtHandler __instance, Thought_Memory newThought, Pawn otherPawn)
		{
			Log.Message(__instance.pawn + " is gaining thought " + newThought.def);
		}
	}
}
