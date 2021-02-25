using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;
using Verse.AI.Group;
using Verse.Grammar;

namespace VanillaSocialInteractionsExpanded
{
    public class LordJob_Joinable_GrabbingBeer : LordJob_Joinable_Gathering
    {
        public Pawn secondPawn;

        private int durationTicks;
        public override bool AllowStartNewGatherings => false;

        protected virtual ThoughtDef AttendeeThought => VSIE_DefOf.VSIE_HadNiceChatWithBeer;

        protected virtual TaleDef AttendeeTale => TaleDefOf.AttendedParty;

        protected virtual ThoughtDef OrganizerThought => VSIE_DefOf.VSIE_HadNiceChatWithBeer;

        protected virtual TaleDef OrganizerTale => TaleDefOf.AttendedParty;

        public int DurationTicks => durationTicks;

        public override bool LostImportantReferenceDuringLoading
        {
            get
            {
                if (organizer != null)
                {
                    return secondPawn == null;
                }
                return true;
            }
        }

        public LordJob_Joinable_GrabbingBeer()
        {
        }

        public LordJob_Joinable_GrabbingBeer(Pawn firstPawn, Pawn secondPawn, IntVec3 spot, GatheringDef gatheringDef, int durationTicks)
        {
            Log.Message("LordJob_Joinable_GrabbingBeer : LordJob_Joinable_Gathering - LordJob_Joinable_GrabbingBeer - this.organizer = firstPawn; - 1", true);
            this.organizer = firstPawn;
            Log.Message("LordJob_Joinable_GrabbingBeer : LordJob_Joinable_Gathering - LordJob_Joinable_GrabbingBeer - this.secondPawn = secondPawn; - 2", true);
            this.secondPawn = secondPawn;
            Log.Message("LordJob_Joinable_GrabbingBeer : LordJob_Joinable_Gathering - LordJob_Joinable_GrabbingBeer - this.spot = spot; - 3", true);
            this.spot = spot;
            Log.Message("LordJob_Joinable_GrabbingBeer : LordJob_Joinable_Gathering - LordJob_Joinable_GrabbingBeer - this.gatheringDef = gatheringDef; - 4", true);
            this.gatheringDef = gatheringDef;
            this.durationTicks = durationTicks;
        }

        protected override LordToil CreateGatheringToil(IntVec3 spot, Pawn organizer, GatheringDef gatheringDef)
        {
            Log.Message("LordJob_Joinable_GrabbingBeer : LordJob_Joinable_Gathering - CreateGatheringToil - return new LordToil_Party(spot, gatheringDef); - 6", true);
            return new LordToil_Party(spot, gatheringDef);
        }
        public override StateGraph CreateGraph()
        {
            Log.Message("LordJob_Joinable_GrabbingBeer : LordJob_Joinable_Gathering - CreateGraph - StateGraph stateGraph = new StateGraph(); - 7", true);
            StateGraph stateGraph = new StateGraph();
            Log.Message("LordJob_Joinable_GrabbingBeer : LordJob_Joinable_Gathering - CreateGraph - LordToil party = CreateGatheringToil(spot, base.organizer, gatheringDef); - 8", true);
            LordToil party = CreateGatheringToil(spot, base.organizer, gatheringDef);
            Log.Message("LordJob_Joinable_GrabbingBeer : LordJob_Joinable_Gathering - CreateGraph - stateGraph.AddToil(party); - 9", true);
            stateGraph.AddToil(party);
            Log.Message("LordJob_Joinable_GrabbingBeer : LordJob_Joinable_Gathering - CreateGraph - LordToil_End lordToil_End = new LordToil_End(); - 10", true);
            LordToil_End lordToil_End = new LordToil_End();
            Log.Message("LordJob_Joinable_GrabbingBeer : LordJob_Joinable_Gathering - CreateGraph - stateGraph.AddToil(lordToil_End); - 11", true);
            stateGraph.AddToil(lordToil_End);
            //Transition transition = new Transition(party, lordToil_End);
            //transition.AddTrigger(new Trigger_TickCondition(() => ShouldBeCalledOff()));
            //transition.AddTrigger(new Trigger_PawnKilled());
            //transition.AddTrigger(new Trigger_PawnLost(PawnLostCondition.LeftVoluntarily, base.organizer));
            //transition.AddPreAction(new TransitionAction_Custom((Action)delegate
            //{
            //    ApplyOutcome((LordToil_Party)party);
            //}));
            //transition.AddPreAction(new TransitionAction_Message(gatheringDef.calledOffMessage, MessageTypeDefOf.NegativeEvent, new TargetInfo(spot, base.Map)));
            //stateGraph.AddTransition(transition);
            Log.Message("LordJob_Joinable_GrabbingBeer : LordJob_Joinable_Gathering - CreateGraph - timeoutTrigger = GetTimeoutTrigger(); - 20", true);
            timeoutTrigger = GetTimeoutTrigger();
            Log.Message("LordJob_Joinable_GrabbingBeer : LordJob_Joinable_Gathering - CreateGraph - Transition transition2 = new Transition(party, lordToil_End); - 21", true);
            Transition transition2 = new Transition(party, lordToil_End);
            Log.Message("LordJob_Joinable_GrabbingBeer : LordJob_Joinable_Gathering - CreateGraph - transition2.AddTrigger(timeoutTrigger); - 22", true);
            transition2.AddTrigger(timeoutTrigger);
            transition2.AddPreAction(new TransitionAction_Custom((Action)delegate
            {
                Log.Message("LordJob_Joinable_GrabbingBeer : LordJob_Joinable_Gathering - CreateGraph - ApplyOutcome((LordToil_Party)party); - 23", true);
                ApplyOutcome((LordToil_Party)party);
                Log.Message("LordJob_Joinable_GrabbingBeer : LordJob_Joinable_Gathering - CreateGraph - })); - 24", true);
            }));
            Log.Message("LordJob_Joinable_GrabbingBeer : LordJob_Joinable_Gathering - CreateGraph - transition2.AddPreAction(new TransitionAction_Message(gatheringDef.finishedMessage, MessageTypeDefOf.SituationResolved, new TargetInfo(spot, base.Map))); - 25", true);
            transition2.AddPreAction(new TransitionAction_Message(gatheringDef.finishedMessage, MessageTypeDefOf.SituationResolved, new TargetInfo(spot, base.Map)));
            Log.Message("LordJob_Joinable_GrabbingBeer : LordJob_Joinable_Gathering - CreateGraph - stateGraph.AddTransition(transition2); - 26", true);
            stateGraph.AddTransition(transition2);
            Log.Message("LordJob_Joinable_GrabbingBeer : LordJob_Joinable_Gathering - CreateGraph - return stateGraph; - 27", true);
            return stateGraph;
        }

