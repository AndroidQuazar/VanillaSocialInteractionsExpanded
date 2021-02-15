using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;
using Verse.Grammar;

namespace VanillaSocialInteractionsExpanded
{
	public class Tale_TriplePawn : Tale
	{
		public TaleData_Pawn firstPawnData;

		public TaleData_Pawn secondPawnData;

		public TaleData_Pawn thirdPawnData;
		public override Pawn DominantPawn => firstPawnData.pawn;

		public override string ShortSummary
		{
			get
			{
				string text = (string)(def.LabelCap + ": ") + firstPawnData.name;
				if (secondPawnData != null)
				{
					text = text + ", " + secondPawnData.name;
				}
				if (thirdPawnData != null)
				{
					text = text + ", " + thirdPawnData.name;
				}
				return text;
			}
		}

		public Tale_TriplePawn()
		{

		}

		public Tale_TriplePawn(Pawn firstPawn, Pawn secondPawn, Pawn thirdPawn)
		{
			firstPawnData = TaleData_Pawn.GenerateFrom(firstPawn);
			if (secondPawn != null)
			{
				secondPawnData = TaleData_Pawn.GenerateFrom(secondPawn);
			}
			if (thirdPawn != null)
			{
				thirdPawnData = TaleData_Pawn.GenerateFrom(thirdPawn);
			}
			if (firstPawn.SpawnedOrAnyParentSpawned)
			{
				surroundings = TaleData_Surroundings.GenerateFrom(firstPawn.PositionHeld, firstPawn.MapHeld);
			}
		}

		public override bool Concerns(Thing th)
		{
			if (secondPawnData != null && secondPawnData.pawn == th)
			{
				return true;
			}
			if (thirdPawnData != null && thirdPawnData.pawn == th)
			{
				return true;
			}
			if (!base.Concerns(th))
			{
				return firstPawnData.pawn == th;
			}
			return true;
		}

		public override void Notify_FactionRemoved(Faction faction)
		{
			if (firstPawnData.faction == faction)
			{
				firstPawnData.faction = null;
			}
			if (secondPawnData != null && secondPawnData.faction == faction)
			{
				secondPawnData.faction = null;
			}
			if (thirdPawnData != null && thirdPawnData.faction == faction)
			{
				thirdPawnData.faction = null;
			}
		}

		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Deep.Look(ref firstPawnData, "firstPawnData");
			Scribe_Deep.Look(ref secondPawnData, "secondPawnData");
			Scribe_Deep.Look(ref thirdPawnData, "thirdPawnData");

		}

		protected override IEnumerable<Rule> SpecialTextGenerationRules()
		{
			if (def.firstPawnSymbol.NullOrEmpty() || def.secondPawnSymbol.NullOrEmpty())
			{
				Log.Error(string.Concat(def, " uses DoublePawn tale class but firstPawnSymbol and secondPawnSymbol are not both set"));
			}
			foreach (Rule rule in firstPawnData.GetRules("ANYPAWN"))
			{
				yield return rule;
			}
			foreach (Rule rule2 in firstPawnData.GetRules(def.firstPawnSymbol))
			{
				yield return rule2;
			}
			if (secondPawnData != null)
			{
				foreach (Rule rule3 in firstPawnData.GetRules("ANYPAWN"))
				{
					yield return rule3;
				}
				foreach (Rule rule4 in secondPawnData.GetRules(def.secondPawnSymbol))
				{
					yield return rule4;
				}
			}
			if (thirdPawnData != null)
			{
				foreach (Rule rule3 in firstPawnData.GetRules("ANYPAWN"))
				{
					yield return rule3;
				}
				foreach (Rule rule4 in thirdPawnData.GetRules(def.secondPawnSymbol))
				{
					yield return rule4;
				}
			}
		}

		public override void GenerateTestData()
		{
			base.GenerateTestData();
			firstPawnData = TaleData_Pawn.GenerateRandom();
			secondPawnData = TaleData_Pawn.GenerateRandom();
			thirdPawnData = TaleData_Pawn.GenerateRandom();
		}
	}
}