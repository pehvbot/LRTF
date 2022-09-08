using TestFlightAPI;
using UnityEngine;
using FerramAerospaceResearch.RealChuteLite;

[assembly: KSPAssemblyDependency("FerramAerospaceResearch", 0, 0)]

namespace TestFlight.LRTF
{
    public class LRTFDataRecorder_RealChuteFAR : LRTFDataRecorderBase
    {
        RealChuteFAR chute;

        public override void OnStart(PartModule.StartState state)
        {
            base.OnStart(state);
            chute = part.FindModuleImplementing<RealChuteFAR>();
            if (chute == null)
            {
                isEnabled = false;
                Debug.Log("[LRTF] RealChuteFAR not found for " + part.name + "!  Recording will be disabled for this part!");
            }
        }

        public override bool IsPartOperating()
        {
            if (!isEnabled || !HighLogic.CurrentGame.Parameters.CustomParams<LRTFGameSettings>().lrtfParachutes || TimeWarp.CurrentRate > 4)
                return false;

            return chute.armed || chute.IsDeployed;
        }

        public override bool IsRecordingFlightData()
        {
            if (vessel.situation == Vessel.Situations.PRELAUNCH)
                return false;
            return IsPartOperating();
        }
    }
}
