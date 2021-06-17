using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace VanillaSocialInteractionsExpanded
{
	public class ThoughtWorker_SocialEnvironment : ThoughtWorker
	{
		protected override ThoughtState CurrentStateInternal(Pawn p)
		{
			if (p.Faction != null && p.Faction != Faction.OfPlayer)
            {
				var averageOpinionOf = VSIE_Utils.GetAverageOpinionOf(p);
				if (averageOpinionOf >= 10)
				{
					return ThoughtState.ActiveAtStage(0);
				}
				if (averageOpinionOf < 0)
				{
					return ThoughtState.ActiveAtStage(1);
				}
			}
			return ThoughtState.Inactive;
		}
	}
}
