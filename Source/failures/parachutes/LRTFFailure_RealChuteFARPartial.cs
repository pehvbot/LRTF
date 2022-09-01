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
                ModWrapper.FerramWrapper.PreDeploy(chute);

            base.DoFailure();
        }

        public override float DoRepair()
        {
            base.DoRepair();

            ModWrapper.FerramWrapper.SetDeploymentAltitude(chute, deployAltitude);
            chute.Fields["deployAltitude"].guiActive = true;
            //ModWrapper.FerramWrapper.DeployChute(chute);
            return 0f;
        }
    }
}