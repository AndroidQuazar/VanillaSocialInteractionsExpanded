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
    public class GatheringWorker_MealTogether : GatheringWorker_Dating
    {
        protected override bool PawnsCanGatherTogether(Pawn organizer, Pawn companion)
        {
            return base.PawnsCanGatherTogether(organizer, companion) && TryFindProperFood(organizer, companion);
        }

        public static bool TryFindProperFood(Pawn organizer, Pawn companion)
        {
            if (FoodUtility.TryFindBestFoodSourceFor(organizer, organizer, false, out Thing foodSource, out ThingDef foodDef, canRefillDispenser: false, canUseInventory: true,
                allowForbidden: false, false, allowSociallyImproper: false, organizer.IsWildMan(), false, false, minPrefOverride: FoodPreferability.MealSimple))
            {
                var firstChair = GetChairFor(organizer, foodSource);
                if (firstChair != null && FoodUtility.TryFindBestFoodSourceFor(companion, companion, false, out Thing foodSource2, out ThingDef foodDef2, canRefillDispenser: false, canUseInventory: true,
                allowForbidden: false, false, allowSociallyImproper: false, organizer.IsWildMan(), false, false, minPrefOverride: FoodPreferability.MealSimple))
                {
                    var secondChair = GetChairFor(organizer, foodSource, firstChair);
                    if (secondChair != null)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public static Thing GetChairFor(Pawn pawn, Thing food, Thing firstChair = null)
        {
            Thing thing = GenClosest.ClosestThingReachable(food.Position, pawn.Map, ThingRequest.ForGroup(ThingRequestGroup.BuildingArtificial), PathEndMode.OnCell, TraverseParms.For(pawn),
                food.def.ingestible.chairSearchRadius, (Thing t) => IsProperChair(t, pawn) && (firstChair is null || firstChair != t && firstChair.Position.DistanceTo(t.Position) <= 3f) && t.Position.GetDangerFor(pawn, t.Map) == Danger.None);
            if (thing == null)
            {
                var intVec = RCellFinder.SpotToChewStandingNear(pawn, food);
                Danger chewSpotDanger = intVec.GetDangerFor(pawn, pawn.Map);
                if (chewSpotDanger != Danger.None)
                {
                    thing = GenClosest.ClosestThingReachable(food.Position, pawn.Map, ThingRequest.ForGroup(ThingRequestGroup.BuildingArtificial), PathEndMode.OnCell,
                        TraverseParms.For(pawn), food.def.ingestible.chairSearchRadius, (Thing t) => IsProperChair(t, pawn)
                        && (firstChair is null || firstChair != t && firstChair.Position.DistanceTo(t.Position) <= 3f) && (int)t.Position.GetDangerFor(pawn, t.Map) <= (int)chewSpotDanger);
                }
            }
            return thing;
        }

        private static bool IsProperChair(Thing t, Pawn pawn)
        {
            if (t.def.building == null || !t.def.building.isSittable)
            {
                return false;
            }
            if (t.IsForbidden(pawn))
            {
                return false;
            }
            if (!pawn.CanReserve(t))
            {
                return false;
            }
            if (!t.IsSociallyProper(pawn))
            {
                return false;
            }
            if (t.IsBurning())
            {
                return false;
            }
            if (t.HostileTo(pawn))
            {
                return false;
            }
            bool flag = false;
            for (int i = 0; i < 4; i++)
            {
                Building edifice = (t.Position + GenAdj.CardinalDirections[i]).GetEdifice(t.Map);
                if (edifice != null && edifice.def.surfaceType == SurfaceType.Eat)
                {
                    flag = true;
                    break;
                }
            }
            return flag ? true : false;
        }
        protected override bool TryFindGatherSpot(Pawn organizer, out IntVec3 spot)
        {
            spot = IntVec3.Invalid;
            return true;
        }

        protected override LordJob CreateLordJobCustom(IntVec3 spot, Pawn organizer, Pawn companion)
        {
            return new LordJob_Joinable_MealTogether(organizer, companion, spot, this.def, new IntRange(3 * GenDate.TicksPerHour, 5 * GenDate.TicksPerHour).RandomInRange);
        }
    }
}