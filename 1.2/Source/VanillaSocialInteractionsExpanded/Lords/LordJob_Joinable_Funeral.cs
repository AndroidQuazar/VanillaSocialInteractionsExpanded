using RimWorld;
using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;
using Verse.AI.Group;

namespace VanillaSocialInteractionsExpanded
{
	public class LordJob_Joinable_Funeral : LordJob_Joinable_Gathering
	{
		private int durationTicks;
		public override bool AllowStartNewGatherings => false;
		protected virtual ThoughtDef AttendeeThought => VSIE_DefOf.VSIE_GotSomeClosure;
		protected virtual TaleDef AttendeeTale => TaleDefOf.AttendedParty;

		public int DurationTicks => durationTicks;

		private Building_Grave grave;
		public LordJob_Joinable_Funeral()
		{
		}

		public LordJob_Joinable_Funeral(IntVec3 spot, Pawn organizer, Building_Grave grave, GatheringDef gatheringDef)
			: base(spot, organizer, gatheringDef)
		{
			durationTicks = Rand.RangeInclusive(10000, 15000);
			this.grave = grave;
		}

		public override string GetReport(Pawn pawn)
		{
			return "LordReportAttendingParty".Translate();
		}

		protected override LordToil CreateGatheringToil(IntVec3 spot, Pawn organizer, GatheringDef gatheringDef)
		{
			return new LordToil_Party(spot, gatheringDef);
		}

		public override StateGraph CreateGraph()
		{
			StateGraph stateGraph = new StateGraph();
			LordToil party = CreateGatheringToil(spot, organizer, gatheringDef);
			stateGraph.AddToil(party);
			LordToil_End lordToil_End = new LordToil_End();
			stateGraph.AddToil(lordToil_End);
			Transition transition = new Transition(party, lordToil_End);
			transition.AddTrigger(new Trigger_TickCondition(() => ShouldBeCalledOff()));
			transition.AddTrigger(new Trigger_PawnKilled());
			transition.AddTrigger(new Trigger_PawnLost(PawnLostCondition.LeftVoluntarily, organizer));
			transition.AddPreAction(new TransitionAction_Custom((Action)delegate
			{
				ApplyOutcome((LordToil_Party)party);
			}));
			transition.AddPreAction(new TransitionAction_Message(gatheringDef.calledOffMessage, MessageTypeDefOf.NegativeEvent, new TargetInfo(spot, base.Map)));
			stateGraph.AddTransition(transition);
			timeoutTrigger = GetTimeoutTrigger();
			Transition transition2 = new Transition(party, lordToil_End);
			transition2.AddTrigger(timeoutTrigger);
			transition2.AddPreAction(new TransitionAction_Custom((Action)delegate
			{
				ApplyOutcome((LordToil_Party)party);
			}));
			transition2.AddPreAction(new TransitionAction_Message(gatheringDef.finishedMessage, MessageTypeDefOf.SituationResolved, new TargetInfo(spot, base.Map)));
			stateGraph.AddTransition(transition2);
			return stateGraph;
		}

		protected virtual Trigger_TicksPassed GetTimeoutTrigger()
		{
			return new Trigger_TicksPassed(durationTicks);
		}

		private void ApplyOutcome(LordToil_Party toil)
		{
			VSIE_Utils.SocialInteractionsManager.honoredDeadPawns.Add(this.grave.Corpse.InnerPawn);
			List<Pawn> ownedPawns = lord.ownedPawns;
			LordToilData_Party lordToilData_Party = (LordToilData_Party)toil.data;
			for (int i = 0; i < ownedPawns.Count; i++)
			{
				Pawn pawn = ownedPawns[i];
				if (lordToilData_Party.presentForTicks.TryGetValue(pawn, out int value) && value > 0)
				{
					if (ownedPawns[i].needs.mood != null)
					{
						ownedPawns[i].needs.mood.thoughts.memories.TryGainMemory(AttendeeThought);

						MemoryThoughtHandler memories = ownedPawns[i].needs.mood.thoughts.memories;
						memories.RemoveMemoriesOfDefWhereOtherPawnIs(ThoughtDefOf.KnowColonistDied, this.grave.Corpse.InnerPawn);
						memories.RemoveMemoriesOfDefWhereOtherPawnIs(ThoughtDefOf.KnowPrisonerDiedInnocent, this.grave.Corpse.InnerPawn);
						memories.RemoveMemoriesOfDefWhereOtherPawnIs(ThoughtDefOf.PawnWithGoodOpinionDied, this.grave.Corpse.InnerPawn);
						memories.RemoveMemoriesOfDefWhereOtherPawnIs(ThoughtDefOf.PawnWithBadOpinionDied, this.grave.Corpse.InnerPawn);
						List<PawnRelationDef> allDefsListForReading = DefDatabase<PawnRelationDef>.AllDefsListForReading;
						for (int j = 0; j < allDefsListForReading.Count; j++)
						{
							ThoughtDef genderSpecificDiedThought = allDefsListForReading[j].GetGenderSpecificDiedThought(this.grave.Corpse.InnerPawn);
							if (genderSpecificDiedThought != null)
							{
								memories.RemoveMemoriesOfDefWhereOtherPawnIs(genderSpecificDiedThought, this.grave.Corpse.InnerPawn);
							}
						}
					}
				}
			}
		}

		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Values.Look(ref durationTicks, "durationTicks", 0);
			Scribe_References.Look(ref grave, "grave");
			if (Scribe.mode == LoadSaveMode.PostLoadInit && gatheringDef == null)
			{
				gatheringDef = VSIE_DefOf.VSIE_Funeral;
			}
		}
	}
}
