using TestFlightAPI;
using System.Linq;
using UnityEngine;

namespace TestFlight
{
    public class LRTFDataRecorder_Decoupler : LRTFDataRecorderBase
    {
        public override void OnStart(StartState state)
        {
            base.OnStart(state);
        }

        public override bool IsPartOperating()
        {
            if (isEnabled && HighLogic.CurrentGame.Parameters.CustomParams<LRTFGameSettings>().lrtfDecouplers
                && this.part.vessel.situation != Vessel.Situations.PRELAUNCH && TimeWarp.CurrentRate <= 4)
                return true;
            return false;
        }

        public override bool IsRecordingFlightData()
        {
            return IsPartOperating();
        }
    }
}