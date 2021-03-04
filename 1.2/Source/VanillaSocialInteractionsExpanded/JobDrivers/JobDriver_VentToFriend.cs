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
	public class JobDriver_StandAndHearVenting : JobDriver
	{
		public Pawn Venter => (Pawn)job.GetTarget(TargetIndex.A).Thing;
		public override bool TryMakePreToilReservations(bool errorOnFailed)
		{
			return true;
		}
		protected override IEnumerable<Toil> MakeNewToils()
		{
			this.FailOn(() => Venter.CurJobDef != VSIE_DefOf.VSIE_VentToFriend);
			Toil toil = new Toil();
			toil.tickAction = delegate
			{
				base.pawn.rotationTracker.FaceTarget(Venter);
				base.pawn.GainComfortFromCellIfPossible();
			};
			toil.socialMode = RandomSocialMode.Off;
			toil.defaultCompleteMode = ToilCompleteMode.Never;
			toil.handlingFacing = true;
			yield return toil;
		}
	}
	public class JobDriver_VentToFriend : JobDriver
	{
		private Pawn Friend => (Pawn)job.GetTarget(TargetIndex.A).Thing;
		private int ventDuration;
		private int curDuration;
		public override bool TryMakePreToilReservations(bool errorOnFailed)
		{
			if (!pawn.Reserve(Friend, job, 1, -1, null, errorOnFailed))
			{
				return false;
			}
			return true;
		}

		protected override IEnumerable<Toil> MakeNewToils()
		{
			this.FailOnDespawnedNullOrForbidden(TargetIndex.A);
			this.FailOn(() => !Friend.Awake());
			yield return Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.InteractionCell);

			Toil toil = new Toil();
			toil.initAction = delegate
			{
				ventDuration = new IntRange(640, 960).RandomInRange;
			};
			toil.tickAction = delegate
			{
				Pawn friend = Friend;
				pawn.rotationTracker.FaceTarget(friend);
				Map map = pawn.Map;
				if (InteractionUtility.IsGoodPositionForInteraction(pawn, friend) && pawn.Position.InHorDistOf(friend.Position, Mathf.CeilToInt(3f))
				&& (!pawn.pather.Moving || pawn.pather.nextCell.GetDoor(map) == null))
				{
					pawn.pather.StopDead();
					DoVentingTick();
				}
				else if (!pawn.pather.Moving)
				{
					IntVec3 intVec = IntVec3.Invalid;
					for (int i = 0; i < 9 && (i != 8 || !intVec.IsValid); i++)
					{
						IntVec3 intVec2 = friend.Position + GenAdj.AdjacentCellsAndInside[i];
						if (intVec2.InBounds(map) && intVec2.Walkable(map) && intVec2 != pawn.Position && InteractionUtility.IsGoodPositionForInteraction(intVec2, friend.Position, map) && pawn.CanReach(intVec2, PathEndMode.OnCell, Danger.Deadly) && (!intVec.IsValid || pawn.Position.DistanceToSquared(intVec2) < pawn.Position.DistanceToSquared(intVec)))
						{
							intVec = intVec2;
						}
					}
					if (intVec.IsValid)
					{
						pawn.pather.StartPath(intVec, PathEndMode.OnCell);
					}
					else
					{
						DoVentingTick();
					}
				}

				if (curDuration >= ventDuration)
				{
					pawn.jobs.EndCurrentJob(JobCondition.Succeeded);
				}
			};
			toil.handlingFacing = true;
			toil.socialMode = RandomSocialMode.Off;
			toil.defaultCompleteMode = ToilCompleteMode.Never;
			yield return toil;
		}

		private void DoVentingTick()
		{
			Pawn friend = Friend;
			if (friend.CurJobDef != VSIE_DefOf.VSIE_StandAndHearVenting)
			{
				Job job = JobMaker.MakeJob(VSIE_DefOf.VSIE_StandAndHearVenting, pawn);
				friend.jobs.TryTakeOrderedJob(job);
				friend.pather.StopDead();
			}
			pawn.GainComfortFromCellIfPossible();
			curDuration++;
			if (pawn.IsHashIntervalTick(320))
			{
				pawn.interactions.TryInteractWith(friend, VSIE_DefOf.VSIE_Vent);
			}
		}

		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Values.Look(ref curDuration, "curDuration");
			Scribe_Values.Look(ref ventDuration, "ventDuration");
		}
	}
}

