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
					TaleRecorder.RecordTale(VSIE_DefOf.VSIE_AttendedMyWedding, pawn, __instance.firstPawn);
				}
				else if (pawn != __instance.secondPawn)
                {
					TaleRecorder.RecordTale(VSIE_DefOf.VSIE_AttendedMyWedding, pawn, __instance.secondPawn);
				}
			}
			foreach (var pawn in __instance.lord.Map.mapPawns.AllPawns.Where(x => x.IsColonist))
            {
				if (!attendedWedding.Contains(pawn))
                {
					if (attendedWedding.Contains(__instance.firstPawn))
                    {
						TaleRecorder.RecordTale(VSIE_DefOf.VSIE_DidNotAttendWedding, pawn, __instance.firstPawn);
					}
					if (attendedWedding.Contains(__instance.secondPawn))
					{
						TaleRecorder.RecordTale(VSIE_DefOf.VSIE_DidNotAttendWedding, pawn, __instance.secondPawn);
					}
				}
            }
		}
	}
}
