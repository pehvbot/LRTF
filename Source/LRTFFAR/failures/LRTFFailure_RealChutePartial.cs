using System.Collections.Generic;
using RealChute;
using UnityEngine;

namespace TestFlight.LRTF.LRTFRealChute
{
    public class LRTFFailure_RealChutePartial : LRTFFailureBase_RealChute
    {
        private ConfigNode altitudes;

        public override void DoFailure()
        {
            failed = true;
            Failed = true;
            base.DoFailure();

            altitudes = new ConfigNode();

            foreach (Parachute p in chute.parachutes)
            {
                altitudes.AddValue(p.parachuteName, p.deploymentAlt);
                p.deploymentAlt = 0f;
                if (p.DeploymentState == DeploymentStates.DEPLOYED || p.DeploymentState == DeploymentStates.LOWDEPLOYED)
                {
                    p.PreDeploy();
                    p.DeploymentState = DeploymentStates.PREDEPLOYED;
                }
            }
        }

        public override float DoRepair()
        {
            base.DoRepair();

            foreach(Parachute p in chute.parachutes)
            {
                p.deploymentAlt = float.Parse(altitudes.GetValue(p.parachuteName));
            }

            return 0f;
        }
    }
}