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
					if (___pawn.ideo != null)
					{
						var spouses = ___pawn.GetSpouses(false);
						if (___pawn.gender == Gender.Female)
                        {
							var spouseCount_Female_MaxTwo = DefDatabase<PreceptDef>.GetNamedSilentFail("SpouseCount_Female_MaxTwo");
							if (spouseCount_Female_MaxTwo != null && ___pawn.Ideo.HasPrecept(spouseCount_Female_MaxTwo) && spouses.Count < 2)
                            {
								return;
                            }

							var spouseCount_Female_MaxThree = DefDatabase<PreceptDef>.GetNamedSilentFail("SpouseCount_Female_MaxThree");
							if (spouseCount_Female_MaxThree != null && ___pawn.Ideo.HasPrecept(spouseCount_Female_MaxThree) && spouses.Count < 3)
							{
								return;
							}

							var spouseCount_Female_MaxFour = DefDatabase<PreceptDef>.GetNamedSilentFail("SpouseCount_Female_MaxFour");
							if (spouseCount_Female_MaxFour != null && ___pawn.Ideo.HasPrecept(spouseCount_Female_MaxFour) && spouses.Count < 4)
							{
								return;
							}

							var spouseCount_Female_Unlimited = DefDatabase<PreceptDef>.GetNamedSilentFail("SpouseCount_Female_Unlimited");
							if (spouseCount_Female_Unlimited != null && ___pawn.Ideo.HasPrecept(spouseCount_Female_Unlimited))
							{
								return;
							}
						}

						else if (___pawn.gender == Gender.Male)
                        {
							var spouseCount_Male_MaxTwo = DefDatabase<PreceptDef>.GetNamedSilentFail("SpouseCount_Male_MaxTwo");
							if (spouseCount_Male_MaxTwo != null && ___pawn.Ideo.HasPrecept(spouseCount_Male_MaxTwo) && spouses.Count < 2)
							{
								return;
							}

							var spouseCount_Male_MaxThree = DefDatabase<PreceptDef>.GetNamedSilentFail("SpouseCount_Male_MaxThree");
							if (spouseCount_Male_MaxThree != null && ___pawn.Ideo.HasPrecept(spouseCount_Male_MaxThree) && spouses.Count < 3)
							{
								return;
							}

							var spouseCount_Male_MaxFour = DefDatabase<PreceptDef>.GetNamedSilentFail("SpouseCount_Male_MaxFour");
							if (spouseCount_Male_MaxFour != null && ___pawn.Ideo.HasPrecept(spouseCount_Male_MaxFour) && spouses.Count < 4)
							{
								return;
							}

							var spouseCount_Male_Unlimited = DefDatabase<PreceptDef>.GetNamedSilentFail("SpouseCount_Male_Unlimited");
							if (spouseCount_Male_Unlimited != null && ___pawn.Ideo.HasPrecept(spouseCount_Male_Unlimited))
							{
								return;
							}
						}
                    }

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
