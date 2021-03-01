using RimWorld;
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
        public override bool CanExecute(Map map, Pawn organizer = null)
        {
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
