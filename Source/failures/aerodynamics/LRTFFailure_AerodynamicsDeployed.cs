using UnityEngine;

namespace TestFlight.LRTF
{
    public class LRTFFailure_AerodynamicsDeployed : LRTFFailureBase_Aerodynamics
    {
        [KSPField(isPersistant = true)]
        bool startingDeployState;
        [KSPField(isPersistant = true)]
        float startingDeployAngle = 0;
        [KSPField(isPersistant = true)]
        float deployAngle = 0;
        
        public override void OnSave(ConfigNode node)
        {
            node.SetValue("startingDeployState", startingDeployState, true);
            node.SetValue("startingDeployAngle", startingDeployAngle, true);
            base.OnSave(node);
        }

        public override void OnLoad(ConfigNode node)
        {
            node.TryGetValue("startingDeployState", ref startingDeployState);
            node.TryGetValue("startingDeployAngle", ref startingDeployAngle);
            base.OnLoad(node);
        }

        public override void DoFailure()
        {
            if (hasStarted)
            {
                Vector2 limits = controlSurface.deployAngleLimits;
                deployAngle = (float)core.RandomGenerator.Next((int)limits[0], (int)limits[1]);
            }
            controlSurface.Fields["deploy"].guiActive = false;
            controlSurface.Fields["deployInvert"].guiActive = false;
            controlSurface.Fields["deployAngle"].guiActive = false;

            startingDeployAngle = controlSurface.deployAngle;
            controlSurface.deployAngle = deployAngle;

            controlSurface.deploy = true;

            base.DoFailure();
            if (part.PartActionWindow != null)
                part.PartActionWindow.displayDirty = true;
        }

        public override float DoRepair()
        {
            controlSurface.Fields["deploy"].guiActive = true;
            controlSurface.Fields["deployInvert"].guiActive = true;
            controlSurface.Fields["deployAngle"].guiActive = true;
            controlSurface.deploy = startingDeployState;
            controlSurface.deployAngle = startingDeployAngle;

            return base.DoRepair();
        }
    }
}
