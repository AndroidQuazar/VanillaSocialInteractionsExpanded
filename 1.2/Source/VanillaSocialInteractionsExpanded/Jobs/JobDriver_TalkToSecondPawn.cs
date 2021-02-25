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
	public class JobDriver_TalkToSecondPawn : JobDriver
	{
		public override bool TryMakePreToilReservations(bool errorOnFailed)
		{
			return true;
		}
		protected override IEnumerable<Toil> MakeNewToils()
		{
			Toil toil = new Toil();
			toil.tickAction = delegate
			{
				Pawn pawn = this.TargetA.Pawn;
				if (pawn != null)
				{
					base.pawn.rotationTracker.FaceCell(pawn.Position);
				}
				base.pawn.GainComfortFromCellIfPossible();
			};
			toil.socialMode = RandomSocialMode.SuperActive;
			toil.defaultCompleteMode = ToilCompleteMode.Never;
			toil.handlingFacing = true;
			yield return toil;
		}
	}
}

