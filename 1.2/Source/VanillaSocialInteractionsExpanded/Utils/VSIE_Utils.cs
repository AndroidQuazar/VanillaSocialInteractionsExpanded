using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using Verse.AI;
using Verse.AI.Group;

namespace VanillaSocialInteractionsExpanded
{
	public class FloatValueCache
	{
		public FloatValueCache(float value)
		{
			this.value = value;
		}
		public float value;
		public float Value
		{
			get
			{
				return value;
			}
			set
			{
				this.value = value;
				updateTick = Find.TickManager.TicksGame;
			}
		}
		public int updateTick;
	}

	[StaticConstructorOnStartup]
    public static class VSIE_Utils
    {
		static VSIE_Utils()
        {
			foreach (var aspirationDef in DefDatabase<AspirationDef>.AllDefs)
            {
				if (!aspirationDef.beginLetterContinue.NullOrEmpty())
                {
					aspirationDef.inspirationDef.beginLetter += aspirationDef.beginLetterContinue;
				}
			}
		}
		public static SocialInteractionsManager SocialInteractionsManager
		{
			get
			{
				if (sManager is null)
				{
					sManager = Current.Game.GetComponent<SocialInteractionsManager>();
					return sManager;
				}
				return sManager;
			}
		}

		private static Dictionary<Pawn, FloatValueCache> averageOpinionOfCache = new Dictionary<Pawn, FloatValueCache>();

		public static float GetAverageOpinionOf(Pawn pawn)
		{
			if (averageOpinionOfCache.TryGetValue(pawn, out FloatValueCache floatValueCache))
			{
				var averageOpinionOf = floatValueCache.value;
				if (Find.TickManager.TicksGame > floatValueCache.updateTick + 3600)
				{
					averageOpinionOf = AverageOpinionOf(pawn);
					floatValueCache.Value = averageOpinionOf;
				}
				return averageOpinionOf;
			}
			else
			{
				var averageOpinionOf = AverageOpinionOf(pawn);
				averageOpinionOfCache[pawn] = new FloatValueCache(averageOpinionOf);
				return averageOpinionOf;
			}
		}

		private static float AverageOpinionOf(Pawn pawn)
		{
			var allOpinions = new List<float>();
			var pawns = PawnsFinder.AllMaps_SpawnedPawnsInFaction(pawn.Faction).ToHashSet();
			if (pawn.relations != null)
            {
				pawns.AddRange(pawn.relations.PotentiallyRelatedPawns);
            }
			foreach (var otherPawn in pawns)
			{
				if (otherPawn.relations != null)
				{
					allOpinions.Add(otherPawn.relations.OpinionOf(pawn));
				}
			}
			if (allOpinions.Any())
            {
				return allOpinions.Average();
            }
			return 0f;
		}
		public static Pawn GetCompanion(Pawn pawn)
		{
			var lord = pawn.GetLord();
			if (lord != null && lord.LordJob is LordJob_Joinable_DoublePawn lordJob)
			{
				if (pawn == lordJob.Organizer)
				{
					return lordJob.secondPawn;
				}
				else
				{
					return lordJob.Organizer;
				}
			}
			return null;
		}
		public static bool DrugValidator(Pawn pawn, Thing drug)
		{
			if (drug is null)
            {
				return false;
            }
			if (!drug.def.IsDrug)
			{
				return false;
			}
			if (drug.Spawned)
			{
				if (drug.IsForbidden(pawn))
				{
					return false;
				}
				if (!pawn.CanReserve(drug))
				{
					return false;
				}
				if (!drug.IsSociallyProper(pawn))
				{
					return false;
				}
				if (drug.def.ingestible.drugCategory != DrugCategory.Social)
				{
					return false;
				}
			}
			CompDrug compDrug = drug.TryGetComp<CompDrug>();
			if (compDrug == null || compDrug.Props.chemical == null)
			{
				return false;
			}
			if (pawn.drugs != null && !pawn.drugs.CurrentPolicy[drug.def].allowedForAddiction && pawn.story != null
				&& pawn.story.traits.DegreeOfTrait(TraitDefOf.DrugDesire) <= 0)
			{
				return false;
			}
			return true;
		}

