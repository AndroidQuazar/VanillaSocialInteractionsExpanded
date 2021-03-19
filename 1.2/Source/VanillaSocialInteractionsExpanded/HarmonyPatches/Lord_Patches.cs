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
	[HarmonyPatch(typeof(LordJob_Joinable_MarriageCeremony), "AddAttendedWeddingThoughts")]
	public class AddAttendedWeddingThoughts_Patch
	{
		private static void Postfix(LordJob_Joinable_MarriageCeremony __instance)
        {
			if (VanillaSocialInteractionsExpandedSettings.EnableMemories)
			{
				List<Pawn> attendedWedding = new List<Pawn>();
				List<Pawn> ownedPawns = __instance.lord.ownedPawns;
				for (int i = 0; i < ownedPawns.Count; i++)
				{
					if (__instance.firstPawn.Position.InHorDistOf(ownedPawns[i].Position, 18f) || __instance.secondPawn.Position.InHorDistOf(ownedPawns[i].Position, 18f))
					{
						attendedWedding.Add(ownedPawns[i]);
					}
				}

				foreach (var pawn in attendedWedding)
				{
					if (pawn != __instance.firstPawn)
					{
						if (Rand.Chance(0.1f))
						{
							TaleRecorder.RecordTale(VSIE_DefOf.VSIE_AttendedMyWedding, __instance.firstPawn, pawn);
						}
					}
					else if (pawn != __instance.secondPawn)
					{
						if (Rand.Chance(0.1f))
						{
							TaleRecorder.RecordTale(VSIE_DefOf.VSIE_AttendedMyWedding, __instance.secondPawn, pawn);
						}
					}
				}
				foreach (var pawn in __instance.lord.Map.mapPawns.AllPawns.Where(x => x.IsColonist))
				{
					if (!attendedWedding.Contains(pawn))
					{
						if (attendedWedding.Contains(__instance.firstPawn))
						{
							if (Rand.Chance(0.1f))
							{
								TaleRecorder.RecordTale(VSIE_DefOf.VSIE_DidNotAttendWedding, __instance.firstPawn, pawn);
							}
						}
						if (attendedWedding.Contains(__instance.secondPawn))
						{
							if (Rand.Chance(0.1f))
							{
								TaleRecorder.RecordTale(VSIE_DefOf.VSIE_DidNotAttendWedding, __instance.secondPawn, pawn);
							}
						}
					}
				}
			}
		}
	}
}
