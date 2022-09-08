using UnityEngine;

namespace TestFlight.LRTF
{
    public class LRTFFailure_RealChuteFARPartial : LRTFFailureBase_RealChuteFAR
    {
        [KSPField(isPersistant = true)]
        public float deployAltitude;

        public override void DoFailure()
        {
            if(hasStarted)
                deployAltitude = chute.deployAltitude;

            chute.deployAltitude = 0f;
            chute.Fields["deployAltitude"].guiActive = false;

            if (chute.DeploymentState == FerramAerospaceResearch.RealChuteLite.RealChuteFAR.DeploymentStates.DEPLOYED)
                chute.PreDeploy();

            base.DoFailure();
            Debug.Log("[LRTF] b:" + deployAltitude);
        }

        public override float DoRepair()
        {
            base.DoRepair();

            chute.deployAltitude = deployAltitude;
            chute.Fields["deployAltitude"].guiActive = true;
            return 0f;
        }
    }
}