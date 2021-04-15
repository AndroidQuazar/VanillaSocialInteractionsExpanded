using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace VanillaSocialInteractionsExpanded
{
	public class ThoughtWorker_ButheredMyBondedPet : ThoughtWorker
	{
        public override string PostProcessLabel(Pawn p, string label)
        {
			Predicate<Tale_DoublePawn> validator = delegate (Tale_DoublePawn t)
			{
				if (!VSIE_Utils.HaveNoticedTale(p, t))
				{
					return false;
				}
				return t.firstPawnData.pawn.relations.GetDirectRelation(PawnRelationDefOf.Bond, p) != null;
			};
			var tale = VSIE_Utils.GetLatestDoublePawnTale(def.taleDef, validator);
			if (tale != null)
			{
				return label.Formatted(tale.secondPawnData.pawn.Named("BUTCHER"), tale.firstPawnData.pawn.Named("VICTIM"), p.Named("PAWN"));
			}
			return base.PostProcessLabel(p, label);
		}
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
			Predicate<Tale_DoublePawn> validator = delegate (Tale_DoublePawn t)
			{
				if (!VSIE_Utils.HaveNoticedTale(p, t))
				{
					return false;
				}
				return t.firstPawnData.pawn.relations.GetDirectRelation(PawnRelationDefOf.Bond, p) != null && t.secondPawnData.pawn == other;
			};
			var tale = VSIE_Utils.GetLatestDoublePawnTale(def.taleDef, validator);
			if (tale != null)
            {
				return true;
            }
			return false;
		}
	}
}
