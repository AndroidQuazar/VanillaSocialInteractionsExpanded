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
	[HarmonyPatch(typeof(TradeDeal), "TryExecute", new Type[]
	{
		typeof(bool),
	}, new ArgumentType[] { ArgumentType.Out})]
	public static class TryExecute_Patch
	{
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            FieldInfo playerNegotiatorInfo = AccessTools.Field(typeof(TradeSession), "playerNegotiator");
            foreach (var ins in instructions)
            {
                if (ins.OperandIs(playerNegotiatorInfo))
                {
                    yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(TryExecute_Patch), "Notify_Progress", null, null));
                    yield return ins;
                }
                else
                {
                    yield return ins;
                }
            }
            yield break;
        }

        public static void Notify_Progress()
        {
            if (TradeSession.playerNegotiator.InspirationDef == VSIE_DefOf.Inspired_Trade)
            {
                VSIE_Utils.SocialInteractionsManager.Notify_AspirationProgress(TradeSession.playerNegotiator);
            }
        }
    }
    [HarmonyPatch(typeof(CaravanExitMapUtility), "ExitMapAndCreateCaravan", new Type[]
    {
        typeof(IEnumerable<Pawn>),
        typeof(Faction),
        typeof(int),
        typeof(int),
        typeof(int),
        typeof(bool)
    })]
    public static class ExitMapAndCreateCaravan_Patch
    {
        private static void Postfix(Caravan __result, IEnumerable<Pawn> pawns, Faction faction, int exitFromTile, int directionTile, int destinationTile, bool sendMessage = true)
        {
            foreach (var pawn in __result.PawnsListForReading)
            {
                if (pawn.InspirationDef == VSIE_DefOf.Frenzy_Go)
                {
                    VSIE_Utils.SocialInteractionsManager.Notify_AspirationProgress(pawn);
                }
            }
        }
    }

    [HarmonyPatch(typeof(ResearchManager), "FinishProject")]
    public static class FinishProject_Patch
    {
        private static void Postfix(ResearchProjectDef proj, bool doCompletionDialog = false, Pawn researcher = null)
        {
            if (researcher != null && researcher.InspirationDef == VSIE_DefOf.VSIE_Inspired_Research)
            {
                VSIE_Utils.SocialInteractionsManager.Notify_AspirationProgress(researcher);
            }
        }
    }


    [HarmonyPatch(typeof(Mineable), "DestroyMined")]
    public static class DestroyMined_Patch
    {
        private static void Postfix(Pawn pawn)
        {
            if (pawn != null && pawn.InspirationDef == VSIE_DefOf.VSIE_Inspired_Mining)
            {
                VSIE_Utils.SocialInteractionsManager.Notify_AspirationProgress(pawn);
            }
        }
    }
}
