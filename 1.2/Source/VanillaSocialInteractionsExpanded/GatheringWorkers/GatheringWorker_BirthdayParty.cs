using RimWorld;
using Verse;
using Verse.AI.Group;

namespace VanillaSocialInteractionsExpanded
{
	public class GatheringWorker_BirthdayParty : GatheringWorker
	{
		protected override LordJob CreateLordJob(IntVec3 spot, Pawn organizer)
		{
			return new LordJob_Joinable_Party(spot, organizer, def);
		}

        protected override Pawn FindOrganizer(Map map)
        {
			if (!VanillaSocialInteractionsExpandedSettings.EnableGroupActivities)
			{
				return null;
			}
			var socialManager = VSIE_Utils.SocialInteractionsManager;
			foreach (var data in socialManager.birthdays)
            {
				if (data.Value + GenDate.TicksPerDay > Find.TickManager.TicksGame)
                {
					return data.Key;
                }
            }
			return null;
        }
        protected override bool TryFindGatherSpot(Pawn organizer, out IntVec3 spot)
		{
			return RCellFinder.TryFindGatheringSpot_NewTemp(organizer, def, ignoreRequiredColonistCount: false, out spot);
		}
	}
}
