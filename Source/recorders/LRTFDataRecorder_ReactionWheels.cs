﻿using TestFlightAPI;

namespace TestFlight.LRTF
{
    public class LRTFDataRecorder_ReactionWheel : LRTFDataRecorderBase
    {
        private ModuleReactionWheel module;

        public override void OnStart(PartModule.StartState state)
        {
            base.OnStart(state);
            module = base.part.FindModuleImplementing<ModuleReactionWheel>();
        }

        public override bool IsPartOperating()
        {
            if (!(isEnabled && HighLogic.CurrentGame.Parameters.CustomParams<LRTFGameSettings>().lrtfReaction))
                return false;

            return module.wheelState == ModuleReactionWheel.WheelState.Active && module.inputSum > 0.01;
        }

        public override bool IsRecordingFlightData()
        {
            if (this.part.vessel.situation == Vessel.Situations.PRELAUNCH)
                return false;

            return IsPartOperating();
        }
    }
}
