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
	public class JobDriver_ViewArtTogether : JobDriver
	{
		private Thing ArtThing => job.GetTarget(TargetIndex.A).Thing;

		public override bool TryMakePreToilReservations(bool errorOnFailed)
		{
			if (!VSIE_Utils.GetCompanion(pawn).HasReserved(job.GetTarget(TargetIndex.A)))
            {
				return pawn.Reserve(job.GetTarget(TargetIndex.A), job, 1, -1, null, errorOnFailed);
            }
			return true;
		}

		protected void WaitTickAction()
		{
			float num = ArtThing.GetStatValue(StatDefOf.Beauty) / ArtThing.def.GetStatValueAbstract(StatDefOf.Beauty);
			float extraJoyGainFactor = (num > 0f) ? num : 0f;
			pawn.GainComfortFromCellIfPossible();
			JoyUtility.JoyTickCheckEnd(pawn, JoyTickFullJoyAction.None, extraJoyGainFactor, (Building)ArtThing);
		}

		protected override IEnumerable<Toil> MakeNewToils()
		{
			this.FailOnDestroyedNullOrForbidden(TargetIndex.A);
			yield return Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.Touch);
			Toil toil = Toils_General.Wait(job.def.joyDuration);
			toil.FailOnCannotTouch(TargetIndex.A, PathEndMode.Touch);
			toil.tickAction = delegate
			{
				WaitTickAction();
			};
			toil.AddFinishAction(delegate
			{
				JoyUtility.TryGainRecRoomThought(pawn);
			});
			toil.socialMode = RandomSocialMode.SuperActive;
			yield return toil;
		}
	}
}

