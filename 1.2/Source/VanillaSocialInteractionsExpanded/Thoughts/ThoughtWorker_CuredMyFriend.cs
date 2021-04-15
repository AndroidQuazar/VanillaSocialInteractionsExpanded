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
	public class ThoughtWorker_CuredMyFriend : ThoughtWorker
	{
        protected override ThoughtState CurrentSocialStateInternal(Pawn p, Pawn other)
        {
            if (!other.RaceProps.Humanlike)
            {
                return false;
            }
            if (!RelationsUtility.PawnsKnowEachOther(p, other))
            {
                return false;
            }

            Predicate<Tale_TriplePawn> validator = delegate (Tale_TriplePawn t)
            {
				try
                {
					if (!VSIE_Utils.HaveNoticedTale(p, t))
					{
						return false;
                    }
					if (t.firstPawnData is null) Log.Error(t + " hasn't firstPawnData, this shouldn't happen.");
					if (t.secondPawnData is null) Log.Error(t + " hasn't secondPawnData, this shouldn't happen.");
					return p == t.firstPawnData.pawn && other == t.secondPawnData.pawn && OpinionOf(p, t.thirdPawnData.pawn) >= 20;
				}
				catch (Exception ex)
                {
					Log.Error("Error: " + ex);
					return false;
                }
            };
            var tale = VSIE_Utils.GetLatestTriplePawnTale(this.def.taleDef, validator);
            if (tale != null)
            {
                return true;
            }
            return false;
        }

		public int OpinionOf(Pawn pawn, Pawn other)
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
