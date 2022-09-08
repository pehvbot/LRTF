using UnityEngine;
using RealChute;

namespace TestFlight.LRTF
{
    public class LRTFFailure_RealChuteFail : LRTFFailureBase_RealChute
    {
        [KSPField(isPersistant = true)]
        bool GUIRepack;

        public override void DoFailure()
        {
            base.DoFailure();
            if(hasStarted)
                GUIRepack = chute.Events["GUIRepack"].guiActive;

            chute.Events["GUIRepack"].guiActive = false;
            foreach (Parachute p in chute.parachutes)
            {
                if (p.IsDeployed)
                    p.Cut();
            }
            chute.DeactivateRC();
            
        }

        public override float DoRepair()
        {
            base.DoRepair();
            chute.Events["GUIRepack"].guiActive = GUIRepack;

            chute.ActivateRC();
            
            return 0f;
        }
    }
}