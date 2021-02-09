﻿using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

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


	[HarmonyPatch(typeof(TaleRecorder), "RecordTale")]
	public class RecordTale_Patch
	{
		private static void Postfix(Tale __result)
		{
			if (__result != null)
			{
				Log.Message($"Recording new tale: {__result}");
			}
		}
	}
}
