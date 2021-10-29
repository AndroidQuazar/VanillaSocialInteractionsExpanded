using HarmonyLib;
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
	[StaticConstructorOnStartup]
	internal static class HarmonyInit
	{
		public static Harmony harmony;
		static HarmonyInit()
		{
			harmony = new Harmony("OskarPotocki.VanillaSocialInteractionsExpanded");
			harmony.PatchAll();
		}
	}
}
