using LRTF;

namespace TestFlight.LRTF
{
    public class LRTFFailure_ParachuteFail : LRTFFailureBase_Parachute
    {
        public override void DoFailure()
        {

            if (chute.deploymentState == ModuleParachute.deploymentStates.SEMIDEPLOYED || chute.deploymentState == ModuleParachute.deploymentStates.DEPLOYED)
            {
                chute.CutParachute();
            }
            else
            {
                chute.deploymentState = ModuleParachute.deploymentStates.STOWED;
                chute.enabled = false;
            }

            base.DoFailure();
        }

        public override float DoRepair()
        {
            base.DoRepair();
            if(chute != null)
                chute.enabled = true;

            return 0f;
        }
    }
}