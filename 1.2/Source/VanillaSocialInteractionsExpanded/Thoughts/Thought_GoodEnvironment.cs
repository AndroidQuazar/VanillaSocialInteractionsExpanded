﻿using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace VanillaSocialInteractionsExpanded
{
	public class Thought_GoodEnvironment : Thought_Situational
    {
        public override float MoodOffset()
        {
            var averageOpinionOf = VSIE_Utils.AverageOpinionOf(pawn);
            if (averageOpinionOf >= 10)
            {
                return averageOpinionOf / 10f;
            }
            return base.MoodOffset();
        }
    }
}