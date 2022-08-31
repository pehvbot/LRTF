using TestFlightAPI;
using UnityEngine;
using LRTF;
using FerramAerospaceResearch.RealChuteLite;

namespace TestFlight.LRTF
{
    public class LRTFDataRecorder_RealChuteFAR : LRTFDataRecorderBase
    {
        PartModule chute;

        public override void OnStart(PartModule.StartState state)
        {
            base.OnStart(state);
            chute = part.FindModuleImplementing<ModuleParachute>();
            foreach(var p in part.Modules)
            {
                if (p.moduleName == "RealChuteFAR")
                    chute = p;
            }
            if (chute == null)
            {
                isEnabled = false;
                Debug.Log("[LRTF] RealChuteFAR not found for " + part.name + "!  Recording will be disabled for this part!");
            }
            RealChuteFAR f = new RealChuteFAR();
        }

        public override bool IsPartOperating()
        {
            if (!isEnabled || !HighLogic.CurrentGame.Parameters.CustomParams<LRTFGameSettings>().lrtfParachutes || TimeWarp.CurrentRate > 4)
                return false;

            return ModWrapper.FerramWrapper.IsDeployed(chute);
        }

        public override bool IsRecordingFlightData()
        {
            if (vessel.situation == Vessel.Situations.PRELAUNCH)
                return false;
            return IsPartOperating();
        }
    }
}
