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
            if (!TryFindGatherSpot(organizer, out IntVec3 _))
            {
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
            var graves = map.listerThings.AllThings.OfType<Building_Grave>().Where(g => g.Corpse != null && (g.Corpse.InnerPawn?.RaceProps.Humanlike ?? false) 
                && g.Corpse.InnerPawn.Faction == Faction.OfPlayer
                && !VSIE_Utils.SocialInteractionsManager.honoredDeadPawns.Contains(g.Corpse.InnerPawn)
                && g.Corpse.Age > GenDate.TicksPerDay && g.Corpse.Age < GenDate.TicksPerDay * 7);
            if (graves.Any())
            {
                grave = graves.OrderBy(x => x.Corpse.Age).First();
                return grave.Corpse.InnerPawn;
            }
            grave = null;
            return null;
        }
        protected override bool TryFindGatherSpot(Pawn organizer, out IntVec3 spot)
        {
            var deadPawn = FindPawnToHonor(organizer.Map, out var grave);
            if (grave != null)
            {
                spot = grave.Position;
                return true;
            }
            spot = IntVec3.Invalid;
            return false;
        }
    }
}