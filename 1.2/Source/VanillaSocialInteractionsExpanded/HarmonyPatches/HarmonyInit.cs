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
}
