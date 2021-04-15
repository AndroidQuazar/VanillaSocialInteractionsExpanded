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
	public class Thought_CuredMyFriend : Thought_SituationalSocial
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
					var relatedPawns = pawn.relations.DirectRelations.Where(x => x.otherPawn == latestTale.thirdPawnData.pawn);
					if (relatedPawns.Any())
                    {
						var firstRelation = relatedPawns.OrderBy(y => y.def.opinionOffset).FirstOrDefault();
						var genderSpecificLabel = firstRelation.def.GetGenderSpecificLabel(latestTale.firstPawnData.pawn);
						return base.CurStage.label.Formatted(genderSpecificLabel, latestTale.secondPawnData.pawn.Named("SECONDPAWN"), latestTale.thirdPawnData.pawn.Named("THIRDPAWN"), pawn.Named("PAWN"));
					}
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
				try
                {
					if (!VSIE_Utils.HaveNoticedTale(pawn, tale))
					{
						return false;
					}
					return otherPawn == tale.secondPawnData.pawn && OpinionOf(tale.thirdPawnData.pawn) >= 20;
				}
				catch (Exception ex)
                {
					Log.Error("Error: " + ex);
					return false;
                }
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
