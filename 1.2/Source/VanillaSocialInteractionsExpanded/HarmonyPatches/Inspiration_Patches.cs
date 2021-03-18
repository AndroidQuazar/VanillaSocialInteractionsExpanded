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
	[HarmonyPatch(typeof(InspirationHandler), "TryStartInspiration_NewTemp")]
	public class TryStartInspiration_NewTemp_Patch
	{
		private static void Postfix(bool __result, InspirationHandler __instance, InspirationDef def, string reason = null)
		{
			if (VanillaSocialInteractionsExpandedSettings.EnableAspirations)
            {
				if (__result)
				{
					VSIE_Utils.SocialInteractionsManager.TryRegisterNewIspiration(__instance.pawn, def);
				}
			}

		}
	}

	[HarmonyPatch(typeof(InspirationHandler), "EndInspiration", new Type[] { typeof(Inspiration)})]
	public class EndInspiration_Patch
	{
		private static void Postfix(InspirationHandler __instance, Inspiration inspiration)
		{
			if (VanillaSocialInteractionsExpandedSettings.EnableAspirations)
			{
				VSIE_Utils.SocialInteractionsManager.Notify_InspirationExpired(__instance.pawn, inspiration.def);
			}
		}
	}
}