		public static HashSet<JobTag> workTags = new HashSet<JobTag> { JobTag.Misc, JobTag.MiscWork, JobTag.Fieldwork };

		private static SocialInteractionsManager sManager;

		public static IEnumerable<Pawn> GetFriendsFor(Pawn pawn)
        {
			var bestFriend = pawn.relations.GetFirstDirectRelationPawn(VSIE_DefOf.VSIE_BestFriend);
			if (bestFriend != null)
            {
				yield return bestFriend;
            }
			foreach (var otherPawn in pawn.relations.PotentiallyRelatedPawns)
            {
				if (otherPawn.relations.OpinionOf(pawn) >= 20 && pawn.relations.OpinionOf(otherPawn) >= 20)
                {
					yield return otherPawn;
                }
            }
		}
		public static void TryDevelopNewTrait(Pawn pawn, string letterText)
        {
			var manager = VSIE_Utils.SocialInteractionsManager;
			if (!manager.pawnsWithAdditionalTrait.Contains(pawn))
			{
				manager.TryDevelopNewTrait(pawn, letterText);
			}
		}

		public static bool HaveNoticedTale(Pawn pawn, Tale tale)
        {
			if (!SocialInteractionsManager.joinedColonists.TryGetValue(pawn, out int date))
			{
				//Log.Message(pawn + " haven't joined the colony " + tale);
				return false;
			}
			else if (tale.date < date)
            {
				//Log.Message(pawn + " haven't noticed " + tale + " - " + tale.date + " - " + date);
				return false;
			}
			//Log.Message(pawn + " noticed " + tale);
			return true;
		}

		public static void TryRegisterNewColonist(Pawn pawn, Faction faction)
        {
			if (SocialInteractionsManager.joinedColonists is null)
            {
				SocialInteractionsManager.joinedColonists = new Dictionary<Pawn, int>();
			}
			if (pawn != null && faction == Faction.OfPlayer && (pawn.RaceProps?.Humanlike ?? false) && !SocialInteractionsManager.joinedColonists.ContainsKey(pawn))
			{
				SocialInteractionsManager.joinedColonists[pawn] = Find.TickManager.TicksAbs;
			}
		}

		//public static List<TaleDef> blacklistedTales = new List<TaleDef>
		//{
		//	VSIE_DefOf.VSIE_ExposedCorpseOfMyFriend
		//};
		//
		//[DebugAction("General", null, allowedGameStates = AllowedGameStates.PlayingOnMap)]
		//private static void DebugAllRelations()
		//{
		//	var tales = DefDatabase<TaleDef>.AllDefs.Where(x => x.defName.StartsWith("VSIE") && !blacklistedTales.Contains(x));
		//	var testeers = Find.CurrentMap.mapPawns.AllPawns.Where(x => x.IsColonist && x.Spawned && x.RaceProps.Humanlike);
		//	var triplePawnsTale = tales.Where(x => x.taleClass == typeof(Tale_TriplePawn));
		//	var doublePawnsTale = tales.Where(x => x.taleClass == typeof(Tale_DoublePawn));
		//	var onePawnTale = tales.Where(x => x.taleClass == typeof(Tale_SinglePawn));
		//
		//	foreach (var tale in triplePawnsTale)
		//    {
		//		var threePawns = testeers.InRandomOrder().Take(3).ToList();
		//		TaleRecorder.RecordTale(tale, threePawns[0], threePawns[1], threePawns[2]);
		//		Find.LetterStack.ReceiveLetter(tale.defName, tale.label, LetterDefOf.NeutralEvent, threePawns);
		//    }
		//
		//	foreach (var tale in doublePawnsTale)
		//	{
		//		var twoPawns = testeers.InRandomOrder().Take(2).ToList();
		//		TaleRecorder.RecordTale(tale, twoPawns[0], twoPawns[1]);
		//		Find.LetterStack.ReceiveLetter(tale.defName, tale.label, LetterDefOf.NeutralEvent, twoPawns);
		//	}
		//	
		//	foreach (var tale in onePawnTale)
		//	{
		//		var onePawns = testeers.InRandomOrder().Take(1).ToList();
		//		TaleRecorder.RecordTale(tale, onePawns[0]);
		//		Find.LetterStack.ReceiveLetter(tale.defName, tale.label, LetterDefOf.NeutralEvent, onePawns);
		//	}
		//}


