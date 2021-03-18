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
	[HarmonyPatch(typeof(JobDriver_HaulCorpseToPublicPlace), "MakeNewToils")]
	public class JobDriver_HaulCorpseToPublicPlace_MakeNewToils
	{
		private static void Postfix(ref IEnumerable<Toil> __result)
		{
			if (VanillaSocialInteractionsExpandedSettings.EnableMemories)
			{
				List<Toil> toils = __result.ToList();
				var toil = new Toil();
				toil.initAction = delegate ()
				{
					var actor = toil.actor;
					if (Rand.Chance(0.1f))
					{
						TaleRecorder.RecordTale(VSIE_DefOf.VSIE_ExposedCorpseOfMyFriend, (actor.CurJob.GetTarget(TargetIndex.A).Thing as Corpse).InnerPawn, actor);
					}
				};
				toils.Add(toil);
				__result = toils;
			}
		}
	}

	[HarmonyPatch(typeof(JobDriver_Ingest), "MakeNewToils")]
	public class JobDriver_Ingest_MakeNewToils
	{
		private static void Postfix(ref IEnumerable<Toil> __result)
		{
			if (VanillaSocialInteractionsExpandedSettings.EnableMemories)
			{
				List<Toil> toils = __result.ToList();
				var toil = new Toil();
				toil.initAction = delegate ()
				{
					var actor = toil.actor;
					if (actor.CurJob.overeat)
					{
						if (actor.CurJob.GetTarget(TargetIndex.A).Thing.def.IsDrug)
						{
							if (Rand.Chance(0.1f))
							{
								TaleRecorder.RecordTale(VSIE_DefOf.VSIE_BingedDrug, actor);
							}
						}
						else
						{
							if (Rand.Chance(0.1f))
							{
								TaleRecorder.RecordTale(VSIE_DefOf.VSIE_BingedFood, actor);
							}
						}
					}
				};
				toils.Add(toil);
				__result = toils;
			}
		}
	}

	[HarmonyPatch(typeof(JobDriver_TakeToBed), "MakeNewToils")]
	public class JobDriver_TakeToBed_MakeNewToils
	{
		private static void Postfix(ref IEnumerable<Toil> __result)
		{
			if (VanillaSocialInteractionsExpandedSettings.EnableMemories)
			{
				List<Toil> toils = __result.ToList();
				var toil = new Toil();
				toil.initAction = delegate ()
				{
					var actor = toil.actor;
					var takee = actor.CurJob.targetA.Thing as Pawn;
					if (actor.CurJobDef.makeTargetPrisoner && takee.IsPrisonerOfColony)
					{
						if (Rand.Chance(0.1f))
						{
							TaleRecorder.RecordTale(VSIE_DefOf.VSIE_ArrestedMe, takee, actor);
						}
					}
				};
				toils.Add(toil);
				__result = toils;
			}
		}
	}

	[HarmonyPatch(typeof(JobDriver_Resurrect), "MakeNewToils")]
	public class JobDriver_Resurrect_MakeNewToils
	{
		private static void Postfix(ref IEnumerable<Toil> __result)
		{
			List<Toil> toils = __result.ToList();
			var toil = new Toil();
			toil.initAction = delegate ()
			{
				var actor = toil.actor;
				var resurrected = (actor.CurJob.targetA.Thing as Corpse).InnerPawn;
				if (VanillaSocialInteractionsExpandedSettings.EnableMemories)
				{
					if (Rand.Chance(0.1f))
					{
						TaleRecorder.RecordTale(VSIE_DefOf.VSIE_ResurrectedMe, resurrected, actor);
					}
				}
				if (VanillaSocialInteractionsExpandedSettings.EnableObtainingNewTraits)
				{
					if (Rand.Chance(0.1f))
					{
						VSIE_Utils.TryDevelopNewTrait(resurrected, "VSIE.TraitChangePawnResurrected");
					}
				}
			};
			toils.Insert(toils.Count - 1, toil);
			__result = toils;
		}
	}

	[HarmonyPatch(typeof(JobDriver_Lovin), "MakeNewToils")]
	public class JobDriver_Lovin_MakeNewToils
	{
		private static void Postfix(ref IEnumerable<Toil> __result)
		{
			if (VanillaSocialInteractionsExpandedSettings.EnableAspirations)
			{
				List<Toil> toils = __result.ToList();
				var toil = new Toil();
				toil.initAction = delegate ()
				{
					var actor = toil.actor;
					if (actor.InspirationDef == VSIE_DefOf.VSIE_Flirting_Frenzy)
					{
						VSIE_Utils.SocialInteractionsManager.Notify_AspirationProgress(actor);
					}
				};
				toils.Insert(toils.Count - 1, toil);
				__result = toils;
			}
		}
	}

	[HarmonyPatch(typeof(JobDriver_BestowingCeremony), "MakeNewToils")]
	public class JobDriver_BestowingCeremony_MakeNewToils
	{
		private static void Postfix(ref IEnumerable<Toil> __result)
		{
			if (VanillaSocialInteractionsExpandedSettings.EnableObtainingNewTraits)
			{
				List<Toil> toils = __result.ToList();
				var toil = new Toil();
				toil.initAction = delegate ()
				{
					var actor = toil.actor;
					if (Rand.Chance(0.1f))
					{
						VSIE_Utils.TryDevelopNewTrait(actor, "VSIE.BestowingCeremony");
					}
				};
				toils.Insert(toils.Count - 1, toil);
				__result = toils;
			}
		}
	}

	[HarmonyPatch(typeof(JoyUtility), "JoyTickCheckEnd")]
	public class JoyUtility_JoyTickCheckEnd
	{
		private static void Prefix(Pawn pawn, ref float extraJoyGainFactor)
		{
			if (pawn.InspirationDef == VSIE_DefOf.VSIE_Party_Frenzy && pawn.CurJobDef == JobDefOf.SocialRelax)
            {
				extraJoyGainFactor *= 2f;
			}
		}
	}


	[HarmonyPatch(typeof(ToilEffects), "WithEffect", new Type[] {typeof(Toil), typeof(Func<EffecterDef>), typeof(Func<LocalTargetInfo>)})]
	[HarmonyPatch()]
	public static class WithEffect_Patch
	{
		private static void Postfix(this Toil __result)
		{
			var socialManager = VSIE_Utils.SocialInteractionsManager;
			if (VanillaSocialInteractionsExpandedSettings.EnableDiscord)
			{
				__result.AddPreTickAction(delegate
				{
					var actor = __result.actor;
					if (actor.RaceProps.Humanlike && actor.Faction != null)
					{
						socialManager.WorkerTick(actor);
					}
				});
			}
			if (VanillaSocialInteractionsExpandedSettings.EnableAspirations)
			{
				__result.AddFinishAction(delegate
				{
					var actor = __result.actor;
					if (actor.InspirationDef == VSIE_DefOf.Frenzy_Work && actor.mindState.lastJobTag == JobTag.MiscWork)
					{
						socialManager.Notify_AspirationProgress(actor);
					}
				});
			}
		}
	}
	[HarmonyPatch(typeof(ToilEffects))]
	[HarmonyPatch("WithProgressBar")]
	public static class WithProgressBar_Patch
	{
		private static void Postfix(this Toil __result)
		{
			var socialManager = VSIE_Utils.SocialInteractionsManager;
			if (VanillaSocialInteractionsExpandedSettings.EnableDiscord)
			{
				__result.AddPreTickAction(delegate
				{
					var actor = __result.actor;
					if (actor.RaceProps.Humanlike && actor.Faction != null)
					{
						socialManager.WorkerTick(actor);
					}
				});
			}
			if (VanillaSocialInteractionsExpandedSettings.EnableAspirations)
			{
				__result.AddFinishAction(delegate
				{
					var actor = __result.actor;
					if (actor.InspirationDef == VSIE_DefOf.Frenzy_Work && actor.mindState.lastJobTag == JobTag.MiscWork)
					{
						socialManager.Notify_AspirationProgress(actor);
					}
				});
			}
		}
	}
}
