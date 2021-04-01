using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace VanillaSocialInteractionsExpanded
{
    class VanillaSocialInteractionsExpandedSettings : ModSettings
    {
        public static bool EnableMemories = true;
        public static bool EnableInspirations = true;
        public static bool EnableAspirations = true;
        public static bool EnableObtainingNewTraits = true;
        public static bool EnableVenting = true;
        public static bool EnableBestFriend = true;
        public static bool EnableTeaching = true;
        public static bool EnableManyHandsMakeLightWork = true;
        public static bool EnableDiscord = true;
        public static bool EnableGroupActivities = true;
        public static bool EnableDating = true;
        public static bool EnableUnitedWeStand = true;
        public static bool EnableSocialEnvironmentThoughts = true;
        public static bool EnableOneNightStand = true;
        public override void ExposeData()
        {
            Scribe_Values.Look<bool>(ref EnableMemories, "EnableMemories", true);
            Scribe_Values.Look<bool>(ref EnableInspirations, "EnableInspirations", true);
            Scribe_Values.Look<bool>(ref EnableAspirations, "EnableAspirations", true);
            Scribe_Values.Look<bool>(ref EnableObtainingNewTraits, "EnableObtainingNewTraits", true);
            Scribe_Values.Look<bool>(ref EnableVenting, "EnableVenting", true);
            Scribe_Values.Look<bool>(ref EnableBestFriend, "EnableBestFriend", true);
            Scribe_Values.Look<bool>(ref EnableTeaching, "EnableTeaching", true);
            Scribe_Values.Look<bool>(ref EnableManyHandsMakeLightWork, "EnableManyHandsMakeLightWork", true);
            Scribe_Values.Look<bool>(ref EnableDiscord, "EnableDiscord", true);
            Scribe_Values.Look<bool>(ref EnableGroupActivities, "EnableGroupActivities", true);
            Scribe_Values.Look<bool>(ref EnableDating, "EnableDating", true);
            Scribe_Values.Look<bool>(ref EnableUnitedWeStand, "EnableUnitedWeStand", true);
            Scribe_Values.Look<bool>(ref EnableSocialEnvironmentThoughts, "EnableSocialEnvironmentThoughts", true);
            Scribe_Values.Look<bool>(ref EnableOneNightStand, "EnableOneNightStand", true);
            base.ExposeData();
        }

        public void DoSettingsWindowContents(Rect inRect)
        {
            Listing_Standard listingStandard = new Listing_Standard();
            listingStandard.Begin(inRect);
            listingStandard.CheckboxLabeled("VSIE.EnableMemories".Translate(), ref EnableMemories);
            listingStandard.CheckboxLabeled("VSIE.EnableInspirations".Translate(), ref EnableInspirations);
            listingStandard.CheckboxLabeled("VSIE.EnableAspirations".Translate(), ref EnableAspirations);
            listingStandard.CheckboxLabeled("VSIE.EnableObtainingNewTraits".Translate(), ref EnableObtainingNewTraits);
            listingStandard.CheckboxLabeled("VSIE.EnableVenting".Translate(), ref EnableVenting);
            listingStandard.CheckboxLabeled("VSIE.EnableBestFriend".Translate(), ref EnableBestFriend);
            listingStandard.CheckboxLabeled("VSIE.EnableTeaching".Translate(), ref EnableTeaching);
            listingStandard.CheckboxLabeled("VSIE.EnableManyHandsMakeLightWork".Translate(), ref EnableManyHandsMakeLightWork);
            listingStandard.CheckboxLabeled("VSIE.EnableDiscord".Translate(), ref EnableDiscord);
            listingStandard.CheckboxLabeled("VSIE.EnableGroupActivities".Translate(), ref EnableGroupActivities);
            listingStandard.CheckboxLabeled("VSIE.EnableDating".Translate(), ref EnableDating);
            listingStandard.CheckboxLabeled("VSIE.EnableUnitedWeStand".Translate(), ref EnableUnitedWeStand);
            listingStandard.CheckboxLabeled("VSIE.EnableSocialEnvironmentThoughts".Translate(), ref EnableSocialEnvironmentThoughts);
            listingStandard.CheckboxLabeled("VSIE.EnableOneNightStand".Translate(), ref EnableOneNightStand);
            listingStandard.End();
            base.Write();
        }
    }
}
