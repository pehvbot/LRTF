using TestFlightAPI;
using System.Linq;
using UnityEngine;

namespace TestFlight.LRTF
{
    public class LRTFDataRecorder_RCS : LRTFDataRecorderBase
    {
        private ModuleRCS rcs;

        public override void OnStart(StartState state)
        {
            base.OnStart(state);
            rcs = base.part.FindModuleImplementing<ModuleRCS>();
            if (rcs == null)
            {
                isEnabled = false;
                Debug.Log("[LRTF] ModuleRCS not found for " + part.name + "!  Recording will be disabled for this part!");
            }
        }

        public override bool IsPartOperating()
        {
            if (!isEnabled || !HighLogic.CurrentGame.Parameters.CustomParams<LRTFGameSettings>().lrtfRCS || TimeWarp.CurrentRate > 4)
                return false;

            //records only when active
            return rcs.enabled && rcs.thrustForces.Max() > 0; 

        }

        public override bool IsRecordingFlightData()
        {
            if (this.part.vessel.situation == Vessel.Situations.PRELAUNCH)
                return false;

            return IsPartOperating();
        }
    }
}