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
	public class PawnRelationWorker_BestFriend : PawnRelationWorker
	{
		public override float GenerationChance(Pawn generated, Pawn other, PawnGenerationRequest request)
		{
			if (VanillaSocialInteractionsExpandedSettings.EnableBestFriend)
			{
				if (generated.relations.OpinionOf(other) >= 80 && other.relations.OpinionOf(generated) >= 80)
				{
					return 1f;
				}
			}
			return 0f;
		}

		public override void CreateRelation(Pawn generated, Pawn other, ref PawnGenerationRequest request)
		{
			generated.relations.AddDirectRelation(VSIE_DefOf.VSIE_BestFriend, other);
		}
	}
}