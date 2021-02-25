using RimWorld;
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

        public override bool CanExecute(Map map, Pawn organizer = null)
        {
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
			return RCellFinder.TryFindGatheringSpot_NewTemp(organizer, VSIE_DefOf.VSIE_BingeParty, ignoreRequiredColonistCount: false, out spot);
		}
	}
}
