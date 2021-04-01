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

    public class LordJob_Joinable_DoublePawn : LordJob_Joinable_Gathering
    {
        public Pawn secondPawn;

        private int durationTicks;
        private int startTicks;
        public override bool AllowStartNewGatherings => false;
        protected virtual ThoughtDef AttendeeThought => null;
        protected virtual TaleDef AttendeeTale => TaleDefOf.AttendedParty;
        protected virtual ThoughtDef OrganizerThought => null;

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

        public LordJob_Joinable_DoublePawn()
        {
        }

        public LordJob_Joinable_DoublePawn(Pawn firstPawn, Pawn secondPawn, IntVec3 spot, GatheringDef gatheringDef, int durationTicks)
        {
            this.organizer = firstPawn;
            this.secondPawn = secondPawn;
            this.spot = spot;
            this.gatheringDef = gatheringDef;
            this.durationTicks = durationTicks;
            this.startTicks = Find.TickManager.TicksGame;
        }

        protected override LordToil CreateGatheringToil(IntVec3 spot, Pawn organizer, GatheringDef gatheringDef)
        {
            return new LordToil_Party(spot, gatheringDef);
        }
        public override StateGraph CreateGraph()
        {
            StateGraph stateGraph = new StateGraph();
            LordToil party = CreateGatheringToil(spot, base.organizer, gatheringDef);
            stateGraph.AddToil(party);
            LordToil_End lordToil_End = new LordToil_End();
            stateGraph.AddToil(lordToil_End);

            Transition transition = new Transition(party, lordToil_End);
            transition.AddTrigger(new Trigger_TickCondition(() => ShouldBeCalledOff()));
            transition.AddTrigger(new Trigger_PawnKilled());
            transition.AddTrigger(new Trigger_PawnLost(PawnLostCondition.LeftVoluntarily, base.organizer));
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

        public override void LordJobTick()
        {
            base.LordJobTick();
            if (Find.TickManager.TicksGame > this.startTicks + this.durationTicks + 100 && this.lord.CurLordToil is LordToil_Party toil) // in order to end infinite dates
            {
                ApplyOutcome(toil);
                this.Map.lordManager.RemoveLord(this.lord);
            }
        }
        protected virtual void ApplyOutcome(LordToil_Party toil)
        {
            List<Pawn> ownedPawns = lord.ownedPawns;
            LordToilData_Party lordToilData_Party = (LordToilData_Party)toil.data;
            for (int i = 0; i < ownedPawns.Count; i++)
            {
                try
                {
                    Pawn pawn = ownedPawns[i];
                    bool flag = pawn == base.organizer;
                    if (lordToilData_Party.presentForTicks.TryGetValue(pawn, out int value) && value > 0)
                    {
                        if (ownedPawns[i].needs?.mood != null)
                        {
                            try
                            {
                                ThoughtDef thoughtDef = flag ? OrganizerThought : AttendeeThought;
                                if (thoughtDef != null)
                                {
                                    float num = 0.5f / thoughtDef.stages[0].baseMoodEffect;
                                    float moodPowerFactor = Mathf.Min((float)value / (float)durationTicks + num, 1f);
                                    Thought_Memory thought_Memory = (Thought_Memory)ThoughtMaker.MakeThought(thoughtDef);
                                    thought_Memory.moodPowerFactor = moodPowerFactor;
                                    ownedPawns[i].needs.mood.thoughts.memories.TryGainMemory(thought_Memory);
                                }
                            }
                            catch { }
                        }
                    }
                }
                catch { }
            }

            try
            {
                var organizerPartner = organizer.GetSpouseOrLoverOrFiance();
                if (organizerPartner != null && secondPawn != organizerPartner)
                {
                    Thought_Memory thought_Memory = (Thought_Memory)ThoughtMaker.MakeThought(VSIE_DefOf.VSIE_JealouslyMyPartnerDatedSomeoneElse);
                    organizerPartner.needs.mood.thoughts.memories.TryGainMemory(thought_Memory, organizer);
                }
                var secondPawnPartner = secondPawn.GetSpouseOrLoverOrFiance();
                if (secondPawnPartner != null && organizer != secondPawnPartner)
                {
                    Thought_Memory thought_Memory = (Thought_Memory)ThoughtMaker.MakeThought(VSIE_DefOf.VSIE_JealouslyMyPartnerDatedSomeoneElse);
                    secondPawnPartner.needs.mood.thoughts.memories.TryGainMemory(thought_Memory, secondPawn);
                }
            }
            catch { };
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
            base.ExposeData();
            Scribe_References.Look(ref secondPawn, "secondPawn");
            Scribe_Values.Look(ref spot, "spot");
            Scribe_Values.Look(ref durationTicks, "durationTicks", 0);
            Scribe_Values.Look(ref startTicks, "startTicks", 0);
        }
    }
}