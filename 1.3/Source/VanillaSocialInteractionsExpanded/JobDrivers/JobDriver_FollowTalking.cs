using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;
using Verse.AI;
using Verse.Grammar;

namespace VanillaSocialInteractionsExpanded
{
	public class JobDriver_FollowTalking : JobDriver
	{
		public override bool TryMakePreToilReservations(bool errorOnFailed)
		{
			return true;
		}
        protected override IEnumerable<Toil> MakeNewToils()
        {
            this.FailOnDespawnedOrNull(TargetIndex.A);
            Toil toil = new Toil();
            toil.tickAction = delegate
            {
                Pawn target = (Pawn)job.GetTarget(TargetIndex.A).Thing;
                if (base.pawn.Position.DistanceTo(target.Position) > 6f)
                {
                    if (!base.pawn.CanReach(target, PathEndMode.Touch, Danger.Deadly))
                    {
                        EndJobWith(JobCondition.Incompletable);
                    }
                    else if (!base.pawn.pather.Moving || base.pawn.pather.Destination != target)
                    {
                        base.pawn.pather.StartPath(target, PathEndMode.Touch);
                    }
                }
                else if (!base.pawn.pather.Moving)
                {
                    base.pawn.pather.StartPath(target.CurJob.targetB.Cell.RandomAdjacentCell8Way(), PathEndMode.OnCell);
                }
                var pawnDistanceToTarget = base.pawn.Position.DistanceTo(target.Position);
                var pawnDistanceToWalkCell = base.pawn.Position.DistanceTo(target.CurJob.targetB.Cell);
                var targetDistanceToWalkCell = target.Position.DistanceTo(target.CurJob.targetB.Cell);
                if (pawnDistanceToTarget > 3f || pawnDistanceToWalkCell > targetDistanceToWalkCell)
                {
                    if ((int)this.job.locomotionUrgency > 1 && target.CurJob.locomotionUrgency != (LocomotionUrgency)(this.job.locomotionUrgency - 1))
                    {
                        if ((int)target.CurJob.locomotionUrgency > 1)
                        {
                            target.CurJob.locomotionUrgency = (LocomotionUrgency)(this.job.locomotionUrgency - 1);
                        }
                    }
                }

                else if (pawnDistanceToWalkCell <= targetDistanceToWalkCell)
                {
                    target.CurJob.locomotionUrgency = LocomotionUrgency.Walk;
                    base.pawn.CurJob.locomotionUrgency = LocomotionUrgency.Walk;
                }


                if ((int)target.CurJob.locomotionUrgency > 2)
                {
                    target.CurJob.locomotionUrgency = LocomotionUrgency.Walk;
                }
            };
            toil.socialMode = RandomSocialMode.SuperActive;
			toil.defaultCompleteMode = ToilCompleteMode.Never;
			yield return toil;
		}

		public override bool IsContinuation(Job j)
		{
			return job.GetTarget(TargetIndex.A) == j.GetTarget(TargetIndex.A);
		}
	}
}

