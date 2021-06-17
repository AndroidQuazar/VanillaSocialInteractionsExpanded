using HarmonyLib;
using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using Verse;
using Verse.AI;

namespace VanillaSocialInteractionsExpanded
{
	[HarmonyPatch(typeof(SkillRecord), "Learn")]
	public static class Learn_Patch
	{
		private static void Prefix(Pawn ___pawn, int ___levelInt, out int __state, ref float xp, bool direct = false)
		{
			__state = ___levelInt;
			if (___pawn.InspirationDef == VSIE_DefOf.VSIE_Learning_Frenzy && xp > 0)
            {
				xp *= 2f;
            }
		}

		private static void Postfix(Pawn ___pawn, int ___levelInt, int __state, float xp, bool direct = false)
        {
			if (VanillaSocialInteractionsExpandedSettings.EnableAspirations)
			{
				if (___pawn.InspirationDef == VSIE_DefOf.VSIE_Learning_Frenzy && __state < 10 && ___levelInt >= 10)
				{
					VSIE_Utils.SocialInteractionsManager.Notify_AspirationProgress(___pawn);
				}
			}
		}
	}
}
