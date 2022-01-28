using TestFlightAPI;

namespace TestFlight.LRTF
{
    public class LRTFDataRecorder_Parachutes : LRTFDataRecorderBase
    {
        ModuleParachute chute;

        public override void OnStart(PartModule.StartState state)
        {
            base.OnStart(state);
            chute = base.part.FindModuleImplementing<ModuleParachute>();
        }

        public override bool IsPartOperating()
        {
            if (!isEnabled || !HighLogic.CurrentGame.Parameters.CustomParams<LRTFGameSettings>().lrtfParachutes || TimeWarp.CurrentRate > 4)
                return false;

            return (chute.deploymentState == ModuleParachute.deploymentStates.ACTIVE || chute.deploymentState == ModuleParachute.deploymentStates.SEMIDEPLOYED || chute.deploymentState == ModuleParachute.deploymentStates.DEPLOYED);
        }

        public override bool IsRecordingFlightData()
        {
            if (vessel.situation == Vessel.Situations.PRELAUNCH)
                return false;
            return IsPartOperating();
        }
    }
}
