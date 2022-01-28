using TestFlightAPI;

namespace TestFlight.LRTF
{
    public class LRTFDataRecorder_Engine : LRTFDataRecorderBase
    {
        private EngineModuleWrapper engine;

        public override void OnStart(StartState state)
        {
            base.OnStart(state);
            engine = new EngineModuleWrapper();
            engine.Init(this.part);
        }

        public override bool IsPartOperating()
        {
            if (!isEnabled || !HighLogic.CurrentGame.Parameters.CustomParams<LRTFGameSettings>().lrtfEngines || TimeWarp.CurrentRate > 4)
                return false;

            return engine.IgnitionState == EngineModuleWrapper.EngineIgnitionState.IGNITED;
        }

        public override bool IsRecordingFlightData()
        {
            if (vessel.situation == Vessel.Situations.PRELAUNCH)
                return false;
            return IsPartOperating();
        }
    }
}

