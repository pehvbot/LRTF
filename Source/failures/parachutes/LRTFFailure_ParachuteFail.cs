using LRTF;

namespace TestFlight.LRTF
{
    public class LRTFFailure_ParachuteFail : LRTFFailureBase_Parachute
    {
        public override void DoFailure()
        {
            if(far != null)
            {
                if(ModWrapper.FerramWrapper.IsDeployed(far))
                {
                    ModWrapper.FerramWrapper.CutChute(far);
                    far.Events["GUIRepack"].active = false;
                }
                else
                {
                    //ModWrapper.FerramWrapper.DeployChute(far); //Will deploy the chute right away, ignoring chutes min altitude/pressure. 
                    far.Events["GUIDisarm"].active = false;
                    far.Events["GUIRepack"].active = false;
                }
            }

            if (chute != null)
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
            }
            base.DoFailure();
        }

        public override float DoRepair()
        {
            base.DoRepair();
            if(far != null)
            {
                far.Events["GUIDisarm"].active = true;
                far.Events["GUIRepack"].active = true;
            }
            else if(chute != null)
                chute.enabled = true;

            return 0f;
        }
    }
}