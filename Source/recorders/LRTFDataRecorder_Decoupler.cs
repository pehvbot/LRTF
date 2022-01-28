using TestFlightAPI;

namespace TestFlight.LRTF
{
    public class LRTFDataRecorder_Decoupler : LRTFDataRecorderBase
    {
        private LRTFModuleAnchoredDecoupler anchoredDecoupler;
        private LRTFModuleDecouple decouple;

        public override void OnStart(StartState state)
        {
            base.OnStart(state);
            foreach (var m in part.FindModulesImplementing<LRTFModuleDecouple>())
                decouple = m;
            foreach (var m in part.FindModulesImplementing<LRTFModuleAnchoredDecoupler>())
                anchoredDecoupler = m;
        }

        public override bool IsPartOperating()
        {
            if (isEnabled && HighLogic.CurrentGame.Parameters.CustomParams<LRTFGameSettings>().lrtfDecouplers && TimeWarp.CurrentRate <= 4)
            {
                if (anchoredDecoupler != null)
                    return !anchoredDecoupler.isDecoupled;
                if (decouple != null)
                    return !decouple.isDecoupled;
            }
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