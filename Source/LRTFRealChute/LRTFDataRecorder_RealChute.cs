using TestFlightAPI;
using UnityEngine;
using RealChute;

[assembly: KSPAssemblyDependency("RealChute",0,0)]

namespace TestFlight.LRTF
{
    public class LRTFDataRecorder_RealChute : LRTFDataRecorderBase
    {
        RealChuteModule chute;

        public override void OnStart(PartModule.StartState state)
        {
            base.OnStart(state);
            chute = part.FindModuleImplementing<RealChuteModule>();
            
            if (chute == null)
            {
                isEnabled = false;
                Debug.Log("[LRTF] RealChuteModule not found for " + part.name + "!  Recording will be disabled for this part!");
            }
        }

        public override bool IsPartOperating()
        {
            if (!isEnabled || !HighLogic.CurrentGame.Parameters.CustomParams<LRTFGameSettings>().lrtfParachutes || TimeWarp.CurrentRate > 4)
                return false;

            return chute.armed || chute.AnyDeployed;
        }

        public override bool IsRecordingFlightData()
        {
            return vessel.situation != Vessel.Situations.PRELAUNCH && IsPartOperating();
        }

    }
}
