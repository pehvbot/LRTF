using TestFlightAPI;
using UnityEngine;

namespace TestFlight.LRTF
{
    public class LRTFDataRecorder_Parachutes : LRTFDataRecorderBase
    {
        ModuleParachute chute;
        PartModule far;

        public override void OnStart(PartModule.StartState state)
        {
            base.OnStart(state);
            chute = part.FindModuleImplementing<ModuleParachute>();
            foreach(var p in part.Modules)
            {
                if (p.moduleName == "RealChuteFAR")
                    far = p;
            }
            if (chute == null && far == null)
            {
                isEnabled = false;
                Debug.Log("[LRTF] ModuleParachute not found for " + part.name + "!  Recording will be disabled for this part!");
            }
        }

        public override bool IsPartOperating()
        {
            if (!isEnabled || !HighLogic.CurrentGame.Parameters.CustomParams<LRTFGameSettings>().lrtfParachutes || TimeWarp.CurrentRate > 4)
                return false;

            return (chute.deploymentState == ModuleParachute.deploymentStates.ACTIVE ||
                chute.deploymentState == ModuleParachute.deploymentStates.SEMIDEPLOYED ||
                chute.deploymentState == ModuleParachute.deploymentStates.DEPLOYED);
        }

        public override bool IsRecordingFlightData()
        {
            if (vessel.situation == Vessel.Situations.PRELAUNCH)
                return false;
            return IsPartOperating();
        }
    }
}
