using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using Verse.AI;
using Verse.AI.Group;

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

	//[HarmonyPatch(typeof(TaleRecorder), "RecordTale")]
	//public class RecordTale_Patch
	//{
	//	private static void Postfix(Tale __result, TaleDef def, params object[] args)
	//	{
	//		if (__result != null)
	//		{
	//			Log.Message($"{def.defName} Recording new tale: {__result}");
	//		}
	//		else
    //        {
	//			Log.Message("Couldn't create new tale: " + def + " - " + args);
    //        }
	//	}
	//}
	//
	//[HarmonyPatch(typeof(Pawn_RelationsTracker), "AddDirectRelation")]
	//public class Pawn_RelationsTracker_AddDirectRelation_Patchfff
	//{
	//	private static void Postfix(Pawn ___pawn, PawnRelationDef def, Pawn otherPawn)
	//	{
	//		Log.Message($"{___pawn} developed {def.defName} relationship with {otherPawn}");
	//	}
	//}
	//
	//[HarmonyPatch(typeof(Pawn_RelationsTracker), "TryRemoveDirectRelation")]
	//public class TryRemoveDirectRelation_Patch
	//{
	//	private static void Postfix(bool __result, Pawn ___pawn, PawnRelationDef def, Pawn otherPawn)
	//	{
	//		if (__result)
    //        {
	//			Log.Message($"{___pawn} abandoned {def.defName} relationship with {otherPawn}");
    //        }
	//	}
	//}
	//
	//[HarmonyPatch(typeof(Pawn_InteractionsTracker), "TryInteractWith")]
	//public class TryInteractWith_Patch2
	//{
	//	private static void Postfix(bool __result, Pawn ___pawn, Pawn recipient, InteractionDef intDef)
	//	{
	//		if (__result)
	//		{
	//			Log.Message($"{___pawn} is interacting ({intDef.defName}) with {recipient}");
	//		}
	//	}
	//}
	//
	//[HarmonyPatch(typeof(MemoryThoughtHandler), "TryGainMemory", new Type[]
	//{
	//	typeof(Thought_Memory),
	//	typeof(Pawn)
	//})]
	//public static class TryGainMemory_Patch2
	//{
	//	private static void Postfix(MemoryThoughtHandler __instance, Thought_Memory newThought, Pawn otherPawn)
	//	{
	//		Log.Message(__instance.pawn + " is gaining thought " + newThought.def);
	//	}
	//}
	
    //[HarmonyPatch(typeof(Pawn_JobTracker), "StartJob")]
    //public class StartJobPatch
    //{
    //    private static void Postfix(Pawn_JobTracker __instance, Pawn ___pawn, Job newJob, JobTag? tag)
    //    {
    //        if (___pawn.RaceProps.Humanlike)// && (!___pawn.CurJobDef?.defName.Contains("Wait") ?? false))
    //        {
	//			Log.Message(___pawn + " is starting " + newJob);
	//		}
	//	}
    //}
	//
	//
    //[HarmonyPatch(typeof(Pawn_JobTracker), "EndCurrentJob")]
    //public class EndCurrentJobPatch
    //{
    //    private static void Prefix(Pawn_JobTracker __instance, Pawn ___pawn, JobCondition condition, ref bool startNewJob, bool canReturnToPool = true)
    //    {
	//		if (___pawn.RaceProps.Humanlike)// && (!___pawn.CurJobDef?.defName.Contains("Wait") ?? false))
	//
	//		{
	//			Log.Message(___pawn + " is ending " + ___pawn.CurJob);
	//		}
	//	}
    //}
	//
	//[HarmonyPatch(typeof(ThinkNode_JobGiver), "TryIssueJobPackage")]
	//public class TryIssueJobPackage
	//{
	//	private static void Postfix(ThinkNode_JobGiver __instance, ThinkResult __result, Pawn pawn, JobIssueParams jobParams)
	//	{
	//		if (pawn.RaceProps.Humanlike && __result.Job != null)
	//		{
	//			Log.Message(pawn + " gets " + __result.Job + " from " + __instance);
	//		}
	//	}
	//}
	//[HarmonyPatch(typeof(Pawn), "GetInspectString")]
	//public class GetInspectString_Patch
	//{
	//	private static void Postfix(Pawn __instance, ref string __result)
	//	{
	//		__result += "\n";
	//		__result += "job: " + __instance.CurJob + "\n";
	//		__result += "driver: " + __instance.jobs.curDriver + "\n";
	//		__result += "duty: " + __instance.mindState.duty + "\n";
	//		__result += "lord: " + __instance.GetLord()?.LordJob;
	//	}
	//}

	//[HarmonyPatch(typeof(TimeSlower), "SignalForceNormalSpeed")]
	//public class SignalForceNormalSpeed_Patch
	//{
	//	private static bool Prefix()
	//	{
	//		return false;
	//	}
	//}
	//
	//[HarmonyPatch(typeof(TimeSlower), "SignalForceNormalSpeedShort")]
	//public class SignalForceNormalSpeedShort_Patch
	//{
	//	private static bool Prefix()
	//	{
	//		return false;
	//	}
	//}
}
