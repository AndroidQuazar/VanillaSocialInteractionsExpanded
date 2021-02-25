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
	public class JobDriver_TakeBeerToCell : JobDriver
	{
		public override bool TryMakePreToilReservations(bool errorOnFailed)
		{
			if (pawn.Reserve(job.GetTarget(TargetIndex.B), job, 1, -1, null, errorOnFailed))
			{
				return pawn.Reserve(job.GetTarget(TargetIndex.A), job, 1, -1, null, errorOnFailed);
			}
			return false;
		}
		protected override IEnumerable<Toil> MakeNewToils()
		{
			this.FailOnDestroyedOrNull(TargetIndex.A);
			this.FailOnBurningImmobile(TargetIndex.B);
			this.FailOnForbidden(TargetIndex.A);
			Toil reserveTargetA = Toils_Reserve.Reserve(TargetIndex.A);
			yield return reserveTargetA;
			Toil toilGoto = Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.ClosestTouch).FailOnSomeonePhysicallyInteracting(TargetIndex.A);
			yield return toilGoto;
			yield return Toils_Haul.StartCarryThing(TargetIndex.A, putRemainderInQueue: false, subtractNumTakenFromJobCount: true);
			Toil gotoCell = Toils_Goto.Goto(TargetIndex.B, PathEndMode.OnCell);
			yield return gotoCell;
		}
	}
}