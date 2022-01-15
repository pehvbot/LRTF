using System;
using System.Collections.Generic;
using UnityEngine;

using TestFlightAPI;

namespace TestFlight
{
    public class LRTFDataRecorder_Engine : FlightDataRecorderBase
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
            if (vessel.situation == Vessel.Situations.PRELAUNCH || !isEnabled || !HighLogic.CurrentGame.Parameters.CustomParams<LRTFGameSettings>().lrtfEngines || TimeWarp.CurrentRate > 4)
                return false;

            return engine.IgnitionState == EngineModuleWrapper.EngineIgnitionState.IGNITED;
        }

        public override bool IsRecordingFlightData()
        {
            return IsPartOperating();
        }
    }
}

