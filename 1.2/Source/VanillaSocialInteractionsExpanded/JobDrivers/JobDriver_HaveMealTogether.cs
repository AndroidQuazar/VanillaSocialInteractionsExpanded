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
    public class JobDriver_HaveMealTogether : JobDriver
    {
        private bool eatingFromInventory;
        private Thing IngestibleSource => job.GetTarget(TargetIndex.A).Thing;

        private float ChewDurationMultiplier
        {
            get
            {
                Thing ingestibleSource = IngestibleSource;
                if (ingestibleSource.def.ingestible != null && !ingestibleSource.def.ingestible.useEatingSpeedStat)
                {
                    return 2f;
                }
                return 2f;
            }
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref eatingFromInventory, "eatingFromInventory", defaultValue: false);
        }

        public override void Notify_Starting()
        {
            base.Notify_Starting();
            eatingFromInventory = (pawn.inventory != null && pawn.inventory.Contains(IngestibleSource));
        }

        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
            if (pawn.Faction != null && !(IngestibleSource is Building_NutrientPasteDispenser))
            {
                Thing ingestibleSource = IngestibleSource;
                if (!pawn.Reserve(ingestibleSource, job, 10, FoodUtility.GetMaxAmountToPickup(ingestibleSource, pawn, job.count), null, errorOnFailed))
                {
                    return false;
                }
            }
            return true;
        }

        protected override IEnumerable<Toil> MakeNewToils()
        {
            this.FailOn(() => !IngestibleSource.Destroyed && !IngestibleSource.IngestibleNow);
            Toil chew = ChewIngestibleWithTalking(pawn, ChewDurationMultiplier, TargetIndex.A, TargetIndex.B)
                    .FailOn((Toil x) => !IngestibleSource.Spawned && (pawn.carryTracker == null || pawn.carryTracker.CarriedThing != IngestibleSource))
                    .FailOnCannotTouch(TargetIndex.A, PathEndMode.Touch);
            foreach (Toil item in PrepareToIngestToils(chew))
            {
                yield return item;
            }
            yield return chew;
            yield return Toils_Ingest.FinalizeIngest(pawn, TargetIndex.A);
            yield return Toils_Jump.JumpIf(chew, () => job.GetTarget(TargetIndex.A).Thing is Corpse && pawn.needs.food.CurLevelPercentage < 0.9f);
            Toil saveState = new Toil();
            saveState.initAction = delegate
            {
                Pawn actor = saveState.actor;
                if (actor.GetLord()?.LordJob is LordJob_Joinable_MealTogether lordJob_Joinable_MealTogether)
                {
                    if (lordJob_Joinable_MealTogether.mealsEated is null)
                    {
                        lordJob_Joinable_MealTogether.mealsEated = new Dictionary<Pawn, int>();
                    }
                    if (lordJob_Joinable_MealTogether.mealsEated.ContainsKey(actor))
                    {
                        lordJob_Joinable_MealTogether.mealsEated[actor]++;
                    }
                    else
                    {
                        lordJob_Joinable_MealTogether.mealsEated[actor] = 1;
                    }
                }
            };
            yield return saveState;
        }

        public static Toil ChewIngestibleWithTalking(Pawn chewer, float durationMultiplier, TargetIndex ingestibleInd, TargetIndex eatSurfaceInd = TargetIndex.None)
        {
            Toil toil = new Toil();
            toil.initAction = delegate
            {
                Pawn actor = toil.actor;
                Thing thing4 = actor.CurJob.GetTarget(ingestibleInd).Thing;
                if (!thing4.IngestibleNow)
                {
                    chewer.jobs.EndCurrentJob(JobCondition.Incompletable);
                }
                else
                {
                    toil.actor.pather.StopDead();
                    actor.jobs.curDriver.ticksLeftThisToil = Mathf.RoundToInt((float)thing4.def.ingestible.baseIngestTicks * durationMultiplier);
                    if (thing4.Spawned)
                    {
                        thing4.Map.physicalInteractionReservationManager.Reserve(chewer, actor.CurJob, thing4);
                    }
                }
            };
            toil.tickAction = delegate
            {
                if (chewer != toil.actor)
                {
                    toil.actor.rotationTracker.FaceCell(chewer.Position);
                }
                else
                {
                    Thing thing3 = toil.actor.CurJob.GetTarget(ingestibleInd).Thing;
                    if (thing3 != null && thing3.Spawned)
                    {
                        toil.actor.rotationTracker.FaceCell(thing3.Position);
                    }
                    else if (eatSurfaceInd != 0 && toil.actor.CurJob.GetTarget(eatSurfaceInd).IsValid)
                    {
                        toil.actor.rotationTracker.FaceCell(toil.actor.CurJob.GetTarget(eatSurfaceInd).Cell);
                    }
                }
                toil.actor.GainComfortFromCellIfPossible();
            };
            toil.WithProgressBar(ingestibleInd, delegate
            {
                Thing thing2 = toil.actor.CurJob.GetTarget(ingestibleInd).Thing;
                return (thing2 == null) ? 1f : (1f - (float)toil.actor.jobs.curDriver.ticksLeftThisToil / Mathf.Round((float)thing2.def.ingestible.baseIngestTicks * durationMultiplier));
            });
            toil.defaultCompleteMode = ToilCompleteMode.Delay;
            toil.FailOnDestroyedOrNull(ingestibleInd);
            toil.AddFinishAction(delegate
            {
                if (chewer != null && chewer.CurJob != null)
                {
                    Thing thing = chewer.CurJob.GetTarget(ingestibleInd).Thing;
                    if (thing != null && chewer.Map.physicalInteractionReservationManager.IsReservedBy(chewer, thing))
                    {
                        chewer.Map.physicalInteractionReservationManager.Release(chewer, toil.actor.CurJob, thing);
                    }
                }
            });
            toil.socialMode = RandomSocialMode.SuperActive;
            toil.handlingFacing = true;
            Toils_Ingest.AddIngestionEffects(toil, chewer, ingestibleInd, eatSurfaceInd);
            return toil;
        }

        private IEnumerable<Toil> PrepareToIngestToils(Toil chewToil)
        {
            if (pawn.RaceProps.ToolUser)
            {
                return PrepareToIngestToils_ToolUser(chewToil);
            }
            return PrepareToIngestToils_NonToolUser();
        }

        private IEnumerable<Toil> PrepareToIngestToils_ToolUser(Toil chewToil)
        {
            if (eatingFromInventory)
            {
                yield return Toils_Misc.TakeItemFromInventoryToCarrier(pawn, TargetIndex.A);
            }
            else
            {
                yield return ReserveFood();
                Toil gotoToPickup = Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.ClosestTouch).FailOnDespawnedNullOrForbidden(TargetIndex.A);
                yield return Toils_Jump.JumpIf(gotoToPickup, () => pawn.health.capacities.CapableOf(PawnCapacityDefOf.Manipulation));
                yield return Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.Touch).FailOnDespawnedNullOrForbidden(TargetIndex.A);
                yield return Toils_Jump.Jump(chewToil);
                yield return gotoToPickup;
                yield return Toils_Ingest.PickupIngestible(TargetIndex.A, pawn);
            }
            if (!pawn.Drafted)
            {
                yield return CarryIngestibleToChewSpot(pawn, TargetIndex.A, TargetIndex.B).FailOnDestroyedOrNull(TargetIndex.A);
            }
            yield return Toils_Ingest.FindAdjacentEatSurface(TargetIndex.B, TargetIndex.A);
        }

        public static Toil CarryIngestibleToChewSpot(Pawn pawn, TargetIndex ingestibleInd, TargetIndex cellGoto)
        {
            Toil toil = new Toil();
            toil.initAction = delegate
            {
                Pawn actor = toil.actor;
                IntVec3 intVec = IntVec3.Invalid;
                Thing thing = null;
                Thing thing2 = actor.CurJob.GetTarget(ingestibleInd).Thing;
                Predicate<Thing> baseChairValidator = delegate (Thing t)
                {
                    if (t.def.building == null || !t.def.building.isSittable)
                    {
                        return false;
                    }
                    if (t.IsForbidden(pawn))
                    {
                        return false;
                    }
                    if (!actor.CanReserve(t))
                    {
                        return false;
                    }
                    if (!t.IsSociallyProper(actor))
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
                };
                if (actor.CurJob.GetTarget(TargetIndex.C).Thing != null)
                {
                    thing = actor.CurJob.GetTarget(TargetIndex.C).Thing;
                }

                if (thing != null)
                {
                    intVec = thing.Position;
                    actor.Reserve(thing, actor.CurJob);
                    actor.CurJob.SetTarget(TargetIndex.C, thing);
                }
                actor.Map.pawnDestinationReservationManager.Reserve(actor, actor.CurJob, intVec);
                actor.pather.StartPath(intVec, PathEndMode.OnCell);
            };
            toil.defaultCompleteMode = ToilCompleteMode.PatherArrival;
            return toil;
        }

        private IEnumerable<Toil> PrepareToIngestToils_NonToolUser()
        {
            yield return ReserveFood();
            yield return Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.Touch);
        }

        private Toil ReserveFood()
        {
            return new Toil
            {
                initAction = delegate
                {
                    if (pawn.Faction != null)
                    {
                        Thing thing = job.GetTarget(TargetIndex.A).Thing;
                        if (pawn.carryTracker.CarriedThing != thing)
                        {
                            int maxAmountToPickup = FoodUtility.GetMaxAmountToPickup(thing, pawn, job.count);
                            if (maxAmountToPickup != 0)
                            {
                                if (!pawn.Reserve(thing, job, 10, maxAmountToPickup))
                                {
                                    Log.Error(string.Concat("Pawn food reservation for ", pawn, " on job ", this, " failed, because it could not register food from ", thing, " - amount: ", maxAmountToPickup));
                                    pawn.jobs.EndCurrentJob(JobCondition.Errored);
                                }
                                job.count = maxAmountToPickup;
                            }
                        }
                    }
                },
                defaultCompleteMode = ToilCompleteMode.Instant,
                atomicWithPrevious = true
            };
        }

        public override bool ModifyCarriedThingDrawPos(ref Vector3 drawPos, ref bool behind, ref bool flip)
        {
            IntVec3 cell = job.GetTarget(TargetIndex.B).Cell;
            return ModifyCarriedThingDrawPosWorker(ref drawPos, ref behind, ref flip, cell, pawn);
        }

        public static bool ModifyCarriedThingDrawPosWorker(ref Vector3 drawPos, ref bool behind, ref bool flip, IntVec3 placeCell, Pawn pawn)
        {
            if (pawn.pather.Moving)
            {
                return false;
            }
            Thing carriedThing = pawn.carryTracker.CarriedThing;
            if (carriedThing == null || !carriedThing.IngestibleNow)
            {
                return false;
            }
            if (placeCell.IsValid && placeCell.AdjacentToCardinal(pawn.Position) && placeCell.HasEatSurface(pawn.Map) && carriedThing.def.ingestible.ingestHoldUsesTable)
            {
                drawPos = new Vector3((float)placeCell.x + 0.5f, drawPos.y, (float)placeCell.z + 0.5f);
                return true;
            }
            if (carriedThing.def.ingestible.ingestHoldOffsetStanding != null)
            {
                HoldOffset holdOffset = carriedThing.def.ingestible.ingestHoldOffsetStanding.Pick(pawn.Rotation);
                if (holdOffset != null)
                {
                    drawPos += holdOffset.offset;
                    behind = holdOffset.behind;
                    flip = holdOffset.flip;
                    return true;
                }
            }
            return false;
        }
    }
}