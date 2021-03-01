using RimWorld;
using System.Linq;
using Verse;
using Verse.AI.Group;

namespace VanillaSocialInteractionsExpanded
{
    public class GatheringWorker_Funeral : GatheringWorker
    {
        protected override LordJob CreateLordJob(IntVec3 spot, Pawn organizer)
        {
            var deadPawn = FindPawnToHonor(organizer.Map, out var grave);
            return new LordJob_Joinable_Funeral(spot, organizer, grave, VSIE_DefOf.VSIE_Funeral);
        }
        public override bool CanExecute(Map map, Pawn organizer = null)
        {
            Log.Message("GatheringWorker_Funeral : GatheringWorker - CanExecute - if (organizer == null) - 2", true);
            if (organizer == null)
            {
                Log.Message("GatheringWorker_Funeral : GatheringWorker - CanExecute - organizer = FindOrganizer(map); - 3", true);
                organizer = FindOrganizer(map);
            }
            Log.Message("GatheringWorker_Funeral : GatheringWorker - CanExecute - if (organizer == null) - 4", true);
            if (organizer == null)
            {
                Log.Message("GatheringWorker_Funeral : GatheringWorker - CanExecute - return false; - 5", true);
                return false;
            }
            Log.Message("GatheringWorker_Funeral : GatheringWorker - CanExecute - if (!TryFindGatherSpot(organizer, out IntVec3 _)) - 6", true);
            if (!TryFindGatherSpot(organizer, out IntVec3 _))
            {
                Log.Message("GatheringWorker_Funeral : GatheringWorker - CanExecute - return false; - 7", true);
                return false;
            }
            return true;
        }

        protected override void SendLetter(IntVec3 spot, Pawn organizer)
        {
            var pawnToHonor = FindPawnToHonor(organizer.Map, out var grave);
            Find.LetterStack.ReceiveLetter(def.letterTitle, def.letterText.Formatted(pawnToHonor.Named("DEADPAWN")), LetterDefOf.PositiveEvent, new TargetInfo(spot, organizer.Map));
        }
        private Pawn FindPawnToHonor(Map map, out Building_Grave grave)
        {
            var graves = map.listerThings.AllThings.OfType<Building_Grave>().Where(g => g.Corpse != null
                && !VSIE_Utils.SocialInteractionsManager.honoredDeadPawns.Contains(g.Corpse.InnerPawn)
                && g.Corpse.Age > GenDate.TicksPerDay && g.Corpse.Age < GenDate.TicksPerDay * 7);
            Log.Message("GatheringWorker_Funeral : GatheringWorker - FindOrganizer - if (graves.Any()) - 12", true);
            if (graves.Any())
            {
                grave = graves.OrderBy(x => x.Corpse.Age).First();
                Log.Message("GatheringWorker_Funeral : GatheringWorker - FindOrganizer - return graves.OrderBy(x => x.Corpse.Age).First().Corpse.InnerPawn; - 13", true);
                return grave.Corpse.InnerPawn;
            }
            grave = null;
            return null;
        }
        protected override bool TryFindGatherSpot(Pawn organizer, out IntVec3 spot)
        {
            var deadPawn = FindPawnToHonor(organizer.Map, out var grave);
            Log.Message("GatheringWorker_Funeral : GatheringWorker - TryFindGatherSpot - if (organizer?.ParentHolder is Building_Grave building_Grave) - 15", true);
            if (grave != null)
            {
                Log.Message("GatheringWorker_Funeral : GatheringWorker - TryFindGatherSpot - spot = building_Grave.Position; - 16", true);
                spot = grave.Position;
                Log.Message("GatheringWorker_Funeral : GatheringWorker - TryFindGatherSpot - return true; - 17", true);
                return true;
            }
            spot = IntVec3.Invalid;
            Log.Message("GatheringWorker_Funeral : GatheringWorker - TryFindGatherSpot - return false; - 19", true);
            return false;
        }
    }
}