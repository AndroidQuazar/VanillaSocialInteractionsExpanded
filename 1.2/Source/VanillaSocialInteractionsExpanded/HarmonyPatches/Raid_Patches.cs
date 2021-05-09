using HarmonyLib;
using RimWorld;
using RimWorld.Planet;
using RimWorld.QuestGen;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
using Verse.AI.Group;
using static Verse.DamageWorker;

namespace VanillaSocialInteractionsExpanded
{
    [StaticConstructorOnStartup]
    public static class RaidPatches
    {
        static RaidPatches()
        {
            if (VanillaSocialInteractionsExpandedSettings.EnableUnitedWeStand)
            {
                var postfix = typeof(RaidPatches).GetMethod("RaidGroupChecker");
                var baseType = typeof(PawnsArrivalModeWorker);
                var types = baseType.AllSubclassesNonAbstract();
                foreach (Type cur in types)
                {
                    var method = cur.GetMethod("Arrive");
                    try
                    {
                        HarmonyInit.harmony.Patch(method, null, new HarmonyMethod(postfix));
                    }
                    catch (Exception ex)
                    {
                        Log.Error("Error patching " + cur + " - " + method);
                    }
                }
            }
        }

        public static void RaidGroupChecker(List<Pawn> pawns, IncidentParms parms)
        {
            try
            {
                if (VanillaSocialInteractionsExpandedSettings.EnableUnitedWeStand)
                {
                    if (pawns != null && pawns.Any() && parms != null && parms.target is Map map)
                    {
                        var gameComp = Current.Game.GetComponent<SocialInteractionsManager>();
                        var raidGroup = new RaidGroup();
                        if (parms.faction != null)
                        {
                            raidGroup.faction = parms.faction;
                        }
                        else
                        {
                            raidGroup.faction = pawns.First().Faction;
                        }
                        raidGroup.raiders = pawns.ToHashSet();
                        raidGroup.defenders = map.mapPawns.AllPawnsSpawned.Where(x => x != null && x.RaceProps.Humanlike && !x.Dead && !x.Fogged() && !x.IsPrisoner && x.Faction != null
                            && (x.Faction == Faction.OfPlayer || !x.HostileTo(Faction.OfPlayer)) && x.HostileTo(raidGroup.faction)).ToHashSet();

                        raidGroup.initTime = Find.TickManager.TicksGame;
                        if (gameComp.raidGroups is null)
                        {
                            gameComp.raidGroups = new List<RaidGroup>();
                        }
                        gameComp.raidGroups.Add(raidGroup);
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error("Error in VSIE: " + ex);
            }
        }
    }

    [HarmonyPatch(typeof(Lord), "AddPawn")]
    public static class Patch_AddPawn
    {
        public static void Postfix(Lord __instance, Pawn p)
        {
            if (VanillaSocialInteractionsExpandedSettings.EnableUnitedWeStand)
            {
                var gameComp = Current.Game.GetComponent<SocialInteractionsManager>();
                if (gameComp.raidGroups != null)
                {
                    foreach (var rg in gameComp.raidGroups)
                    {
                        if (rg.raiders.Contains(p))
                        {
                            rg.raiderLords.Add(__instance);
                        }
                    }
                }
            }
        }
    }

    [HarmonyPatch(typeof(Lord))]
    [HarmonyPatch("Cleanup")]
    public static class Patch_Cleanup
    {
        public static void Prefix(Lord __instance)
        {
            if (VanillaSocialInteractionsExpandedSettings.EnableUnitedWeStand)
            {
                VSIE_Utils.SocialInteractionsManager.TryAssignThoughtsAfterRaid(__instance);
            }
        }
    }

    [HarmonyPatch(typeof(Transition))]
    [HarmonyPatch("Execute")]
    public static class Patch_Execute
    {
        public static void Prefix(Transition __instance, Lord lord)
        {
            if (VanillaSocialInteractionsExpandedSettings.EnableUnitedWeStand)
            {
                if (__instance.canMoveToSameState || __instance.target != lord.CurLordToil)
                {
                    for (int i = 0; i < __instance.preActions.Count; i++)
                    {
                        if (__instance.preActions[i] is TransitionAction_Message transitionAction)
                        {
                            if (transitionAction.message == "MessageRaidersGivenUpLeaving".Translate(lord.faction.def.pawnsPlural.CapitalizeFirst(), lord.faction.Name)
                                || transitionAction.message == "MessageFightersFleeing".Translate(lord.faction.def.pawnsPlural.CapitalizeFirst(), lord.faction.Name))
                            {
                                VSIE_Utils.SocialInteractionsManager.TryAssignThoughtsAfterRaid(lord, true);
                            }
                        }
                    }
                }
            }
        }
    }
}
