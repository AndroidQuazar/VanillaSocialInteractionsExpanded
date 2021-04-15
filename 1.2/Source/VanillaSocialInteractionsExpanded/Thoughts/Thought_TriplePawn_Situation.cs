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
	public class Thought_TriplePawn_Situation : Thought_SituationalSocial
	{
		public override string LabelCap
		{
			get
			{
				Predicate<Tale_TriplePawn> validator = delegate (Tale_TriplePawn t)
				{
					if (!VSIE_Utils.HaveNoticedTale(pawn, t))
					{
						return false;
					}
					return pawn == t.firstPawnData.pawn && otherPawn == t.secondPawnData.pawn;
				};
				Tale_TriplePawn latestTale = VSIE_Utils.GetLatestTriplePawnTale(def.taleDef, validator);
				if (latestTale != null)
				{
					return base.CurStage.label.Formatted(pawn.Named("PAWN"), otherPawn.Named("SECONDPAWN"), latestTale.thirdPawnData.pawn.Named("THIRDPAWN")).CapitalizeFirst();
				}
				return base.LabelCap;
			}

		}
		public override float OpinionOffset()
		{
			if (ThoughtUtility.ThoughtNullified(pawn, def))
			{
				return 0f;
			}
			Predicate<Tale_TriplePawn> validator = delegate (Tale_TriplePawn t)
			{
				return pawn == t.firstPawnData.pawn && otherPawn == t.secondPawnData.pawn;
			};
			Tale_TriplePawn latestTale = VSIE_Utils.GetLatestTriplePawnTale(def.taleDef, validator);

			if (latestTale != null)
			{
				float num = 1f;
				if (latestTale.def.type == TaleType.Expirable)
				{
					float value = (float)latestTale.AgeTicks / (latestTale.def.expireDays * 60000f);
					num = Mathf.InverseLerp(1f, def.lerpOpinionToZeroAfterDurationPct, value);
				}
				return base.CurStage.baseOpinionOffset * num;
			}
			return 0f;
		}
	}
}
