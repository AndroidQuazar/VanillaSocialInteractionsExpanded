using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace VanillaSocialInteractionsExpanded
{
    public static class VSIE_Utils
    {
		public static Tale_AntagonistAndVictimPawns GetLatestAntagonistAndVictimTale(TaleDef def, Predicate<Tale_AntagonistAndVictimPawns> predicate)
		{
			Tale_AntagonistAndVictimPawns tale = null;
			int num = 0;
			for (int i = 0; i < Find.TaleManager.AllTalesListForReading.Count; i++)
			{
				var latestTale = Find.TaleManager.AllTalesListForReading[i];
				if (latestTale.def == def && latestTale is Tale_AntagonistAndVictimPawns tale_AntagonistAndVictimPawns && predicate(tale_AntagonistAndVictimPawns) && (tale == null || latestTale.AgeTicks < num))
				{
					tale = tale_AntagonistAndVictimPawns;
					num = latestTale.AgeTicks;
				}
			}
			return tale;
		}

		public static Tale GetLatestTale(TaleDef def, Predicate<Tale> predicate)
		{
			Tale tale = null;
			int num = 0;
			var tales = Find.TaleManager.AllTalesListForReading;
			for (int i = 0; i < tales.Count; i++)
			{
				if (tales[i].def == def && predicate(tales[i]) && (tale == null || tales[i].AgeTicks < num))
				{
					tale = tales[i];
					num = tales[i].AgeTicks;
				}
			}
			return tale;
		}
	}
}
