using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using Verse.AI;

namespace VanillaSocialInteractionsExpanded
{
    public class Aspiration : IExposable
    {
        private Pawn pawn;
        private ThoughtDef thoughtDefSuccess;
        private ThoughtDef thoughtDefFail;
        public InspirationDef inspirationDef;

        private int count;
        private int finalCount;
        public Aspiration()
        {

        }
        public Aspiration(Pawn pawn, InspirationDef inspirationDef, int finalCount, ThoughtDef thoughtDefSuccess, ThoughtDef thoughtDefFail)
        {
            this.pawn = pawn;
            this.inspirationDef = inspirationDef;
            this.finalCount = finalCount;
            this.thoughtDefSuccess = thoughtDefSuccess;
            this.thoughtDefFail = thoughtDefFail;
        }
        public void Notify_Progress(out bool finished)
        {
            count++;
            if (count >= finalCount)
            {
                finished = true;
            }
            else
            {
                finished = false;
            }
        }

        public void OnComplete()
        {
            pawn.needs?.mood?.thoughts?.memories?.TryGainMemory(thoughtDefSuccess);
        }

        public void OnExpired()
        {
            pawn.needs?.mood?.thoughts?.memories?.TryGainMemory(thoughtDefFail);
        }
        public void ExposeData()
        {
            Scribe_References.Look(ref pawn, "pawn");
            Scribe_Defs.Look(ref thoughtDefSuccess, "thoughtDefSuccess");
            Scribe_Defs.Look(ref thoughtDefFail, "thoughtDefFail");
            Scribe_Values.Look(ref count, "count");
            Scribe_Values.Look(ref finalCount, "finalCount");
            Scribe_Defs.Look(ref inspirationDef, "inspirationDef");
        }
    }
    public class SocialInteractionsManager : GameComponent
    {
        private Dictionary<Pawn, Aspiration> activeAspirations = new Dictionary<Pawn, Aspiration>();
        public HashSet<Pawn> pawnsWithAdditionalTrait = new HashSet<Pawn>();
        public SocialInteractionsManager()
		{
		}

		public SocialInteractionsManager(Game game)
		{

		}
        public void PreInit()
        {

        }
        public override void StartedNewGame()
        {
            PreInit();
            base.StartedNewGame();
        }

        public override void LoadedGame()
        {
            PreInit();
            base.LoadedGame();
        }

        public void Notify_AspirationProgress(Pawn pawn)
        {
            if (activeAspirations.TryGetValue(pawn, out var aspiration))
            {
                if (aspiration.inspirationDef == pawn.InspirationDef)
                {
                    aspiration.Notify_Progress(out bool finished);
                    if (finished)
                    {
                        Log.Message(pawn + " aspiration is done: " + aspiration.inspirationDef);
                        aspiration.OnComplete();
                        activeAspirations.Remove(pawn);
                    }
                }
            }
        }

        public void TryRegisterNewIspiration(Pawn pawn, InspirationDef inspirationDef)
        {
            if (activeAspirations.ContainsKey(pawn))
            {
                activeAspirations.Remove(pawn);
            }
            var aspirationDef = GetAspirationDefFor(inspirationDef);
            if (aspirationDef != null)
            {
                activeAspirations[pawn] = new Aspiration(pawn, inspirationDef, aspirationDef.workCount, aspirationDef.thoughtDefSuccess, aspirationDef.thoughtDefFail);
            }
        }

        public AspirationDef GetAspirationDefFor(InspirationDef inspirationDef)
        {
            foreach (var aspirationDef in DefDatabase<AspirationDef>.AllDefs)
            {
                if (aspirationDef.inspirationDef == inspirationDef)
                {
                    return aspirationDef;
                }
            }
            return null;
        }
        public void Notify_InspirationExpired(Pawn pawn, InspirationDef inspirationDef)
        {
            if (activeAspirations.TryGetValue(pawn, out var aspiration) && inspirationDef == aspiration.inspirationDef)
            {
                Log.Message(pawn + " aspiration is expired: " + aspiration.inspirationDef);
                aspiration.OnExpired();
                activeAspirations.Remove(pawn);
            }
        }

