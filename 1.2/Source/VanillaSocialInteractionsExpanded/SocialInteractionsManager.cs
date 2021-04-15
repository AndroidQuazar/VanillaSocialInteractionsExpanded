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

    public class TeachingTopic : IExposable
    {
        public TeachingTopic()
        {

        }
        public TeachingTopic(Pawn pupil, SkillDef skillDef)
        {
            this.pupil = pupil;
            this.skillDef = skillDef;
        }

        public Pawn pupil;
        public SkillDef skillDef;

        public void ExposeData()
        {
            Scribe_References.Look(ref pupil, "pupil");
            Scribe_Defs.Look(ref skillDef, "skillDef");
        }
    }

    public class WorkTime : IExposable
    {
        public int workTick;
        public int lastTick;

        public void ExposeData()
        {
            Scribe_Values.Look(ref workTick, "workTick");
            Scribe_Values.Look(ref lastTick, "lastTick");
        }
    }
    public class Workers : IExposable
    {
        public Workers()
        {

        }
        public Dictionary<Pawn, WorkTime> workersWithWorkingTicks = new Dictionary<Pawn, WorkTime>();

        public bool TryCauseGroupFights(Pawn initiator)
        {
            if (initiator.Map != null && initiator.Faction != null && initiator.mindState != null && initiator.interactions != null && initiator.CurJobDef != JobDefOf.SocialFight 
                && VSIE_Utils.workTags.Contains(initiator.mindState.lastJobTag)
                && initiator.needs?.mood?.CurLevelPercentage < 0.3f && !initiator.WorkTagIsDisabled(WorkTags.Violent))
            {
                if (!InteractionUtility.TryGetRandomVerbForSocialFight(initiator, out Verb verb))
                {
                    return false;
                }
                if (workersWithWorkingTicks is null)
                {
                    workersWithWorkingTicks = new Dictionary<Pawn, WorkTime>();
                }
                var candidates = workersWithWorkingTicks.Where(x => x.Key != null && x.Key.needs?.mood?.CurInstantLevelPercentage < 0.3f && x.Value.workTick > 3000 
                    && initiator.relations?.OpinionOf(x.Key) < 0).Select(x => x.Key).ToList();
                if (candidates.Any())
                {
                    var manager = VSIE_Utils.SocialInteractionsManager;
                    if (manager.angryWorkers is null)
                    {
                        manager.angryWorkers = new Dictionary<Pawn, int>();
                    }
                    manager.angryWorkers[initiator] = Find.TickManager.TicksGame;
                    foreach (var worker in candidates)
                    {
                        manager.angryWorkers[worker] = Find.TickManager.TicksGame;
                    }

                    var nearestWorkers = initiator.Map.mapPawns.SpawnedPawnsInFaction(initiator.Faction).Where(x => x != initiator && x != null && x.RaceProps.Humanlike
                        && x.mindState != null && VSIE_Utils.workTags.Contains(x.mindState.lastJobTag) && x.needs?.mood?.CurLevelPercentage < 0.3f && !x.WorkTagIsDisabled(WorkTags.Violent)
                        && x.Position.DistanceTo(initiator.Position) < 10).ToHashSet();

                    foreach (var worker in nearestWorkers)
                    {
                        manager.angryWorkers[worker] = Find.TickManager.TicksGame;
                    }
                    var angryWorkers = manager.angryWorkers.Keys.Where(x => x != initiator && x.Position.DistanceTo(initiator.Position) <= 10);
                    nearestWorkers.AddRange(candidates);
                    nearestWorkers.AddRange(angryWorkers);

                    var fighters = new HashSet<Pawn>();
                    var firstPawn = candidates.OrderBy(x => x.Position.DistanceTo(initiator.Position)).First();
                    initiator.interactions.StartSocialFight(firstPawn);
                    fighters.Add(initiator);
                    fighters.Add(firstPawn);
                    foreach (var pawn in nearestWorkers)
                    {
                        if (!pawn.InMentalState && pawn.interactions != null)
                        {
                            var candidatesToFight = angryWorkers.Where(x => x != pawn);
                            if (candidatesToFight.Any() && candidatesToFight.TryRandomElementByWeight(x => DistanceScore(x.Position.DistanceTo(pawn.Position)), out Pawn victim))
                            {
                                fighters.Add(pawn);
                                fighters.Add(victim);
                                pawn.interactions.StartSocialFight(victim);
                            }
                        }
                    }

                    Messages.Message("VSIE.Discord".Translate(), fighters.ToList(), MessageTypeDefOf.NegativeEvent);
                    return true;
                }
            }
            return false;
        }

        private static readonly SimpleCurve DistanceFactor = new SimpleCurve
        {
            new CurvePoint(30f, 0f),
            new CurvePoint(20f, 0.1f),
            new CurvePoint(15f, 0.2f),
            new CurvePoint(10f, 0.6f),
            new CurvePoint(9f, 0.7f),
            new CurvePoint(8f, 0.8f),
            new CurvePoint(7f, 0.85f),
            new CurvePoint(6f, 0.9f),
            new CurvePoint(5f, 0.91f),
            new CurvePoint(4f, 0.93f),
            new CurvePoint(3f, 0.95f),
            new CurvePoint(2f, 0.97f),
            new CurvePoint(1f, 0.99f),
            new CurvePoint(0f, 1f)
        };
        private float DistanceScore(float distance)
        {
            return DistanceFactor.Evaluate(distance);
        }
        public void ExposeData()
        {
            Scribe_Collections.Look(ref workersWithWorkingTicks, "workersWithWorkingTicks", LookMode.Reference, LookMode.Deep, ref pawnsKeys, ref intValues);
        }
        private List<Pawn> pawnsKeys;
        private List<WorkTime> intValues;
    }

    public class RaidGroup : IExposable
    {
        public HashSet<Pawn> defenders;
        public HashSet<Pawn> raiders;
        public HashSet<Lord> raiderLords;
        public Faction faction;
        public int initTime;
        public RaidGroup()
        {
            defenders = new HashSet<Pawn>();
            raiders = new HashSet<Pawn>();
            raiderLords = new HashSet<Lord>();
        }
        public void ExposeData()
        {
            Scribe_Collections.Look(ref defenders, saveDestroyedThings: true, "defenders", LookMode.Reference);
            Scribe_Collections.Look(ref raiders, saveDestroyedThings: true, "raiders", LookMode.Reference);
            Scribe_Collections.Look(ref raiderLords, "lords", LookMode.Reference);
            Scribe_References.Look(ref faction, "faction");
            Scribe_Values.Look(ref initTime, "initTime");
        }
    }

    public class SocialInteractionsManager : GameComponent
    {
        public List<RaidGroup> raidGroups;
        private Dictionary<Pawn, Aspiration> activeAspirations = new Dictionary<Pawn, Aspiration>();
        public HashSet<Pawn> pawnsWithAdditionalTrait = new HashSet<Pawn>();
        public Dictionary<Pawn, TeachingTopic> teachersWithPupils = new Dictionary<Pawn, TeachingTopic>();
        public Dictionary<Pawn, Workers> pawnsWithWorkers = new Dictionary<Pawn, Workers>();
        public Dictionary<Pawn, int> angryWorkers = new Dictionary<Pawn, int>();
        public HashSet<Pawn> honoredDeadPawns = new HashSet<Pawn>();
        public Dictionary<Pawn, int> birthdays = new Dictionary<Pawn, int>();
        public Dictionary<Pawn, int> joinedColonists = new Dictionary<Pawn, int>();

        public int postRaidPeriodTicks;
        public SocialInteractionsManager()
        {
        }

        public SocialInteractionsManager(Game game)
        {

        }
        public void PreInit()
        {
            if (activeAspirations is null) activeAspirations = new Dictionary<Pawn, Aspiration>();
            if (pawnsWithAdditionalTrait is null) pawnsWithAdditionalTrait = new HashSet<Pawn>();
            if (teachersWithPupils is null) teachersWithPupils = new Dictionary<Pawn, TeachingTopic>();
            if (pawnsWithWorkers is null) pawnsWithWorkers = new Dictionary<Pawn, Workers>();
            if (angryWorkers is null) angryWorkers = new Dictionary<Pawn, int>();
            if (honoredDeadPawns is null) honoredDeadPawns = new HashSet<Pawn>();
            if (birthdays is null) birthdays = new Dictionary<Pawn, int>();
            if (raidGroups == null) raidGroups = new List<RaidGroup>();
            if (joinedColonists == null) joinedColonists = new Dictionary<Pawn, int>();

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

        public override void GameComponentTick()
        {
            base.GameComponentTick();
            if (VanillaSocialInteractionsExpandedSettings.EnableDiscord)
            {
                if (pawnsWithWorkers != null)
                {
                    var keysToRemove = new List<Pawn>();
                    foreach (var worker in pawnsWithWorkers)
                    {
                        if (worker.Key.IsHashIntervalTick(300) && Rand.Chance(0.001f) && worker.Value.TryCauseGroupFights(worker.Key))
                        {
                            keysToRemove.Add(worker.Key);
                        }
                    }

                    foreach (var pawn in keysToRemove)
                    {
                        pawnsWithWorkers.Remove(pawn);
                    }
                }
            }
        }

        public void TryAssignThoughtsAfterRaid(Lord lord, bool raidersIsLosing = false)
        {
            if (this.raidGroups != null)
            {
                for (int j = this.raidGroups.Count - 1; j >= 0; j--)
                {
                    if ((this.raidGroups[j].raiderLords?.Contains(lord) ?? false) && this.raidGroups[j].raiderLords.Count > 1)
                    {
                        this.raidGroups[j].raiderLords.Remove(lord);
                        return;
                    }
                }
                var raidGroup = this.raidGroups.Where(x => x.raiderLords?.Contains(lord) ?? false).FirstOrDefault();
                if (raidGroup != null)
                {
                    var remainedRaidersCount = raidGroup.raiders.Where(x => !x.Dead && !x.Downed).Count();
                    var remainedRaidersPercentFromTotal = (float)remainedRaidersCount / (float)raidGroup.raiders.Count();

                    var remainedDefenders = raidGroup.defenders.Where(x => !x.Dead && !x.Downed && x.Map == lord.Map).ToHashSet();
                    remainedDefenders.AddRange(lord.Map.mapPawns.AllPawnsSpawned.Where(x => x.RaceProps.Humanlike && !x.Downed && !x.Dead && !x.Fogged() && !x.IsPrisoner && x.Faction != null
                        && (x.Faction == Faction.OfPlayer || !x.HostileTo(Faction.OfPlayer)) && x.HostileTo(raidGroup.faction)));
                    var remainedDefendersPercentFromTotal = (float)remainedDefenders.Count() / (float)raidGroup.defenders.Count();

                    if (!raidersIsLosing && RaidersWon(raidGroup, lord.Map))
                    {
                        foreach (var pawn in remainedDefenders)
                        {
                            var traits = pawn.story?.traits;
                            if (traits != null)
                            {
                                var nerves = traits.GetTrait(TraitDefOf.Nerves);
                                var naturalMood = traits.GetTrait(TraitDefOf.NaturalMood);
                                if (nerves != null && (nerves.Degree == 2 || nerves.Degree == 1) ||
                                    traits.HasTrait(TraitDefOf.Kind) ||
                                    naturalMood != null && (naturalMood.Degree == 2 || naturalMood.Degree == 1))
                                {
                                    pawn.needs?.mood?.thoughts?.memories?.TryGainMemory(VSIE_DefOf.VSIE_SufferedLossPositive);
                                }
                                else if (traits.HasTrait(TraitDefOf.Greedy) || 
                                    nerves != null && (nerves.Degree == -2 || nerves.Degree == -1) ||
                                    naturalMood != null && (naturalMood.Degree == -2 || naturalMood.Degree == -1))
                                {
                                    pawn.needs?.mood?.thoughts?.memories?.TryGainMemory(VSIE_DefOf.VSIE_SufferedLossNegative);
                                }
                            }
                        }
                        postRaidPeriodTicks = Find.TickManager.TicksGame + (GenDate.TicksPerDay * 3);
                    }

                    if ((Find.TickManager.TicksGame - raidGroup.initTime) >= (GenDate.TicksPerDay / 2) // if raid lasted over 12 in-game hours
                        && raidGroup.raiders.Sum(x => x.kindDef.combatPower) > 500 // if raid is big enough
                        && remainedDefenders.Any() // if there are survivors to give them a trait
                        && Rand.Chance(0.1f))
                    {
                        var candidates = remainedDefenders.Where(x => !pawnsWithAdditionalTrait?.Contains(x) ?? false);
                        if (candidates.Any() && candidates.TryRandomElement(out Pawn pawn))
                        {
                            TryDevelopNewTrait(pawn, "VSIE.ToughtRaidEvent".Translate());
                        }
                    }
                    this.raidGroups.Remove(raidGroup);
                }
            }
        }

        private bool RaidersWon(RaidGroup raidGroup, Map map)
        {
            foreach (var pawn in raidGroup.raiders)
            {
                if (!pawn.Dead && !pawn.Downed && pawn.CurJobDef != JobDefOf.Flee && pawn.MentalStateDef != MentalStateDefOf.PanicFlee
                    && !(pawn.GetLord()?.CurLordToil is LordToil_PanicFlee) && pawn.Map == map)
                {
                    return true;
                }
            }
            return false;
        }

        private static int workerTickRate = 60;
        public void WorkerTick(Pawn pawn)
        {
            if (pawn.IsHashIntervalTick(workerTickRate))
            {
                if (!this.pawnsWithWorkers.TryGetValue(pawn, out Workers workers))
                {
                    workers = new Workers();
                    this.pawnsWithWorkers[pawn] = workers;
                    workers.workersWithWorkingTicks = new Dictionary<Pawn, WorkTime>();
                }
                var nearestWorkers = pawn.Map.mapPawns.SpawnedPawnsInFaction(pawn.Faction).Where(x => x != pawn && VSIE_Utils.workTags.Contains(x.mindState.lastJobTag) && x.RaceProps.Humanlike && x.Position.DistanceTo(pawn.Position) < 10);
                foreach (var worker in nearestWorkers)
                {
                    if (workers.workersWithWorkingTicks.ContainsKey(worker))
                    {
                        if (Find.TickManager.TicksGame > workers.workersWithWorkingTicks[worker].lastTick + 10000)
                        {
                            workers.workersWithWorkingTicks[worker].workTick = 0;
                        }
                        else
                        {
                            workers.workersWithWorkingTicks[worker].workTick += workerTickRate;
                        }
                    }
                    else
                    {
                        var workTime = new WorkTime();
                        workTime.workTick = 0;
                        workers.workersWithWorkingTicks[worker] = workTime;
                    }
                    workers.workersWithWorkingTicks[worker].lastTick = Find.TickManager.TicksGame;
                }
            }
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
                aspiration.OnExpired();
                activeAspirations.Remove(pawn);
            }
        }

        public void TryDevelopNewTrait(Pawn pawn, string letterTextKey)
        {
            if (pawn.RaceProps.Humanlike && pawn.Faction == Faction.OfPlayer && !pawn.Dead)
            {
                var traits = DefDatabase<TraitDef>.AllDefsListForReading;
                var traitsCount = traits.Count;
                for (var i = 0; i <= traitsCount; i++)
                {
                    TraitDef newTraitDef = DefDatabase<TraitDef>.AllDefsListForReading.RandomElementByWeight((TraitDef tr) => tr.GetGenderSpecificCommonality(pawn.gender));
                    int degree = RandomTraitDegree(newTraitDef);
                    if (TraitIsAllowed(pawn, newTraitDef, degree))
                    {
                        Trait trait = new Trait(newTraitDef, degree);
                        if (pawn.mindState == null || pawn.mindState.mentalBreaker == null || !((pawn.mindState.mentalBreaker.BreakThresholdMinor + trait.OffsetOfStat(StatDefOf.MentalBreakThreshold)) * trait.MultiplierOfStat(StatDefOf.MentalBreakThreshold) > 0.5f))
                        {
                            pawn.story.traits.GainTrait(trait);
                            pawnsWithAdditionalTrait.Add(pawn);
                            var traitName = trait.CurrentData.GetLabelFor(pawn);
                            var traitDesc = trait.CurrentData.description.Formatted(pawn.Named("PAWN")).AdjustedFor(pawn).Resolve();
                            Find.LetterStack.ReceiveLetter("VSIE.TraitChangeTitle".Translate(traitName, pawn.Named("PAWN")), letterTextKey.Translate(traitName, traitDesc, pawn.Named("PAWN")), LetterDefOf.NeutralEvent, pawn);
                            return;
                        }
                    }
                    traits.Remove(newTraitDef);
                }
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

        public void RemoveDestroyedPawn(Pawn key)
        {
            activeAspirations.RemoveAll(x => x.Key == key);
            teachersWithPupils.RemoveAll(x => x.Key == key);
            pawnsWithWorkers.RemoveAll(x => x.Key == key);
            angryWorkers.RemoveAll(x => x.Key == key);
            birthdays.RemoveAll(x => x.Key == key);
            pawnsWithAdditionalTrait.Remove(key);
            honoredDeadPawns.Remove(key);
            joinedColonists.Remove(key);
        }
        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Collections.Look(ref activeAspirations, "activeAspirations", LookMode.Reference, LookMode.Deep, ref pawnKeys, ref aspirationValues);
            Scribe_Collections.Look(ref pawnsWithAdditionalTrait, "pawnsWithAdditionalTrait", LookMode.Reference);
            Scribe_Collections.Look(ref teachersWithPupils, "teachersWithPupils", LookMode.Reference, LookMode.Deep, ref pawnKeys2, ref teachingValues);
            Scribe_Collections.Look(ref pawnsWithWorkers, "pawnsWithWorkers", LookMode.Reference, LookMode.Deep, ref pawnKeys3, ref workerValues);
            Scribe_Collections.Look(ref angryWorkers, "angryWorkers", LookMode.Reference, LookMode.Value, ref pawnKeys4, ref intValues);
            Scribe_Collections.Look(ref birthdays, "birthdays", LookMode.Reference, LookMode.Value, ref pawnKeys5, ref intValues2);
            Scribe_Collections.Look(ref joinedColonists, "joinedColonists", LookMode.Reference, LookMode.Value, ref pawnKeys6, ref intValues3);
            Scribe_Collections.Look(ref raidGroups, "raidGroups", LookMode.Deep);
            Scribe_Collections.Look(ref honoredDeadPawns, "honoredDeadPawns", LookMode.Reference);
            Scribe_Values.Look(ref postRaidPeriodTicks, "postRaidPeriodTicks");
        }

        private List<Pawn> pawnKeys;
        private List<Aspiration> aspirationValues;

        private List<Pawn> pawnKeys2;
        private List<TeachingTopic> teachingValues;

        private List<Pawn> pawnKeys3;
        private List<Workers> workerValues;

        private List<Pawn> pawnKeys4;
        private List<int> intValues;

        private List<Pawn> pawnKeys5;
        private List<int> intValues2;

        private List<Pawn> pawnKeys6;
        private List<int> intValues3;
    }
}