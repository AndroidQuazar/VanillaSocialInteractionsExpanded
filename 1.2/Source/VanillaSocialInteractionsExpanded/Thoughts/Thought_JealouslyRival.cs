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
	public class Thought_JealouslyRival : Thought_MemorySocial
	{
		public override string LabelCap
		{
			get
			{
				return base.LabelCap;
			}
		}

		public override float OpinionOffset()
		{
			return 0f;
		}
	}
}
