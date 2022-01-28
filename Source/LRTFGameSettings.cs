namespace TestFlight.LRTF
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
        public bool lrtfLeaveTimeWarp = true;

        [GameParameters.CustomParameterUI("Send Message On Failure?", toolTip = "Send a message to Messages on failure.")]
        public bool lrtfSendMessage = true;

        [GameParameters.CustomParameterUI("Open PAW On Failure?", toolTip = "Opens the Part Action Window on failure.")]
        public bool lrtfOpenPAW = true;

        [GameParameters.CustomParameterUI("Enable Repairs?", toolTip = "Allows parts to be repaired.")]
        public bool lrtfEnableRepair = true;

        [GameParameters.CustomFloatParameterUI("Repair Difficulty Adjuster", toolTip = "Adjusts the difficulty of repairs.  Higher is more difficult.", asPercentage = false, minValue = 1, maxValue = 5, stepCount = 4)]
        public float lrtfRepairAdjuster = 3f;

        [GameParameters.CustomStringParameterUI("Failure Modes", autoPersistance = true, lines = 2, title = "\n<b>Failure Modes</b>", toolTip = "Enable or disable data recording and part failures")]
        public string UIstring = "";

        [GameParameters.CustomParameterUI("Engines", toolTip = "Enables and disableds engine data recording and part failures")]
        public bool lrtfEngines = true;

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

        [GameParameters.CustomParameterUI("Wheels", toolTip = "Enables and disableds wheel data recording and part failures")]
        public bool lrtfWheels = true;

        [GameParameters.CustomParameterUI("Aerodynamics", toolTip = "Enables and disableds aerodynamic data recording and part failures")]
        public bool lrtfAerodynamics = true;

        [GameParameters.CustomParameterUI("Communications", toolTip = "Enables and disableds communication/antenna data recording and part failures")]
        public bool lrtfCommunications = true;

        [GameParameters.CustomParameterUI("Decouplers", toolTip = "Enables and disableds decoupler data recording and part failures")]
        public bool lrtfDecouplers = true;

        public override void SetDifficultyPreset(GameParameters.Preset preset)
        {
            switch (preset)
            {
                case GameParameters.Preset.Easy:
                    lrtfAvionics = false;
                    lrtfEngines = false;
                    lrtfRCS = false;
                    lrtfReaction = false;
                    lrtfResources = false;
                    lrtfParachutes = false;
                    lrtfWheels = false;
                    lrtfAerodynamics = false;
                    lrtfCommunications = false;
                    lrtfDecouplers = false;
                    lrtfRepairAdjuster = 2;
                    break;
                case GameParameters.Preset.Normal:
                    break;
                case GameParameters.Preset.Moderate:
                    lrtfRepairAdjuster = 4;
                    break;
                case GameParameters.Preset.Hard:
                    lrtfRepairAdjuster = 5;
                    break;
            }
        }
    }
}
