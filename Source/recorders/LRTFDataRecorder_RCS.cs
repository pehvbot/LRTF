using TestFlightAPI;
using System.Linq;
using UnityEngine;

namespace TestFlight
{

  
    public class LRTFDataRecorder_RCS : FlightDataRecorderBase
    {
        private ModuleRCS rcs;

        public override void OnStart(StartState state)
        {
            base.OnStart(state);
            this.rcs = base.part.FindModuleImplementing<ModuleRCS>();
        }

        public override bool IsPartOperating()
        {
            if (!(isEnabled && HighLogic.CurrentGame.Parameters.CustomParams<LRTFGameSettings>().lrtfRCS))
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