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
	public class JobDriver_GoToTalkToSecondPawn : JobDriver_TalkToSecondPawn
	{
		public override bool TryMakePreToilReservations(bool errorOnFailed)
		{
			return true;
		}
		protected override IEnumerable<Toil> MakeNewToils()
		{
			yield return Toils_Goto.Goto(TargetIndex.A, PathEndMode.Touch);
			yield return GetTalkingToil();
		}
	}
}

