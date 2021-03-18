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
	public class InteractionWorker_Discord : InteractionWorker
	{
		public override float RandomSelectionWeight(Pawn initiator, Pawn recipient)
		{
            if (VanillaSocialInteractionsExpandedSettings.EnableDiscord)
            {
                var socialManager = VSIE_Utils.SocialInteractionsManager;
                if (socialManager.angryWorkers != null && socialManager.angryWorkers.TryGetValue(initiator, out int lastTick))
                {
                    if (lastTick + (GenDate.TicksPerHour * 12) > Find.TickManager.TicksGame)
                    {
                        var value = 1f * NegativeInteractionUtility.NegativeInteractionChanceFactor(initiator, recipient);
                        Log.Message($"Discord chance for {initiator} and {recipient} is {value}");
                        return value;
                    }
                    Log.Message($"Discord chance for {initiator} and {recipient} is null (fail)");
                }
            }

            return 0f;
		}
	}
}