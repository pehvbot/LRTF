using LRTF;

namespace TestFlight.LRTF
{
    public class LRTFFailure_RealChuteFARFail : LRTFFailureBase_RealChuteFAR
    {
        public override void DoFailure()
        {
            if(ModWrapper.FerramWrapper.IsDeployed(chute))
            {
                ModWrapper.FerramWrapper.CutChute(chute);
            }
            else
            {
                //chute.deploymentState = ModuleParachute.deploymentStates.STOWED;
                chute.enabled = false;
            }

            base.DoFailure();
        }

        public override float DoRepair()
        {
            base.DoRepair();
            chute.enabled = true;

            return 0f;
        }
    }
}