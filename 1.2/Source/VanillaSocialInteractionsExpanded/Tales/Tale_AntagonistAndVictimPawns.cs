using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using Verse.Grammar;

namespace VanillaSocialInteractionsExpanded
{
	public class Tale_AntagonistAndVictimPawns : Tale
	{
		public TaleData_Pawn antagonistData;

		public TaleData_Pawn victimData;

		public override string ShortSummary => (string)(def.LabelCap + ": ") + antagonistData.name;

		public Tale_AntagonistAndVictimPawns()
		{
		}

		public Tale_AntagonistAndVictimPawns(Pawn antagonist, Pawn victim)
		{
			antagonistData = TaleData_Pawn.GenerateFrom(antagonist);
			if (antagonist.SpawnedOrAnyParentSpawned)
			{
				surroundings = TaleData_Surroundings.GenerateFrom(antagonist.PositionHeld, antagonist.MapHeld);
			}
			victimData = TaleData_Pawn.GenerateFrom(victim);
		}

		public override bool Concerns(Thing th)
		{
			if (!base.Concerns(th))
			{
				return antagonistData.pawn == th || victimData.pawn == th;
			}
			return true;
		}

		public override void Notify_FactionRemoved(Faction faction)
		{
			if (antagonistData.faction == faction)
			{
				antagonistData.faction = null;
			}
		}

		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Deep.Look(ref antagonistData, "antagonistData");
			Scribe_Deep.Look(ref victimData, "victimData");
		}

		protected override IEnumerable<Rule> SpecialTextGenerationRules()
		{
			foreach (Rule rule in antagonistData.GetRules("ANYPAWN"))
			{
				yield return rule;
			}
			foreach (Rule rule2 in antagonistData.GetRules("PAWN"))
			{
				yield return rule2;
			}

			foreach (Rule rule in victimData.GetRules("ANYPAWN"))
			{
				yield return rule;
			}
			foreach (Rule rule2 in victimData.GetRules("PAWN"))
			{
				yield return rule2;
			}
		}

		public override void GenerateTestData()
		{
			base.GenerateTestData();
			antagonistData = TaleData_Pawn.GenerateRandom();
			victimData = TaleData_Pawn.GenerateRandom();
		}
	}
}