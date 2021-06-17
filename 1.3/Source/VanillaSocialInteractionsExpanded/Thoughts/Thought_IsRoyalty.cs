using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace VanillaSocialInteractionsExpanded
{
	public class Thought_IsRoyalty : Thought_SituationalSocial
	{
		public override string LabelCap
		{
			get
			{
				return base.CurStage.label.Formatted(pawn.Named("PAWN"), otherPawn.Named("OTHERPAWN")).CapitalizeFirst();
			}
		}
	}
}
