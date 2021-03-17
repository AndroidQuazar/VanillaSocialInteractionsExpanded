using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using Verse;
using Verse.AI;

namespace VanillaSocialInteractionsExpanded
{
	public class JobGiver_EatDrinkAndTakeDrugsInGatheringArea : ThinkNode_JobGiver
	{
		protected override Job TryGiveJob(Pawn pawn)
		{
            try
            {
                PawnDuty duty = pawn.mindState?.duty;
                if (duty == null)
                {
                    return null;
                }
                if (pawn.needs?.food == null && (double)pawn.needs.food.CurLevelPercentage > 0.9)
                {
                    return null;
                }
                Thing thing = FindFood(pawn);
                if (thing == null)
                {
                    return null;
                }
                Job job = JobMaker.MakeJob(JobDefOf.Ingest, thing);
                job.count = FoodUtility.WillIngestStackCountOf(pawn, thing.def, thing.def.GetStatValueAbstract(StatDefOf.Nutrition));
                //Log.Message(pawn + " - TryGiveJob - return job; - " + job, true);
                return job;
            }
            catch (Exception ex)
            {
                Log.Error("Excention in Binge Party: " + ex, true);
                return null;
            }
		}

        private static Thing SpawnedFoodSearchInnerScan(Pawn eater, IntVec3 root, List<Thing> searchSet, PathEndMode peMode, TraverseParms traverseParams, 
            float maxDistance = 9999f, Predicate<Thing> validator = null)
        {
            if (searchSet == null)
            {
                return null;
            }
            Pawn pawn = traverseParams.pawn ?? eater;
            int num = 0;
            int num2 = 0;
            Thing result = null;
            float num3 = 0f;
            float num4 = float.MinValue;
            for (int i = 0; i < searchSet.Count; i++)
            {
                Thing thing = searchSet[i];
                num2++;
                float num5 = (root - thing.Position).LengthManhattan;
                if (!(num5 > maxDistance))
                {
                    num3 = FoodUtility.FoodOptimality(eater, thing, FoodUtility.GetFinalIngestibleDef(thing), num5);
                    if (!(num3 < num4) && thing.Spawned && (validator == null || validator(thing)))
                    {
                        result = thing;
                        num4 = num3;
                        num++;
                    }
                }
            }
            return result;
        }
        public static Thing FindFood(Pawn pawn)
        {
            Predicate<Thing> validator = delegate (Thing x)
            {
                //Log.Message("Item candidate: " + x, true);
                Log.Message(" - FindFood - if (IntVec3Utility.DistanceTo(x.Position, pawn.Position) > 50f) - 2", true);
                if (IntVec3Utility.DistanceTo(x.Position, pawn.Position) > 50f)
                {
                    Log.Message(" - FindFood - return false; - 3", true);
                    return false;
                }
                Log.Message(" - FindFood - if (!x.IngestibleNow) - 4", true);
                if (!x.IngestibleNow)
                {
                    Log.Message(" - FindFood - return false; - 5", true);
                    return false;
                }
                Log.Message(" - FindFood - if (!x.def.IsNutritionGivingIngestible) - 6", true);
                if (!x.def.IsNutritionGivingIngestible)
                {
                    Log.Message(" - FindFood - return false; - 7", true);
                    return false;
                }
                Log.Message(" - FindFood - if (x.def.IsDrug && !pawn.drugs.CurrentPolicy[x.def].allowedForJoy) - 8", true);
                if (x.def.IsDrug && !pawn.drugs.CurrentPolicy[x.def].allowedForJoy)
                {
                    Log.Message(" - FindFood - return false; - 9", true);
                    return false;
                }
                Log.Message(" - FindFood - if ((int)x.def.ingestible.preferability <= 4 && !x.def.IsDrug) - 10", true);
                if ((int)x.def.ingestible.preferability <= 4 && !x.def.IsDrug)
                {
                    //Log.Message(x.def + " - " + x.def.ingestible.preferability, true);
                    Log.Message(" - FindFood - return false; - 12", true);
                    return false;
                }
                Log.Message(" - FindFood - if (!x.def.IsDrug && !pawn.WillEat(x)) - 13", true);
                if (!x.def.IsDrug && !pawn.WillEat(x))
                {
                    Log.Message(" - FindFood - return false; - 14", true);
                    return false;
                }
                Log.Message(" - FindFood - if (x.IsForbidden(pawn)) - 15", true);
                if (x.IsForbidden(pawn))
                {
                    Log.Message(" - FindFood - return false; - 16", true);
                    return false;
                }
                Log.Message(" - FindFood - if (!x.IsSociallyProper(pawn)) - 17", true);
                if (!x.IsSociallyProper(pawn))
                {
                    Log.Message(" - FindFood - return false; - 18", true);
                    return false;
                }
                //Log.Message(pawn + " canReserve " + x + ": " + canReserve, true);
                Log.Message(" - FindFood - return !x.IsForbidden(pawn) && pawn.CanReserveAndReach(x, PathEndMode.OnCell, Danger.Deadly); - 20", true);
                return !x.IsForbidden(pawn) && pawn.CanReserveAndReach(x, PathEndMode.OnCell, Danger.Deadly);
                Log.Message(" - FindFood - }; - 21", true);
            };

            List<Thing> list = pawn.Map.listerThings.ThingsInGroup(ThingRequestGroup.FoodSourceNotPlantOrTree);
            Log.Message(" - FindFood - Thing foodCandidate = null; - 23", true);
            Thing foodCandidate = null;
            Log.Message(" - FindFood - if (pawn.needs.food.CurLevel < 0.4) - 24", true);
            if (pawn.needs.food.CurLevel < 0.4)
            {
                Log.Message(" - FindFood - foodCandidate = SpawnedFoodSearchInnerScan(pawn, pawn.Position, list, PathEndMode.OnCell, TraverseParms.For(TraverseMode.ByPawn, Danger.Deadly), 50, validator); - 25", true);
                foodCandidate = SpawnedFoodSearchInnerScan(pawn, pawn.Position, list, PathEndMode.OnCell, TraverseParms.For(TraverseMode.ByPawn, Danger.Deadly), 50, validator);
            }
            else
            {
                Log.Message(" - FindFood - foodCandidate = list.Where(x => validator(x)).OrderByDescending(x => x.def.ingestible.joy).ToList().FirstOrDefault(); - 26", true);
                foodCandidate = list.Where(x => validator(x)).OrderByDescending(x => x.def.ingestible.joy).ToList().FirstOrDefault();
                Log.Message(" - FindFood - if (!(foodCandidate.def.ingestible.joy > 0)) - 27", true);
                if (!(foodCandidate.def.ingestible.joy > 0))
                {
                    Log.Message(" - FindFood - foodCandidate = SpawnedFoodSearchInnerScan(pawn, pawn.Position, list, PathEndMode.OnCell, TraverseParms.For(TraverseMode.ByPawn, Danger.Deadly), 50, validator); - 28", true);
                    foodCandidate = SpawnedFoodSearchInnerScan(pawn, pawn.Position, list, PathEndMode.OnCell, TraverseParms.For(TraverseMode.ByPawn, Danger.Deadly), 50, validator);
                }
            }
            //Log.Message(pawn + " - " + foodCandidate, true);
            Log.Message(" - FindFood - return foodCandidate; - 30", true);
            return foodCandidate;
        }
	}
}

