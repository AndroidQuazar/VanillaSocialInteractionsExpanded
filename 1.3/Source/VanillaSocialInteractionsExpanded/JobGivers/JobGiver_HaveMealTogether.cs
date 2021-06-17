using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;
using Verse.AI;
using Verse.AI.Group;
using Verse.Grammar;

namespace VanillaSocialInteractionsExpanded
{
    public class JobGiver_HaveMealTogether : ThinkNode_JobGiver
    {
		protected override Job TryGiveJob(Pawn pawn)
		{
			var lordJob = pawn.GetLord()?.LordJob as LordJob_Joinable_MealTogether;
			if (lordJob?.mealsEated != null && lordJob.mealsEated.TryGetValue(pawn, out var value) && value >= 1)
            {
				return null;
            }
			var companion = VSIE_Utils.GetCompanion(pawn);
			if (companion.CurJobDef == VSIE_DefOf.VSIE_HaveMealTogether)
			{
				if (FoodUtility.TryFindBestFoodSourceFor(pawn, pawn, false, out Thing foodSource, out ThingDef foodDef, canRefillDispenser: false, canUseInventory: true,
						allowForbidden: false, false, allowSociallyImproper: false, pawn.IsWildMan(), false, false, minPrefOverride: FoodPreferability.MealSimple))
                {
					var chair = GatheringWorker_MealTogether.GetChairFor(pawn, foodSource, companion.CurJob.targetC.Thing);
					var job = JobMaker.MakeJob(VSIE_DefOf.VSIE_HaveMealTogether, foodSource, null, chair);
					float nutrition = FoodUtility.GetNutrition(foodSource, foodDef);
					job.count = FoodUtility.WillIngestStackCountOf(pawn, foodDef, nutrition);
					return job;
				}
			}
			else if (FoodUtility.TryFindBestFoodSourceFor(pawn, pawn, false, out Thing foodSource, out ThingDef foodDef, canRefillDispenser: false, canUseInventory: true,
						allowForbidden: false, false, allowSociallyImproper: false, pawn.IsWildMan(), false, false, minPrefOverride: FoodPreferability.MealSimple))
			{
				var chair = GatheringWorker_MealTogether.GetChairFor(pawn, foodSource);
				var job = JobMaker.MakeJob(VSIE_DefOf.VSIE_HaveMealTogether, foodSource, null, chair);
				float nutrition = FoodUtility.GetNutrition(foodSource, foodDef);
				job.count = FoodUtility.WillIngestStackCountOf(pawn, foodDef, nutrition);
				return job;
			}
			return null;
		}
	}
}

