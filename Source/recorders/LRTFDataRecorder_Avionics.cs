using TestFlightAPI;
using UnityEngine;

namespace TestFlight
{

    public class LRTFDataRecorder_Avionics : LRTFDataRecorderBase
    {
        private ModuleCommand command;

        public override void OnStart(StartState state)
        {
            base.OnStart(state);
            command = part.Modules.GetModule<ModuleCommand>();
        }
        public override bool IsPartOperating()
        {
            if (!(isEnabled && HighLogic.CurrentGame.Parameters.CustomParams<LRTFGameSettings>().lrtfAvionics))
                return false;

            //hibernating or high warp protects the probe
            if (!command.IsHibernating && TimeWarp.CurrentRate <= 4)
                if (command.ModuleState == ModuleCommand.ModuleControlState.Nominal || command.ModuleState == ModuleCommand.ModuleControlState.PartialProbe)
                    return true;

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