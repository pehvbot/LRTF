using LRTF;

namespace TestFlight.LRTF
{
    public class LRTFFailure_RealChuteFARPartial : LRTFFailureBase_RealChuteFAR
    {
        [KSPField(isPersistant = true)]
        public float deployAltitude;

        public override void DoFailure()
        {
            deployAltitude = ModWrapper.FerramWrapper.GetDeployAltitude(chute);
            ModWrapper.FerramWrapper.SetDeploymentAltitude(chute, 0f);
            chute.Fields["deployAltitude"].guiActive = false;

            if (ModWrapper.FerramWrapper.GetDeploymentState(chute) == ModWrapper.FerramWrapper.DeploymentStates.DEPLOYED)
                ModWrapper.FerramWrapper.AssumeDragCubePosition(chute, "SEMIDEPLOYED");

            base.DoFailure();
        }

        public override float DoRepair()
        {
            base.DoRepair();

            ModWrapper.FerramWrapper.SetDeploymentAltitude(chute, deployAltitude);
            chute.Fields["deployAltitude"].guiActive = true;
            if (ModWrapper.FerramWrapper.GetDeploymentState(chute) == ModWrapper.FerramWrapper.DeploymentStates.DEPLOYED)
                ModWrapper.FerramWrapper.AssumeDragCubePosition(chute, "DEPLOYED");
            return 0f;
        }
    }
}