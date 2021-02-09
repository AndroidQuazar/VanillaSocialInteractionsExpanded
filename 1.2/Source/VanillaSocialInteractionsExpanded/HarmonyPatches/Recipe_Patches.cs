using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Verse;
using Verse.AI;


namespace VanillaSocialInteractionsExpanded
{
	[StaticConstructorOnStartup]
	public static class Recipe_Patch
	{
		public static Pawn _billDoer;
		public static Pawn _pawn;
		static Recipe_Patch()
        {
			MethodInfo method = AccessTools.TypeByName("RimWorld.Recipe_RemoveBodyPart").GetMethod("ApplyOnPawn");
			MethodInfo prefix = AccessTools.Method(typeof(Recipe_Patch), "Prefix");
			MethodInfo postfix = AccessTools.Method(typeof(Recipe_Patch), "Postfix");
			HarmonyInit.harmony.Patch(method, new HarmonyMethod(prefix));
			HarmonyInit.harmony.Patch(method, new HarmonyMethod(postfix));
		}

		private static void Prefix(Pawn pawn, BodyPartRecord part, Pawn billDoer, List<Thing> ingredients, Bill bill)
		{
			_billDoer = billDoer;
			_pawn = pawn;
		}
		private static void Postfix(Pawn pawn, BodyPartRecord part, Pawn billDoer, List<Thing> ingredients, Bill bill)
		{
			_billDoer = null;
			_pawn = null;
		}
	}

	[HarmonyPatch(typeof(ThoughtUtility), "GiveThoughtsForPawnOrganHarvested")]
	public static class GiveThoughtsForPawnOrganHarvested_Patch
	{
		private static void Prefix(Pawn victim)
		{
			if (victim != null && victim == Recipe_Patch._pawn)
            {
				TaleRecorder.RecordTale(VSIE_DefOf.VSIE_RemovedPrisonersOrgans, Recipe_Patch._billDoer);
				Recipe_Patch._pawn = null;
				Recipe_Patch._billDoer = null;
			}
		}
	}

	[HarmonyPatch(typeof(Recipe_Surgery), "CheckSurgeryFail")]
	public static class CheckSurgeryFail_Patch
	{
		public static Pawn _surgeon;

		public static Pawn _patient;

		private static void Prefix(Pawn surgeon, Pawn patient, List<Thing> ingredients, BodyPartRecord part, Bill bill)
		{
			_patient = patient;
			_surgeon = surgeon;
		}

		private static void Postfix(Pawn surgeon, Pawn patient, List<Thing> ingredients, BodyPartRecord part, Bill bill)
		{
			_patient = null;
			_surgeon = null;
		}
	}
}
