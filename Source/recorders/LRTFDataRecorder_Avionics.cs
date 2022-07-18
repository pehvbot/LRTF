using TestFlightAPI;
using UnityEngine;

namespace TestFlight.LRTF
{
    public class LRTFDataRecorder_Avionics : LRTFDataRecorderBase
    {
        private ModuleCommand command;

        public override void OnStart(StartState state)
        {
            base.OnStart(state);
            command = part.Modules.GetModule<ModuleCommand>();
            if (command == null)
            {
                isEnabled = false;
                Debug.Log("[LRTF] ModuleCommand not found for " + part.name + "!  Recording will be disabled for this part!");
            }
        }
        public override bool IsPartOperating()
        {
            //hibernating or high warp protects the probe
            if (isEnabled && HighLogic.CurrentGame.Parameters.CustomParams<LRTFGameSettings>().lrtfAvionics && !command.IsHibernating && TimeWarp.CurrentRate <= 4)
                return command.ModuleState == ModuleCommand.ModuleControlState.Nominal || command.ModuleState == ModuleCommand.ModuleControlState.PartialProbe;

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