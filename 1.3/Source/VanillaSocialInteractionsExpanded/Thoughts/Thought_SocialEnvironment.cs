using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace VanillaSocialInteractionsExpanded
{
	public class Thought_SocialEnvironment : Thought_Situational
    {
        public override float MoodOffset()
        {
            var averageOpinionOf = VSIE_Utils.GetAverageOpinionOf(pawn);
            if (averageOpinionOf >= 10)
            {
                return averageOpinionOf / 10f;
            }
            if (averageOpinionOf < 0)
            {
                return averageOpinionOf / 5f;
            }
            return base.MoodOffset();
        }
    }
}
