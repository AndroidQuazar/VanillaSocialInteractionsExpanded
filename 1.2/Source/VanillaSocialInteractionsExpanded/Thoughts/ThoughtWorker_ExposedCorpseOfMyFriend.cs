using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace VanillaSocialInteractionsExpanded
{
	public class ThoughtWorker_ExposedCorpseOfMyFriend : ThoughtWorker
	{
        public override string PostProcessLabel(Pawn p, string label)
        {
			Predicate<Tale_AntagonistAndVictimPawns> validator = delegate (Tale_AntagonistAndVictimPawns t)
			{
				return p.relations.OpinionOf(t.victimData.pawn) >= 20;
			};
			var tale = VSIE_Utils.GetLatestAntagonistAndVictimTale(def.taleDef, validator);
			if (tale != null)
			{
				return label.Formatted(tale.victimData.pawn.relations.DirectRelations.Where(x => x.otherPawn == p).OrderBy(y => y.def.opinionOffset).FirstOrDefault()
					.def.GetGenderSpecificLabel(tale.victimData.pawn), tale.antagonistData.pawn.Named("ANTAGONIST"), tale.victimData.pawn.Named("VICTIM"), p.Named("PAWN"));
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
			Predicate<Tale_AntagonistAndVictimPawns> validator = delegate (Tale_AntagonistAndVictimPawns t)
			{
				return other == t.antagonistData.pawn && p.relations.OpinionOf(t.victimData.pawn) >= 20;
			};
			var tale = VSIE_Utils.GetLatestAntagonistAndVictimTale(def.taleDef, validator);
			if (tale != null)
			{
				return true;
			}
			return false;
		}
	}
}
