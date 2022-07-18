using TestFlightAPI;
using UnityEngine;

namespace TestFlight.LRTF
{
    public class LRTFDataRecorder_Communications : LRTFDataRecorderBase
    {
        private ModuleDataTransmitter transmitter;
        private ModuleDeployableAntenna antenna;

        public override void OnStart(StartState state)
        {
            base.OnStart(state);
            transmitter = part.Modules.GetModule<ModuleDataTransmitter>();
            foreach(var v in part.Modules.GetModules<ModuleDeployableAntenna>())
            {
                antenna = v;
            }
            if (transmitter == null)
            {
                isEnabled = false;
                Debug.Log("[LRTF] ModuleDataTransmitter not found for " + part.name + "!  Recording will be disabled for this part!");
            }
        }
        public override bool IsPartOperating()
        {
            if (!(isEnabled && HighLogic.CurrentGame.Parameters.CustomParams<LRTFGameSettings>().lrtfCommunications) || TimeWarp.CurrentRate > 4)
                return false;

            if (antenna == null)
                return transmitter.CanTransmit();
            else if (antenna.deployState == ModuleDeployablePart.DeployState.EXTENDED)
                return transmitter.CanTransmit();
            else
                return false;
        }

        public override bool IsRecordingFlightData()
        {
            if (part.vessel.situation == Vessel.Situations.PRELAUNCH)
                return false;

            return IsPartOperating();
        }
    }
}