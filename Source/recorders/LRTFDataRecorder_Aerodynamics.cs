using TestFlightAPI;
using System.Linq;
using UnityEngine;

namespace TestFlight
{


    public class LRTFDataRecorder_Aerodynamics : LRTFDataRecorderBase
    {

        [KSPField]
        public double minDynamicPressureRec = 1.0;

        private ModuleControlSurface controlSurface;

        public override void OnStart(StartState state)
        {
            base.OnStart(state);
            this.controlSurface = base.part.FindModuleImplementing<ModuleControlSurface>();
        }

        public override bool IsPartOperating()
        {
            if (!(isEnabled && HighLogic.CurrentGame.Parameters.CustomParams<LRTFGameSettings>().lrtfAerodynamics))
                return false;

            //records only when active
            return controlSurface.enabled && vessel.dynamicPressurekPa > minDynamicPressureRec;

        }

        public override bool IsRecordingFlightData()
        {
            if (this.part.vessel.situation == Vessel.Situations.PRELAUNCH)
                return false;

            return IsPartOperating();
        }
    }
}