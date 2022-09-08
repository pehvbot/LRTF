using System.Collections.Generic;
using RealChute;
using UnityEngine;

namespace TestFlight.LRTF
{
    public class LRTFFailure_RealChutePartial : LRTFFailureBase_RealChute
    {
        private ConfigNode altitudes;

        public override void OnLoad(ConfigNode node)
        {
            if(node.HasNode("ALTITUDES"))
            {
                altitudes = node.GetNode("ALTITUDES");
            }
            else
            {
                altitudes = new ConfigNode().AddNode("ALTITUDES");
            }
            base.OnLoad(node);
        }

        public override void OnSave(ConfigNode node)
        {
            node.RemoveNodes("ALTITUDES");

            if (altitudes.HasData)
            {
                node.AddNode("ALTITUDES").AddData(altitudes);
            }
            base.OnSave(node);
        }

        public override void DoFailure()
        {
            Failed = true;
            base.DoFailure();

            foreach (Parachute p in chute.parachutes)
            {
                if(hasStarted)
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
            altitudes = new ConfigNode().AddNode("ALTITUDES");

            return 0f;
        }
    }
}