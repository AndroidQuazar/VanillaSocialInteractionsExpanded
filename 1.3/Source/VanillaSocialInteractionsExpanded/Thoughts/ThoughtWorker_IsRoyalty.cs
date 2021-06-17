using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace VanillaSocialInteractionsExpanded
{
	public class ThoughtWorker_IsRoyalty : ThoughtWorker
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
			if (IsRoyalty(p, other) && Rand.ChanceSeeded(0.1f, p.thingIDNumber))
            {
				return true;
            }
			return false;
		}

		public bool IsRoyalty(Pawn pawn, Pawn other)
        {
			var pawnTitle = pawn.royalty.MostSeniorTitle;
			var otherTitle = other.royalty.MostSeniorTitle;
			if (otherTitle != null)
            {
				if (pawnTitle is null || otherTitle.def.seniority > pawnTitle.def.seniority)
				{
					return true;
				}
			}
			return false;
        }
	}
}
