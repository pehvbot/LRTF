using TestFlightAPI;
using UnityEngine;

namespace TestFlight
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
            if (this.part.vessel.situation == Vessel.Situations.PRELAUNCH)
                return false;

            return IsPartOperating();
        }
    }
}