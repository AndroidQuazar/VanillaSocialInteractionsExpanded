using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace VanillaSocialInteractionsExpanded
{
	public class ThoughtWorker_TriplePawn_Situation : ThoughtWorker
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
                if (!VSIE_Utils.HaveNoticedTale(p, t))
                {
                    return false;
                }
                if (t.firstPawnData is null) Log.Error(t + " hasn't firstPawnData, this shouldn't happen.");
                if (t.secondPawnData is null) Log.Error(t + " hasn't secondPawnData, this shouldn't happen.");
                return p == t.firstPawnData.pawn && other == t.secondPawnData.pawn;
            };

            var tale = VSIE_Utils.GetLatestTriplePawnTale(this.def.taleDef, validator);
            if (tale != null)
            {
                return true;
            }
            return false;
        }
	}
}
