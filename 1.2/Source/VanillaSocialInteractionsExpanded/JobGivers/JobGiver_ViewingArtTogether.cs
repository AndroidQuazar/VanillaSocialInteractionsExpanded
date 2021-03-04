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
    public class JobGiver_ViewingArtTogether : ThinkNode_JobGiver
    {
		private static List<Thing> candidates = new List<Thing>();
		protected override Job TryGiveJob(Pawn pawn)
		{
			var companion = VSIE_Utils.GetCompanion(pawn);
			if (companion.CurJobDef == VSIE_DefOf.VSIE_ViewArtTogether)
			{
				return JobMaker.MakeJob(VSIE_DefOf.VSIE_ViewArtTogether, companion.CurJob.targetA, companion);
			}
			else if (TryFindArtToView(pawn, out Thing art))
			{
				return JobMaker.MakeJob(VSIE_DefOf.VSIE_ViewArtTogether, art, companion);
			}
			return null;
		}

		public static bool TryFindArtToView(Pawn pawn, out Thing art)
        {
			bool result = false;
			try
			{
				bool allowedOutside = JoyUtility.EnjoyableOutsideNow(pawn);
				candidates.AddRange(pawn.Map.listerThings.ThingsInGroup(ThingRequestGroup.Art).Where(delegate (Thing thing)
				{
					if (thing.Faction != Faction.OfPlayer || thing.IsForbidden(pawn) || (!allowedOutside && !thing.Position.Roofed(thing.Map))
					|| !pawn.CanReserveAndReach(thing, PathEndMode.Touch, Danger.None) || !thing.IsPoliticallyProper(pawn))
					{
						return false;
					}
					CompArt compArt = thing.TryGetComp<CompArt>();
					if (compArt == null)
					{
						Log.Error("No CompArt on thing being considered for viewing: " + thing);
						return false;
					}
					if (!compArt.CanShowArt || !compArt.Props.canBeEnjoyedAsArt)
					{
						return false;
					}
					Room room = thing.GetRoom();
					if (room == null)
					{
						return false;
					}
					return ((room.Role != RoomRoleDefOf.Bedroom && room.Role != RoomRoleDefOf.Barracks && room.Role != RoomRoleDefOf.PrisonCell && room.Role != RoomRoleDefOf.PrisonBarracks && room.Role != RoomRoleDefOf.Hospital) || (pawn.ownership != null && pawn.ownership.OwnedRoom != null && pawn.ownership.OwnedRoom == room)) ? true : false;
				}));
				if (candidates.TryRandomElementByWeight((Thing target) => Mathf.Max(target.GetStatValue(StatDefOf.Beauty), 0.5f), out art))
				{
					result = true;
				}
			}
			finally
			{
				candidates.Clear();
			}
			return result;
		}
	}
}

