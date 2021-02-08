using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace VanillaSocialInteractionsExpanded
{
	public class ThoughtWorker_AntagonistAndVictim_Situation : ThoughtWorker
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

			Predicate<Tale_AntagonistAndVictimPawns> validator = delegate (Tale_AntagonistAndVictimPawns t)
			{
				return p == t.victimData.pawn && other == t.antagonistData.pawn;
			};

			var tale = VSIE_Utils.GetLatestAntagonistAndVictimTale(this.def.taleDef, validator);			
			if (tale != null)
            {
				return true;
            }
			return false;
		}
	}
}
