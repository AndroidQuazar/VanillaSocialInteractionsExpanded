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
	public class Thought_ButheredMyBondedPet : Thought_SituationalSocial
	{
		public override float OpinionOffset()
		{
			if (ThoughtUtility.ThoughtNullified(pawn, def))
			{
				return 0f;
			}
			Predicate<Tale_DoublePawn> validator = delegate (Tale_DoublePawn tale)
			{
				if (!VSIE_Utils.HaveNoticedTale(pawn, tale))
				{
					return false;
				}
				return tale.firstPawnData.pawn.relations.GetDirectRelation(PawnRelationDefOf.Bond, pawn) != null && tale.secondPawnData.pawn == otherPawn;
			};
			Tale latestTale = VSIE_Utils.GetLatestDoublePawnTale(def.taleDef, validator);
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
