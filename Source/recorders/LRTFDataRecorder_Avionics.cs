using TestFlightAPI;
using UnityEngine;

namespace TestFlight
{

    public class LRTFDataRecorder_Avionics : FlightDataRecorderBase
    {
        public override bool IsPartOperating()
        {
            if (!(isEnabled && HighLogic.CurrentGame.Parameters.CustomParams<LRTFGameSettings>().lrtfAvionics))
                return false;

            PartModuleList mods = this.part.Modules;
            ModuleCommand mod = (ModuleCommand)mods.GetModule("ModuleCommand");
            //hibernating or high warp protects the probe
            if (!mod.IsHibernating && TimeWarp.CurrentRate <= 4)
                if (mod.ModuleState == ModuleCommand.ModuleControlState.Nominal || mod.ModuleState == ModuleCommand.ModuleControlState.PartialProbe)
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