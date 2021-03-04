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
		public int talkDuration;
		public int maxTalkDuration;
		public override bool TryMakePreToilReservations(bool errorOnFailed)
		{
			return true;
		}
		protected override IEnumerable<Toil> MakeNewToils()
		{
			yield return GetTalkingToil();
		}

		public Toil GetTalkingToil()
        {
			Toil toil = new Toil();
			toil.initAction = delegate
			{
				maxTalkDuration = Rand.RangeInclusive(300, 400);
			};
			toil.tickAction = delegate
			{
				talkDuration++;
				Pawn pawn = this.TargetA.Pawn;
				if (pawn != null)
				{
					base.pawn.rotationTracker.FaceCell(pawn.Position);
				}
				base.pawn.GainComfortFromCellIfPossible();
				if (talkDuration >= maxTalkDuration || this.TargetA.Pawn.Position.DistanceTo(pawn.Position) > 4 || this.TargetA.Pawn.pather.MovingNow)
				{
					this.ReadyForNextToil();
				}
			};
			toil.socialMode = RandomSocialMode.SuperActive;
			toil.defaultCompleteMode = ToilCompleteMode.Never;
			toil.handlingFacing = true;
			return toil;
		}
        public override void ExposeData()
        {
            base.ExposeData();
			Scribe_Values.Look(ref talkDuration, "talkDuration");
			Scribe_Values.Look(ref maxTalkDuration, "maxTalkDuration");
		}
	}
}

