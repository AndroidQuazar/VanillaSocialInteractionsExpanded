using RimWorld;
using System;
using System.Linq;
using Verse;
using Verse.AI.Group;

namespace VanillaSocialInteractionsExpanded
{
	public class GatheringWorker_OutdoorParty : GatheringWorker
	{
		protected override LordJob CreateLordJob(IntVec3 spot, Pawn organizer)
		{
			return new LordJob_Joinable_OutdoorParty(spot, organizer, VSIE_DefOf.VSIE_OutdoorParty);
		}

		private bool BasePawnValidator(Pawn pawn, GatheringDef gatheringDef)
		{
			var value = pawn.RaceProps.Humanlike && !pawn.InBed() && !pawn.InMentalState && pawn.GetLord() == null
			&& GatheringsUtility.ShouldPawnKeepGathering(pawn, gatheringDef) && !pawn.Drafted && (gatheringDef.requiredTitleAny == null || gatheringDef.requiredTitleAny.Count == 0
			|| (pawn.royalty != null && pawn.royalty.AllTitlesInEffectForReading.Any((RoyalTitle t) => gatheringDef.requiredTitleAny.Contains(t.def)))) && JoyUtility.EnjoyableOutsideNow(pawn);
			return value;
		}
		protected override Pawn FindOrganizer(Map map)
		{
			Predicate<Pawn> v = (Pawn organizer) => BasePawnValidator(organizer, this.def);
			if (map.mapPawns.SpawnedPawnsInFaction(Faction.OfPlayer).Where(x => v(x)).TryRandomElement(out Pawn result))
			{
				return result;
			}
			return null;
		}
        public override bool CanExecute(Map map, Pawn organizer = null)
        {
			if (!VanillaSocialInteractionsExpandedSettings.EnableGroupActivities)
            {
				return false;
            }
			if (GenLocalDate.HourInteger(map) >= 20 || GenLocalDate.HourInteger(map) <= 5)
            {
				return false;
            }
			if (organizer == null)
			{
				organizer = FindOrganizer(map);
			}
			if (organizer == null)
			{
				return false;
			}
			if (!TryFindGatherSpot(organizer, out IntVec3 _))
			{
				return false;
			}
			if (!GatheringsUtility.PawnCanStartOrContinueGathering(organizer))
			{
				return false;
			}
			return true;
		}
        protected override bool TryFindGatherSpot(Pawn organizer, out IntVec3 spot)
		{
			if (RCellFinder.TryFindRandomSpotJustOutsideColony(organizer, out spot))
            {
				return true;
            }
			return false;
		}
	}
}
