using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace VanillaSocialInteractionsExpanded
{
    class VanillaSocialInteractionsExpandedMod : Mod
    {
        public static VanillaSocialInteractionsExpandedSettings settings;
        public VanillaSocialInteractionsExpandedMod(ModContentPack pack) : base(pack)
        {
            settings = GetSettings<VanillaSocialInteractionsExpandedSettings>();
        }

        public override void DoSettingsWindowContents(Rect inRect)
        {
            base.DoSettingsWindowContents(inRect);
            settings.DoSettingsWindowContents(inRect);
        }

        public override string SettingsCategory()
        {
            return "Vanilla Social Interactions Expanded";
        }
    }
}
