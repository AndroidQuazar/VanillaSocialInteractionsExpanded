using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace VanillaSocialInteractionsExpanded
{
	[HarmonyPatch(typeof(Corpse), "ButcherProducts")]
	public class Patch_ButcherProducts
	{
		private static void Prefix(Corpse __instance, Pawn butcher, float efficiency)
		{
			if (VanillaSocialInteractionsExpandedSettings.EnableMemories)
            {
				Pawn p = __instance.InnerPawn;
				if (p.relations != null)
				{
					List<DirectPawnRelation> directRelations = p.relations.DirectRelations;
					for (int i = 0; i < directRelations.Count; i++)
					{
						DirectPawnRelation directPawnRelation = directRelations[i];
						Pawn otherPawn = directPawnRelation.otherPawn;
						if (directPawnRelation.def == PawnRelationDefOf.Bond && !otherPawn.Dead && otherPawn.Spawned && otherPawn.Faction == Faction.OfPlayer)
						{
							if (Rand.Chance(0.1f))
							{
								TaleRecorder.RecordTale(VSIE_DefOf.VSIE_BondedPetButchered, p, butcher);
							}
						}
					}
				}
			}
		}
	}

}
