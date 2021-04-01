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
				if (VanillaSocialInteractionsExpandedSettings.EnableMemories)
				{
					if (Rand.Chance(0.1f))
					{
						TaleRecorder.RecordTale(VSIE_DefOf.VSIE_RemovedPrisonersOrgans, Recipe_Patch._billDoer);
					}
				}
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
			if (VanillaSocialInteractionsExpandedSettings.EnableAspirations)
			{
				if (surgeon.InspirationDef == VSIE_DefOf.Inspired_Surgery)
				{
					VSIE_Utils.SocialInteractionsManager.Notify_AspirationProgress(surgeon);
				}
			}
		}

		private static void Postfix(Pawn surgeon, Pawn patient, List<Thing> ingredients, BodyPartRecord part, Bill bill)
		{
			_patient = null;
			_surgeon = null;
		}
	}

	[HarmonyPatch(typeof(GenRecipe), "MakeRecipeProducts")]
	public static class MakeRecipeProducts_Patch
	{
		private static void Postfix(IEnumerable<Thing> __result, RecipeDef recipeDef, Pawn worker, List<Thing> ingredients, Thing dominantIngredient, IBillGiver billGiver)
		{
			if (VanillaSocialInteractionsExpandedSettings.EnableAspirations)
			{
				if (worker.InspirationDef == VSIE_DefOf.VSIE_Inspired_Cooking)
				{
					foreach (var thing in __result)
					{
						for (var i = 0; i < thing.stackCount; i++)
						{
							if (thing.def.ingestible?.ingestEffect == EffecterDefOf.EatMeat || thing.def.ingestible?.ingestEffect == DefDatabase< EffecterDef>.GetNamed("EatVegetarian"))
							{
								VSIE_Utils.SocialInteractionsManager.Notify_AspirationProgress(worker);
							}
						}

					}
				}
			}
		}
	}


    [HarmonyPatch(typeof(QualityUtility), "GenerateQualityCreatedByPawn", new Type[] { typeof(Pawn), typeof(SkillDef) })]
    public static class GenerateQualityCreatedByPawn_Patch
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator ilg)
        {
			List<CodeInstruction> instructionList = instructions.ToList();

			for (int i = 0; i < instructionList.Count; i++)
            {
				CodeInstruction instruction = instructionList[i];

				if (instruction.opcode == OpCodes.Ldfld && instruction.LoadsField(AccessTools.Field(typeof(Pawn), nameof(Pawn.mindState))))
                {
					yield return new CodeInstruction(opcode: OpCodes.Pop);
					yield return new CodeInstruction(opcode: OpCodes.Dup);
					yield return new CodeInstruction(opcode: OpCodes.Ldarg_0);
					yield return new CodeInstruction(opcode: OpCodes.Call, operand: AccessTools.Method(typeof(GenerateQualityCreatedByPawn_Patch), nameof(GenerateQualityCreatedByPawn_Patch.Notify_Progress)));
					yield return new CodeInstruction(opcode: OpCodes.Ldarg_0);
                }
				yield return instruction;
            }
        }

        public static void Notify_Progress(QualityCategory level, Pawn pawn)
        {
			if (VanillaSocialInteractionsExpandedSettings.EnableAspirations)
			{
				if (pawn.InspirationDef == VSIE_DefOf.Inspired_Creativity && (level == QualityCategory.Masterwork || level == QualityCategory.Legendary))
				{
					VSIE_Utils.SocialInteractionsManager.Notify_AspirationProgress(pawn);
				}
			}
        }
    }
}
