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
	[HarmonyPatch(typeof(Thing), "Ingested")]
	public class Thing_Ingested
	{
		private static void Postfix(Thing __instance, Pawn ingester, float nutritionWanted)
		{
			if (VanillaSocialInteractionsExpandedSettings.EnableMemories)
            {
				if (ingester.RaceProps.Humanlike && ingester.Faction == Faction.OfPlayer)
				{
					if (IsHumanlikeMeat(__instance.def))
					{
						if (Rand.Chance(0.1f))
						{
							TaleRecorder.RecordTale(VSIE_DefOf.VSIE_IngestedHumanFlesh, ingester);
						}
						return;
					}
					else
					{
						CompIngredients compIngredients = __instance.TryGetComp<CompIngredients>();
						if (compIngredients != null)
						{
							for (int i = 0; i < compIngredients.ingredients.Count; i++)
							{
								if (IsHumanlikeMeat(compIngredients.ingredients[i]))
								{
									if (Rand.Chance(0.1f))
									{
										TaleRecorder.RecordTale(VSIE_DefOf.VSIE_IngestedHumanFlesh, ingester);
									}
									return;
								}
							}
						}
					}
				}
			}
		}

		public static bool IsHumanlikeMeat(ThingDef def)
		{
			if (def.ingestible.sourceDef != null && def.ingestible.sourceDef.race != null && def.ingestible.sourceDef.race.Humanlike)
			{
				return true;
			}
			return false;
		}

	}
}
