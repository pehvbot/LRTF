using TestFlightAPI;
using System.Linq;
using UnityEngine;

namespace TestFlight
{

  
    public class FlightDataRecorder_LRRCS : FlightDataRecorderBase
    {
        private ModuleRCS rcs;

        public override void OnStart(StartState state)
        {
            base.OnStart(state);
            this.rcs = base.part.FindModuleImplementing<ModuleRCS>();
        }

        public override bool IsPartOperating()
        {
            if (!isEnabled)
                return false;

            //records only when active
            return rcs.thrustForces.Max()  >  0.1 * rcs.thrusterPower;

        }

        public override bool IsRecordingFlightData()
        {
            if (this.part.vessel.situation == Vessel.Situations.PRELAUNCH)
                return false;

            return IsPartOperating();
        }
    }
}