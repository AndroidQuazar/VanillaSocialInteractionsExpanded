using RimWorld;
using RimWorld.Planet;
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
	public class JobDriver_GotoTalking : JobDriver
	{
		public override bool TryMakePreToilReservations(bool errorOnFailed)
		{
			if (TargetB.Pawn.Position.DistanceTo(TargetA.Cell) < 7 || TargetB.Pawn.CurJob?.targetA.Cell.DistanceTo(TargetA.Cell) < 7)
			{
				pawn.Map.pawnDestinationReservationManager.Reserve(pawn, job, job.targetA.Cell);
			}
			return true;
		}
		protected override IEnumerable<Toil> MakeNewToils()
		{
			Toil toil = new Toil();
			toil.initAction = delegate
			{
				if (TargetB.Pawn.Position.DistanceTo(TargetA.Cell) < 7 || TargetB.Pawn.CurJob?.targetA.Cell.DistanceTo(TargetA.Cell) < 7)
                {
					toil.actor.pather.StartPath(TargetA.Cell, PathEndMode.OnCell);
                }
				else
                {
					toil.actor.pather.StartPath(TargetB.Pawn, PathEndMode.Touch);
				}
			};
			toil.defaultCompleteMode = ToilCompleteMode.PatherArrival;
			toil.socialMode = RandomSocialMode.SuperActive;
			yield return toil;
		}
	}
}

