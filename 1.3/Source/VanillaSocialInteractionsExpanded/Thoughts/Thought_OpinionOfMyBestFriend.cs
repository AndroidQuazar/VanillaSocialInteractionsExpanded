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
	public class Thought_OpinionOfMyBestFriend : Thought_Situational
	{
		public override string LabelCap
		{
			get
			{

				DirectPawnRelation directPawnRelation = pawn.relations.DirectRelations.FirstOrDefault(x => x.def == VSIE_DefOf.VSIE_BestFriend);
				string text = base.CurStage.label.Formatted(directPawnRelation.def.GetGenderSpecificLabel(directPawnRelation.otherPawn), directPawnRelation.otherPawn.LabelShort, directPawnRelation.otherPawn).CapitalizeFirst();
				if (def.Worker != null)
				{
					text = def.Worker.PostProcessLabel(pawn, text);
				}
				return text;
			}
		}

		protected override float BaseMoodOffset
		{
			get
			{
				DirectPawnRelation directPawnRelation = pawn.relations.DirectRelations.FirstOrDefault(x => x.def == VSIE_DefOf.VSIE_BestFriend);
				float num = 0.05f * (float)pawn.relations.OpinionOf(directPawnRelation.otherPawn);
				if (num < 0f)
				{
					return Mathf.Min(num, -1f);
				}
				return Mathf.Max(num, 1f);
			}
		}
	}
}
