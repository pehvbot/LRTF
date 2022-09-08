namespace TestFlight.LRTF
{
    public class LRTFFailure_ParachutePartial : LRTFFailureBase_Parachute
    {
        [KSPField(isPersistant = true)]
        public float deployAltitude;

        public override void DoFailure()
        {
            deployAltitude = chute.deployAltitude;
            chute.deployAltitude = 0;
            chute.Fields["deployAltitude"].guiActive = false;

            if (chute.deploymentState == ModuleParachute.deploymentStates.DEPLOYED)
                chute.AssumeDragCubePosition("SEMIDEPLOYED");

            base.DoFailure();
        }

        public override float DoRepair()
        {
            base.DoRepair();

            chute.deployAltitude = deployAltitude;
            chute.Fields["deployAltitude"].guiActive = true;
            if (chute.deploymentState == ModuleParachute.deploymentStates.DEPLOYED)
                chute.AssumeDragCubePosition("DEPLOYED");

            return 0f;
        }
    }
}