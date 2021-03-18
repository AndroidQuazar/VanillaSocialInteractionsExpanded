using RimWorld;
using System;
using System.Linq;
using Verse;
using Verse.AI.Group;

namespace VanillaSocialInteractionsExpanded
{

	public class GatheringWorker_BingeParty : GatheringWorker
	{
		protected override LordJob CreateLordJob(IntVec3 spot, Pawn organizer)
		{
			return new LordJob_Joinable_BingeParty(spot, organizer, VSIE_DefOf.VSIE_BingeParty);
		}

        protected override Pawn FindOrganizer(Map map)
        {
			Predicate<Pawn> v = (Pawn x) => x.RaceProps.Humanlike && x.needs?.food != null && !x.InBed() && !x.InMentalState && x.GetLord() == null && GatheringsUtility.ShouldPawnKeepGathering(x, def) && !x.Drafted 
			&& (def.requiredTitleAny == null || def.requiredTitleAny.Count == 0 || (x.royalty != null 
			&& x.royalty.AllTitlesInEffectForReading.Any((RoyalTitle t) => def.requiredTitleAny.Contains(t.def))));
			if ((from x in map.mapPawns.SpawnedPawnsInFaction(Faction.OfPlayer)
				 where v(x)
				 select x).TryRandomElement(out Pawn result))
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
			if (organizer == null)
			{
				organizer = FindOrganizer(map);
			}
			if (organizer == null)
			{
				return false;
			}

			if (JobGiver_EatDrinkAndTakeDrugsInGatheringArea.FindFood(organizer) is null)
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

			return RCellFinder.TryFindGatheringSpot_NewTemp(organizer, VSIE_DefOf.VSIE_BingeParty, ignoreRequiredColonistCount: false, out spot);
		}
	}
}
