using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace VanillaSocialInteractionsExpanded
{
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

		private static SocialInteractionsManager sManager;

		public static void TryDevelopNewTrait(Pawn pawn, string letterText)
        {
			var manager = VSIE_Utils.SocialInteractionsManager;
			if (!manager.pawnsWithAdditionalTrait.Contains(pawn))
			{
				manager.TryDevelopNewTrait(pawn, letterText);
			}
		}

		public static List<TaleDef> blacklistedTales = new List<TaleDef>
		{
			VSIE_DefOf.VSIE_ExposedCorpseOfMyFriend
		};

		[DebugAction("General", null, allowedGameStates = AllowedGameStates.PlayingOnMap)]
		private static void DebugAllRelations()
		{
			var tales = DefDatabase<TaleDef>.AllDefs.Where(x => x.defName.StartsWith("VSIE") && !blacklistedTales.Contains(x));
			var testeers = Find.CurrentMap.mapPawns.AllPawns.Where(x => x.IsColonist && x.Spawned && x.RaceProps.Humanlike);
			var triplePawnsTale = tales.Where(x => x.taleClass == typeof(Tale_TriplePawn));
			var doublePawnsTale = tales.Where(x => x.taleClass == typeof(Tale_DoublePawn));
			var onePawnTale = tales.Where(x => x.taleClass == typeof(Tale_SinglePawn));

			foreach (var tale in triplePawnsTale)
            {
				var threePawns = testeers.InRandomOrder().Take(3).ToList();
				TaleRecorder.RecordTale(tale, threePawns[0], threePawns[1], threePawns[2]);
				Find.LetterStack.ReceiveLetter(tale.defName, tale.label, LetterDefOf.NeutralEvent, threePawns);
            }

			//foreach (var tale in doublePawnsTale)
			//{
			//	var twoPawns = testeers.InRandomOrder().Take(2).ToList();
			//	TaleRecorder.RecordTale(tale, twoPawns[0], twoPawns[1]);
			//	Find.LetterStack.ReceiveLetter(tale.defName, tale.label, LetterDefOf.NeutralEvent, twoPawns);
			//}
			//
			//foreach (var tale in onePawnTale)
			//{
			//	var onePawns = testeers.InRandomOrder().Take(1).ToList();
			//	TaleRecorder.RecordTale(tale, onePawns[0]);
			//	Find.LetterStack.ReceiveLetter(tale.defName, tale.label, LetterDefOf.NeutralEvent, onePawns);
			//}
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
