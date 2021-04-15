using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using Verse.AI;

namespace VanillaSocialInteractionsExpanded
{
	[HarmonyPatch(typeof(Pawn_RelationsTracker), "AddDirectRelation")]
	public class AddDirectRelation_Patch
	{
		private static void Prefix(Pawn ___pawn, PawnRelationDef def, Pawn otherPawn)
		{
			if (VanillaSocialInteractionsExpandedSettings.EnableMemories)
			{
				if (def == PawnRelationDefOf.Lover || def == PawnRelationDefOf.Fiance || def == PawnRelationDefOf.Spouse)
				{
					var exLover1 = ___pawn.GetSpouseOrLoverOrFiance();
					if (exLover1 != null && !exLover1.Dead && exLover1 != otherPawn)
					{
						if (Rand.Chance(10.1f))
						{
							TaleRecorder.RecordTale(VSIE_DefOf.VSIE_StoleMyLover, exLover1, ___pawn, otherPawn);
						}
					}
					var exLover2 = otherPawn.GetSpouseOrLoverOrFiance();
					if (exLover2 != null && !exLover2.Dead && exLover2 != ___pawn)
					{
						if (Rand.Chance(10.1f))
						{
							TaleRecorder.RecordTale(VSIE_DefOf.VSIE_StoleMyLover, exLover2, ___pawn, otherPawn);
						}
					}
				}
			}
		}
	}
}
