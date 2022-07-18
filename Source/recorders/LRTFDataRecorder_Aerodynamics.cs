using TestFlightAPI;
using UnityEngine;

namespace TestFlight.LRTF
{
    public class LRTFDataRecorder_Aerodynamics : LRTFDataRecorderBase
    {
        [KSPField]
        public double minDynamicPressureRec = 1.0;

        private ModuleControlSurface controlSurface;

        public override void OnStart(StartState state)
        {
            base.OnStart(state);
            controlSurface = part.FindModuleImplementing<ModuleControlSurface>();
            if(controlSurface == null)
            {
                isEnabled = false;
                Debug.Log("[LRTF] ModuleControlSurface not found for " + part.name + "!  Recording will be disabled for this part!");
            }
        }

        public override bool IsPartOperating()
        {
            if (!(isEnabled && HighLogic.CurrentGame.Parameters.CustomParams<LRTFGameSettings>().lrtfAerodynamics && controlSurface))
                return false;

            //records only when active
            return controlSurface.enabled && vessel.dynamicPressurekPa > minDynamicPressureRec;

        }

        public override bool IsRecordingFlightData()
        {
            if (part.vessel.situation == Vessel.Situations.PRELAUNCH)
                return false;

            return IsPartOperating();
        }
    }
}