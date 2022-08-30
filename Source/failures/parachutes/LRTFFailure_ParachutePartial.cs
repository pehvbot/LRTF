using LRTF;

namespace TestFlight.LRTF
{
    public class LRTFFailure_ParachuPartial : LRTFFailureBase_Parachute
    {
        [KSPField(isPersistant = true)]
        public float deployAltitude;

        public override void DoFailure()
        {
            if(far != null)
            {
                ModWrapper.FerramWrapper.ReefChute(far);
                far.Fields["deployAltitude"].guiActive = false;
            }
            if (chute != null)
            {
                deployAltitude = chute.deployAltitude;
                chute.deployAltitude = 0;
                chute.Fields["deployAltitude"].guiActive = false;

                if (chute.deploymentState == ModuleParachute.deploymentStates.DEPLOYED)
                    chute.AssumeDragCubePosition("SEMIDEPLOYED");
            }
            base.DoFailure();

        }

        public override float DoRepair()
        {
            base.DoRepair();

            if (chute != null)
            {
                chute.deployAltitude = deployAltitude;
                chute.Fields["deployAltitude"].guiActive = true;
                if (chute.deploymentState == ModuleParachute.deploymentStates.DEPLOYED)
                    chute.AssumeDragCubePosition("DEPLOYED");
            }
            return 0f;
        }
    }
}