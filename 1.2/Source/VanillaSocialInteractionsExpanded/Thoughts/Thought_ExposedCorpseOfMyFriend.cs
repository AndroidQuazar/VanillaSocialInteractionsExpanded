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
	public class Thought_ExposedCorpseOfMyFriend : Thought_SituationalSocial
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
				return otherPawn == tale.secondPawnData?.pawn && OpinionOf(tale.firstPawnData.pawn) >= 20;
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
                var value2 = base.CurStage.baseOpinionOffset * num;
                return value2;
            }
            return 0f;
        }

		public int OpinionOf(Pawn other)
		{
			if (!other.RaceProps.Humanlike || pawn == other)
			{
				return 0;
			}
			if (pawn.Dead)
			{
				return 0;
			}
			int num = 0;
			foreach (PawnRelationDef relation in pawn.GetRelations(other))
			{
				num += relation.opinionOffset;
			}
			//if (pawn.RaceProps.Humanlike && pawn.needs.mood != null)
			//{
			//	num += pawn.needs.mood.thoughts.TotalOpinionOffset(other);
			//}
			if (num != 0)
			{
				float num2 = 1f;
				List<Hediff> hediffs = pawn.health.hediffSet.hediffs;
				for (int i = 0; i < hediffs.Count; i++)
				{
					if (hediffs[i].CurStage != null)
					{
						num2 *= hediffs[i].CurStage.opinionOfOthersFactor;
					}
				}
				num = Mathf.RoundToInt((float)num * num2);
			}
			if (num > 0 && pawn.HostileTo(other))
			{
				num = 0;
			}
			return Mathf.Clamp(num, -100, 100);
		}
	}
}
