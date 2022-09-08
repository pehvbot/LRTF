using UnityEngine;

namespace TestFlight.LRTF
{
    public class LRTFFailure_RealChuteFARFail : LRTFFailureBase_RealChuteFAR
    {
        [KSPField(isPersistant = true)]
        bool GUIDisarm;
        [KSPField(isPersistant = true)]
        bool GUIDeploy;
        [KSPField(isPersistant = true)]
        bool GUIRepack;

        public override void DoFailure()
        {
            if (hasStarted)
            {
                GUIDisarm = chute.Events["GUIDisarm"].guiActive;
                GUIDeploy = chute.Events["GUIDeploy"].guiActive;
                GUIRepack = chute.Events["GUIRepack"].guiActive;
            }

            chute.Events["GUIDisarm"].guiActive = false;
            chute.Events["GUIDeploy"].guiActive = false;
            chute.Events["GUIRepack"].guiActive = false;

            chute.armed = false;
            if (chute.IsDeployed)
            {
                chute.Cut();
            }
            else
            {
                chute.DeactivateRC();
            }

            base.DoFailure();
        }

        public override float DoRepair()
        {
            base.DoRepair();
            chute.Events["GUIDisarm"].guiActive = GUIDisarm;
            chute.Events["GUIDeploy"].guiActive = GUIDeploy;
            chute.Events["GUIRepack"].guiActive = GUIRepack;
            chute.ActivateRC();
            
            return 0f;
        }
    }
}