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
	public class JobDriver_BuildSnowmanTogether : JobDriver
	{
		public float workLeft = -1000f;
		public override bool TryMakePreToilReservations(bool errorOnFailed)
		{
			if (!VSIE_Utils.GetCompanion(pawn).HasReserved(job.GetTarget(TargetIndex.A)))
			{
				return pawn.Reserve(job.GetTarget(TargetIndex.A), job, 1, -1, null, errorOnFailed);
			}
			return true;
		}

		protected override IEnumerable<Toil> MakeNewToils()
		{
			Toil toil = new Toil();
			toil.initAction = delegate
			{
				var companion = VSIE_Utils.GetCompanion(pawn);
				if (companion?.CurJobDef == this.job.def && CellFinder.TryFindRandomCellNear(job.targetA.Cell, pawn.Map, 1, 
					(IntVec3 cell) => cell != companion.CurJob.targetA.Cell && cell.Walkable(pawn.Map), out IntVec3 result))
				{
					toil.actor.pather.StartPath(result, PathEndMode.Touch);
				}
				else
                {
					toil.actor.pather.StartPath(job.targetA, PathEndMode.Touch);
				}
			};
			toil.defaultCompleteMode = ToilCompleteMode.PatherArrival;
			yield return toil;
			yield return Toils_Goto.Goto(TargetIndex.A, PathEndMode.Touch);
			Toil doWork = new Toil();
			doWork.initAction = delegate
			{
				var companion = VSIE_Utils.GetCompanion(pawn);
				if (companion.CurJobDef == this.job.def && (companion.jobs.curDriver as JobDriver_BuildSnowmanTogether).workLeft != -1000)
                {
					workLeft = (companion.jobs.curDriver as JobDriver_BuildSnowmanTogether).workLeft;
				}
				else
                {
					workLeft = 2300f;
                }
			};
			doWork.tickAction = delegate
			{
				var companion = VSIE_Utils.GetCompanion(pawn);
				workLeft -= doWork.actor.GetStatValue(StatDefOf.ConstructionSpeed) * 1.7f;
				if (companion.jobs.curDriver is JobDriver_BuildSnowmanTogether driver && driver.workLeft != -1000)
                {
					driver.workLeft--;
				}
				if (workLeft <= 0f)
				{
					if (base.TargetLocA.GetFirstThing(pawn.Map, ThingDefOf.Snowman) is null)
                    {
						Thing thing = ThingMaker.MakeThing(ThingDefOf.Snowman);
						thing.SetFaction(pawn.Faction);
						GenSpawn.Spawn(thing, base.TargetLocA, base.Map);
					}
					ReadyForNextToil();
				}
				else
				{
					JoyUtility.JoyTickCheckEnd(pawn, JoyTickFullJoyAction.None);
				}
			};
			doWork.defaultCompleteMode = ToilCompleteMode.Never;
			doWork.FailOn(() => !JoyUtility.EnjoyableOutsideNow(pawn));
			doWork.FailOnCannotTouch(TargetIndex.A, PathEndMode.Touch);
			doWork.activeSkill = (() => SkillDefOf.Construction);
			doWork.socialMode = RandomSocialMode.SuperActive;
			yield return doWork;
		}

		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Values.Look(ref workLeft, "workLeft", 0f);
		}
	}
}

