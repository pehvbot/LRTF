using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

using UnityEngine;

using TestFlightAPI;

namespace TestFlight
{
    public class LRTFGameSettings : GameParameters.CustomParameterNode
    {
        public override string Title { get { return "Less Real Options"; } }
        public override GameParameters.GameMode GameMode { get { return GameParameters.GameMode.ANY; } }
        public override string Section { get { return "TestFlight"; } }
        public override string DisplaySection { get { return Section; } }
        public override int SectionOrder { get { return 2; } }
        public override bool HasPresets { get { return true; } }

        [GameParameters.CustomParameterUI("Leave TimeWarp On Failure?", toolTip = "Set to leave TimeWarp on failure.")]
        public bool lrtfLeaveTimeWarp = false;

        [GameParameters.CustomParameterUI("Send Message On Failure?", toolTip = "Send a message to Messages on failure.")]
        public bool lrtfSendMessage = false;

        [GameParameters.CustomParameterUI("Enable Repairs?", toolTip = "Allows parts to be repaired.")]
        public bool lrtfEnableRepair = false;

        [GameParameters.CustomFloatParameterUI("Repair Adjuster", asPercentage = false, minValue = 1, maxValue = 5, stepCount = 4)]
        public float lrtfRepairAdjuster = 3f;

        [GameParameters.CustomStringParameterUI("Failure Modes", autoPersistance = true, lines = 2, title = "\n<b>Failure Modes</b>", toolTip = "Enable or disable data recording and part failures")]
        public string UIstring = "";

        //[GameParameters.CustomParameterUI("Engines", toolTip = "Enables and disableds engine data recording and part failures")]
        //public bool lrtfEngines = true;

        [GameParameters.CustomParameterUI("Resources/Tanks", toolTip = "Enables and disableds resource and tank data recording and part failures")]
        public bool lrtfResources = true;

        [GameParameters.CustomParameterUI("Avionics/Command", toolTip = "Enables and disableds avionics/command data recording and part failures")]
        public bool lrtfAvionics = true;

        [GameParameters.CustomParameterUI("RCS", toolTip = "Enables and disableds RCS data recording and part failures")]
        public bool lrtfRCS = true;

        [GameParameters.CustomParameterUI("Reaction Wheels", toolTip = "Enables and disableds reaction wheel data recording and part failures")]
        public bool lrtfReaction = true;

        [GameParameters.CustomParameterUI("Parachutes", toolTip = "Enables and disableds parachutes data recording and part failures")]
        public bool lrtfParachutes = true;

        public override void SetDifficultyPreset(GameParameters.Preset preset)
        {
            switch (preset)
            {
                case GameParameters.Preset.Easy:
                    lrtfAvionics = false;
                    //lrtfEngines = false;
                    lrtfRCS = false;
                    lrtfReaction = false;
                    lrtfResources = false;
                    lrtfParachutes = false;
                    break;
                case GameParameters.Preset.Normal:
                case GameParameters.Preset.Moderate:
                case GameParameters.Preset.Hard:
                    break;
            }
        }
    }
}