        protected virtual Trigger_TicksPassed GetTimeoutTrigger()
        {
            return new Trigger_TicksPassed(durationTicks);
        }

        private void ApplyOutcome(LordToil_Party toil)
        {
            Log.Message("LordJob_Joinable_GrabbingBeer : LordJob_Joinable_Gathering - ApplyOutcome - List<Pawn> ownedPawns = lord.ownedPawns; - 29", true);
            List<Pawn> ownedPawns = lord.ownedPawns;
            Log.Message("LordJob_Joinable_GrabbingBeer : LordJob_Joinable_Gathering - ApplyOutcome - LordToilData_Party lordToilData_Party = (LordToilData_Party)toil.data; - 30", true);
            LordToilData_Party lordToilData_Party = (LordToilData_Party)toil.data;
            for (int i = 0; i < ownedPawns.Count; i++)
            {
                Log.Message("LordJob_Joinable_GrabbingBeer : LordJob_Joinable_Gathering - ApplyOutcome - Pawn pawn = ownedPawns[i]; - 31", true);
                Pawn pawn = ownedPawns[i];
                Log.Message("LordJob_Joinable_GrabbingBeer : LordJob_Joinable_Gathering - ApplyOutcome - bool flag = pawn == base.organizer; - 32", true);
                bool flag = pawn == base.organizer;
                if (lordToilData_Party.presentForTicks.TryGetValue(pawn, out int value) && value > 0)
                {
                    Log.Message("LordJob_Joinable_GrabbingBeer : LordJob_Joinable_Gathering - ApplyOutcome - if (ownedPawns[i].needs.mood != null) - 34", true);
                    if (ownedPawns[i].needs.mood != null)
                    {
                        Log.Message("LordJob_Joinable_GrabbingBeer : LordJob_Joinable_Gathering - ApplyOutcome - ThoughtDef thoughtDef = flag ? OrganizerThought : AttendeeThought; - 35", true);
                        ThoughtDef thoughtDef = flag ? OrganizerThought : AttendeeThought;
                        Log.Message("LordJob_Joinable_GrabbingBeer : LordJob_Joinable_Gathering - ApplyOutcome - float num = 0.5f / thoughtDef.stages[0].baseMoodEffect; - 36", true);
                        float num = 0.5f / thoughtDef.stages[0].baseMoodEffect;
                        float moodPowerFactor = Mathf.Min((float)value / (float)durationTicks + num, 1f);
                        Log.Message("LordJob_Joinable_GrabbingBeer : LordJob_Joinable_Gathering - ApplyOutcome - Thought_Memory thought_Memory = (Thought_Memory)ThoughtMaker.MakeThought(thoughtDef); - 38", true);
                        Thought_Memory thought_Memory = (Thought_Memory)ThoughtMaker.MakeThought(thoughtDef);
                        Log.Message("LordJob_Joinable_GrabbingBeer : LordJob_Joinable_Gathering - ApplyOutcome - thought_Memory.moodPowerFactor = moodPowerFactor; - 39", true);
                        thought_Memory.moodPowerFactor = moodPowerFactor;
                        Log.Message("LordJob_Joinable_GrabbingBeer : LordJob_Joinable_Gathering - ApplyOutcome - ownedPawns[i].needs.mood.thoughts.memories.TryGainMemory(thought_Memory); - 40", true);
                        ownedPawns[i].needs.mood.thoughts.memories.TryGainMemory(thought_Memory);
                    }
                    TaleRecorder.RecordTale(flag ? OrganizerTale : AttendeeTale, ownedPawns[i], base.organizer);
                }
            }
        }
        public override float VoluntaryJoinPriorityFor(Pawn p)
        {
            if (p == organizer || p == secondPawn)
            {
                return 20f;
            }
            return 0f;
        }

        public override void ExposeData()
        {
            Scribe_References.Look(ref secondPawn, "secondPawn");
            Scribe_Values.Look(ref spot, "spot");
            Scribe_Values.Look(ref durationTicks, "durationTicks", 0);
            if (Scribe.mode == LoadSaveMode.PostLoadInit && gatheringDef == null)
            {
                gatheringDef = VSIE_DefOf.VSIE_GrabbingBeer;
            }
        }
    }
}