        public void TryDevelopNewTrait(Pawn pawn, string letterTextKey)
        {
            Log.Message(" - TryDevelopNewTrait - var traits = DefDatabase<TraitDef>.AllDefsListForReading; - 1", true);
            var traits = DefDatabase<TraitDef>.AllDefsListForReading;
            Log.Message(" - TryDevelopNewTrait - var traitsCount = traits.Count; - 2", true);
            var traitsCount = traits.Count;
            for (var i = 0; i <= traitsCount; i++)
            {
                Log.Message(" - TryDevelopNewTrait - TraitDef newTraitDef = DefDatabase<TraitDef>.AllDefsListForReading.RandomElementByWeight((TraitDef tr) => tr.GetGenderSpecificCommonality(pawn.gender)); - 3", true);
                TraitDef newTraitDef = DefDatabase<TraitDef>.AllDefsListForReading.RandomElementByWeight((TraitDef tr) => tr.GetGenderSpecificCommonality(pawn.gender));
                Log.Message(" - TryDevelopNewTrait - int degree = RandomTraitDegree(newTraitDef); - 4", true);
                int degree = RandomTraitDegree(newTraitDef);
                Log.Message(" - TryDevelopNewTrait - if (TraitIsAllowed(pawn, newTraitDef, degree)) - 5", true);
                if (TraitIsAllowed(pawn, newTraitDef, degree))
                {
                    Log.Message(" - TryDevelopNewTrait - Trait trait2 = new Trait(newTraitDef, degree); - 6", true);
                    Trait trait = new Trait(newTraitDef, degree);
                    Log.Message(" - TryDevelopNewTrait - if (pawn.mindState == null || pawn.mindState.mentalBreaker == null || !((pawn.mindState.mentalBreaker.BreakThresholdMinor + trait2.OffsetOfStat(StatDefOf.MentalBreakThreshold)) * trait2.MultiplierOfStat(StatDefOf.MentalBreakThreshold) > 0.5f)) - 7", true);
                    if (pawn.mindState == null || pawn.mindState.mentalBreaker == null || !((pawn.mindState.mentalBreaker.BreakThresholdMinor + trait.OffsetOfStat(StatDefOf.MentalBreakThreshold)) * trait.MultiplierOfStat(StatDefOf.MentalBreakThreshold) > 0.5f))
                    {
                        Log.Message(" - TryDevelopNewTrait - pawn.story.traits.GainTrait(trait2); - 8", true);
                        pawn.story.traits.GainTrait(trait);
                        Log.Message(" - TryDevelopNewTrait - pawnsWithAdditionalTrait.Add(pawn); - 9", true);
                        pawnsWithAdditionalTrait.Add(pawn);
                        var traitName = trait.CurrentData.GetLabelFor(pawn);
                        var traitDesc = trait.CurrentData.description.Formatted(pawn.Named("PAWN")).AdjustedFor(pawn).Resolve();
                        Find.LetterStack.ReceiveLetter("VSIE.TraitChangeTitle".Translate(traitName, pawn.Named("PAWN")), letterTextKey.Translate(traitName, traitDesc, pawn.Named("PAWN")), LetterDefOf.NeutralEvent, pawn);
                        Log.Message(" - TryDevelopNewTrait - return; - 10", true);
                        return;
                    }
                }
                traits.Remove(newTraitDef);
            }
        }
        private int RandomTraitDegree(TraitDef traitDef)
        {
            if (traitDef.degreeDatas.Count == 1)
            {
                return traitDef.degreeDatas[0].degree;
            }
            return traitDef.degreeDatas.RandomElementByWeight((TraitDegreeData dd) => dd.commonality).degree;
        }
        private bool TraitIsAllowed(Pawn pawn, TraitDef newTraitDef, int degree)
        {
            if (pawn.story.traits.HasTrait(newTraitDef) || (pawn.kindDef.disallowedTraits != null && pawn.kindDef.disallowedTraits.Contains(newTraitDef))
                || (pawn.kindDef.requiredWorkTags != 0 && (newTraitDef.disabledWorkTags & pawn.kindDef.requiredWorkTags) != 0) || (pawn.Faction != null && Faction.OfPlayerSilentFail != null
                && pawn.Faction.HostileTo(Faction.OfPlayer) && !newTraitDef.allowOnHostileSpawn) || pawn.story.traits.allTraits.Any((Trait tr) => newTraitDef.ConflictsWith(tr))
                || (newTraitDef.requiredWorkTypes != null && pawn.OneOfWorkTypesIsDisabled(newTraitDef.requiredWorkTypes)) || pawn.WorkTagIsDisabled(newTraitDef.requiredWorkTags)
                || (newTraitDef.forcedPassions != null && pawn.workSettings != null && newTraitDef.forcedPassions.Any((SkillDef p) =>
                p.IsDisabled(pawn.story.DisabledWorkTagsBackstoryAndTraits, pawn.GetDisabledWorkTypes(permanentOnly: true))))
                || pawn.story.childhood.DisallowsTrait(newTraitDef, degree) && (pawn.story.adulthood == null || pawn.story.adulthood.DisallowsTrait(newTraitDef, degree)))
            {
                    return false;
            }
            return true;
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Collections.Look(ref activeAspirations, "activeAspirations", LookMode.Reference, LookMode.Deep, ref pawnKeys, ref aspirationValues);
            Scribe_Collections.Look(ref pawnsWithAdditionalTrait, "pawnsWithAdditionalTrait", LookMode.Reference);
        }

        private List<Pawn> pawnKeys;
        private List<Aspiration> aspirationValues;
    }
}
