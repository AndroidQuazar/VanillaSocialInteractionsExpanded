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
    public class JobDriver_SkygazeTalking : JobDriver
    {
        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
            return true;
        }
        protected override IEnumerable<Toil> MakeNewToils()
        {
            yield return Toils_Goto.GotoCell(TargetIndex.A, PathEndMode.OnCell);
            Toil toil = new Toil();
            toil.initAction = delegate
            {
                pawn.jobs.posture = PawnPosture.LayingOnGroundFaceUp;
            };
            toil.tickAction = delegate
            {
                float extraJoyGainFactor = GatheringWorker_Skygazing.AggregateSkyGazeChanceFactor(pawn.Map.gameConditionManager, pawn.Map);
                JoyUtility.JoyTickCheckEnd(pawn, JoyTickFullJoyAction.None, extraJoyGainFactor);
            };
            toil.socialMode = RandomSocialMode.SuperActive;
            toil.defaultCompleteMode = ToilCompleteMode.Delay;
            toil.defaultDuration = 4000;
            toil.FailOn(() => pawn.Position.Roofed(pawn.Map));
            toil.FailOn(() => !JoyUtility.EnjoyableOutsideNow(pawn));
            yield return toil;
        }

        public override string GetReport()
        {
            LocalTargetInfo a = job.targetA.IsValid ? job.targetA : job.targetQueueA.FirstValid();
            LocalTargetInfo b = job.targetB.IsValid ? job.targetB : job.targetQueueB.FirstValid();
            LocalTargetInfo targetC = job.targetC;
            return JobUtility.GetResolvedJobReport(BaseReport(), a, b, targetC);
        }
        public string BaseReport()
        {
            if (base.Map.gameConditionManager.ConditionIsActive(GameConditionDefOf.Eclipse))
            {
                return "WatchingEclipse".Translate();
            }
            if (base.Map.gameConditionManager.ConditionIsActive(GameConditionDefOf.Aurora))
            {
                return "WatchingAurora".Translate();
            }
            float num = GenCelestial.CurCelestialSunGlow(base.Map);
            if (num < 0.1f)
            {
                return "Stargazing".Translate();
            }
            if (num < 0.65f)
            {
                if (GenLocalDate.DayPercent(pawn) < 0.5f)
                {
                    return "WatchingSunrise".Translate();
                }
                return "WatchingSunset".Translate();
            }
            return "CloudWatching".Translate();
        }
    }
}