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
	public class Thought_StoleMyLover : Thought_SituationalSocial
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
					return pawn == t.firstPawnData.pawn && otherPawn == t.thirdPawnData.pawn;
				};
				Tale_TriplePawn latestTale = VSIE_Utils.GetLatestTriplePawnTale(def.taleDef, validator);
				if (latestTale != null)
				{

					return base.CurStage.label.Formatted(latestTale.secondPawnData.pawn.Named("SECONDPAWN"), latestTale.thirdPawnData.pawn.Named("THIRDPAWN"), pawn.Named("PAWN"));
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
			Predicate<Tale_TriplePawn> validator = delegate (Tale_TriplePawn tale)
			{
				if (!VSIE_Utils.HaveNoticedTale(pawn, tale))
				{
					return false;
				}
				return pawn == tale.firstPawnData.pawn && otherPawn == tale.thirdPawnData.pawn;
			};
			Tale latestTale = VSIE_Utils.GetLatestTriplePawnTale(def.taleDef, validator);
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
