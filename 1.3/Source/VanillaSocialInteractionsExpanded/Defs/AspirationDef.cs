using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace VanillaSocialInteractionsExpanded
{
    public class AspirationDef : Def
    {
        public InspirationDef inspirationDef;
        public string beginLetterContinue;
        public int workCount;
        public ThoughtDef thoughtDefSuccess;
        public ThoughtDef thoughtDefFail;
    }
}
