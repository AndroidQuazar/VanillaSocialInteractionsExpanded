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
    class VanillaSocialInteractionsExpandedMod : Mod
    {
        public static VanillaSocialInteractionsExpandedSettings settings;
        public VanillaSocialInteractionsExpandedMod(ModContentPack pack) : base(pack)
        {
            settings = GetSettings<VanillaSocialInteractionsExpandedSettings>();
            ApplySettings();
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

        public override void WriteSettings()
        {
            base.WriteSettings();
            ApplySettings();
        }

        private void ApplySettings()
        {
            if (!VanillaSocialInteractionsExpandedSettings.EnableInspirations)
            {
                var inspirationDefs = DefDatabase<InspirationDef>.AllDefsListForReading.Where(x => x.defName.StartsWith("VSIE_"));
                foreach (var def in inspirationDefs)
                {
                    DefDatabase<InspirationDef>.AllDefsListForReading.Remove(def);
                }
            }
        }
    }
}
