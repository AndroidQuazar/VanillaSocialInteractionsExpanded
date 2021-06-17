using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;
using Verse.AI;
using Verse.Grammar;

namespace VanillaSocialInteractionsExpanded
{
	public class IncidentWorker_OneNightStand : IncidentWorker
	{
		protected override bool CanFireNowSub(IncidentParms parms)
		{
			if (!VanillaSocialInteractionsExpandedSettings.EnableOneNightStand)
			{
				return false;
			}
			if (!base.CanFireNowSub(parms))
			{
				return false;
			}
			Map map = (Map)parms.target;
			if (TryFindNewLovers(map, out var init, out var partner))
            {
				return true;
            }
			return false;
		}
		private bool TryFindNewLovers(Map map, out Pawn initiator, out Pawn partner)
        {
			var candidates = map.mapPawns.SpawnedPawnsInFaction(Faction.OfPlayer).Where(x => x.RaceProps.Humanlike);
			var lovers = GetNewLoversFrom(candidates);
			foreach (var pawn in lovers.InRandomOrder())
            {
				foreach (var otherPawn in lovers.InRandomOrder())
                {
					if (pawn != otherPawn)
					{
						var bed1 = pawn.CurrentBed();
						if (bed1 != null && bed1.SleepingSlotsCount > 1)
                        {
							initiator = otherPawn;
							partner = pawn;
							return true;
                        }
						var bed2 = otherPawn.CurrentBed();
						if (bed2 != null && bed2.SleepingSlotsCount > 1)
						{
							initiator = pawn;
							partner = otherPawn;
							return true;
						}
					}
                }
            }
			initiator = null;
			partner = null;
			return false;
		}

		private IEnumerable<Pawn> GetNewLoversFrom(IEnumerable<Pawn> candidates)
        {
			foreach (var pawn in candidates)
			{
				if (Find.TickManager.TicksGame < pawn.mindState.canLovinTick)
				{
					continue;
				}
				if (!pawn.health.capacities.CanBeAwake)
				{
					continue;
				}
				if (LovePartnerRelationUtility.HasAnyLovePartner(pawn))
				{
					continue;
				}
				yield return pawn;
			}
		}
		protected override bool TryExecuteWorker(IncidentParms parms)
		{
			if (!VanillaSocialInteractionsExpandedSettings.EnableOneNightStand)
			{
				return false;
			}
			Map map = (Map)parms.target;
			if (TryFindNewLovers(map, out Pawn initiator, out Pawn partner))
			{
				var job = JobMaker.MakeJob(VSIE_DefOf.VSIE_OneStandLovin, partner, partner.CurrentBed());
				initiator.jobs.TryTakeOrderedJob(job);
				SendStandardLetter("VSIE.OneNightStandTitle".Translate(), "VSIE.OneNightStandText".Translate(initiator.Named("INITIATOR"), partner.Named("PARTNER"))
					, LetterDefOf.PositiveEvent, parms, new List<Pawn> { initiator, partner });
				return true;
			}
			return false;
		}
	}
}