		[DebugAction("Pawns", "Remove Memory", actionType = DebugActionType.ToolMapForPawns, allowedGameStates = AllowedGameStates.PlayingOnMap)]
		private static void RemoveMemory(Pawn p)
		{
			if (Find.TaleManager.AllTalesListForReading.Where(x => x.def.defName.StartsWith("VSIE_") && x.Concerns(p)).Any())
            {
				Find.WindowStack.Add(new Dialog_DebugOptionListLister(Options_RemoveMemory(p)));
            }
		}
		public static List<DebugMenuOption> Options_RemoveMemory(Pawn pawn)
		{
			List<DebugMenuOption> list = new List<DebugMenuOption>();
			foreach (Tale tale in Find.TaleManager.AllTalesListForReading.Where(x => x.def.defName.StartsWith("VSIE_") && x.Concerns(pawn)))
			{
				Tale taleH = tale;
				list.Add(new DebugMenuOption(taleH.def.LabelCap, DebugMenuOptionMode.Action, delegate
				{
					Find.TaleManager.AllTalesListForReading.Remove(taleH);
				}));
			}
			return list;
		}

		public static Pawn GetSpouseOrLoverOrFiance(this Pawn pawn)
		{
			if (!pawn.RaceProps.IsFlesh)
			{
				return null;
			}
			var spouse = pawn.relations.GetFirstDirectRelationPawn(PawnRelationDefOf.Spouse);
			if (spouse is null)
			{
				var lover = pawn.relations.GetFirstDirectRelationPawn(PawnRelationDefOf.Lover);
				if (lover is null)
				{
					var fiance = pawn.relations.GetFirstDirectRelationPawn(PawnRelationDefOf.Fiance);
					return fiance;
				}
				else
				{
					return lover;
				}
			}
			return spouse;
		}

		public static Tale_TriplePawn GetLatestTriplePawnTale(TaleDef def, Predicate<Tale_TriplePawn> predicate)
		{
			Tale_TriplePawn tale = null;
			int num = 0;
			for (int i = 0; i < Find.TaleManager.AllTalesListForReading.Count; i++)
			{
				var latestTale = Find.TaleManager.AllTalesListForReading[i];
				if (latestTale.def == def && latestTale is Tale_TriplePawn tale_triplePawn && predicate(tale_triplePawn) && (tale == null || latestTale.AgeTicks < num))
				{
					tale = tale_triplePawn;
					num = latestTale.AgeTicks;
				}
			}
			return tale;
		}
		public static Tale_DoublePawn GetLatestDoublePawnTale(TaleDef def, Predicate<Tale_DoublePawn> predicate)
		{
			Tale_DoublePawn tale = null;
			int num = 0;
			for (int i = 0; i < Find.TaleManager.AllTalesListForReading.Count; i++)
			{
				var latestTale = Find.TaleManager.AllTalesListForReading[i];
				if (latestTale.def == def && latestTale is Tale_DoublePawn tale_doublePawn && predicate(tale_doublePawn) && (tale == null || latestTale.AgeTicks < num))
				{
					tale = tale_doublePawn;
					num = latestTale.AgeTicks;
				}
			}
			return tale;
		}
		
		public static Tale GetLatestTale(TaleDef def, Predicate<Tale> predicate)
		{
			Tale tale = null;
			int num = 0;
			var tales = Find.TaleManager.AllTalesListForReading;
			for (int i = 0; i < tales.Count; i++)
			{
				if (tales[i].def == def && predicate(tales[i]) && (tale == null || tales[i].AgeTicks < num))
				{
					tale = tales[i];
					num = tales[i].AgeTicks;
				}
			}
			return tale;
		}
	}
}
