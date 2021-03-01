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
			pawn.Map.pawnDestinationReservationManager.Reserve(pawn, job, job.targetA.Cell);
			return true;
		}
		protected override IEnumerable<Toil> MakeNewToils()
		{
			Toil toil = Toils_Goto.GotoCell(TargetB.Cell, PathEndMode.OnCell);
			toil.tickAction = delegate
			{

			};
			toil.socialMode = RandomSocialMode.SuperActive;
			yield return toil;
		}
	}
}

