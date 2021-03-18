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
	[HarmonyPatch(typeof(MemoryThoughtHandler), "TryGainMemory", new Type[]
	{
		typeof(Thought_Memory),
		typeof(Pawn)
	})]
	public static class TryGainMemory_Patch
	{
		private static void Postfix(MemoryThoughtHandler __instance, Thought_Memory newThought, Pawn otherPawn)
		{
			if (VanillaSocialInteractionsExpandedSettings.EnableMemories)
			{
				if (newThought.def == ThoughtDefOf.RebuffedMyRomanceAttempt)
				{
					if (Rand.Chance(0.1f))
					{
						TaleRecorder.RecordTale(VSIE_DefOf.VSIE_RebuffedMe, __instance.pawn, otherPawn);
					}
				}
			}
		}
	}

	[HarmonyPatch(typeof(IndividualThoughtToAdd), "Add")]
	public static class IndividualThoughtToAdd_Patch
	{
		public static void Postfix(IndividualThoughtToAdd __instance)
		{
			if (VanillaSocialInteractionsExpandedSettings.EnableObtainingNewTraits)
			{
				foreach (var relation in DefDatabase<PawnRelationDef>.AllDefs)
				{
					if (relation.opinionOffset >= 20)
					{
						if (relation.diedThought == __instance.thought.def || relation.diedThoughtFemale == __instance.thought.def)
						{
							if (Rand.Chance(0.1f))
							{
								VSIE_Utils.TryDevelopNewTrait(__instance.addTo, "VSIE.TraitChangeFamilyMemberDied");
							}
						}
					}
				}
			}
        }
	}
}
