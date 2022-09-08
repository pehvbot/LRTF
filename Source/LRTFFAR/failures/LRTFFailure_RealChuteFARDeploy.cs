using TestFlightAPI;
using UnityEngine;

namespace TestFlight.LRTF.LRTFFAR
{
    public class LRTFFailure_RealChuteFARDeploy : LRTFFailureBase_RealChuteFAR
    {
        [KSPField]
        public FloatCurve deploymentChanceCurve;

        [KSPField(guiName = "Parachute Deployment Chance", groupName = "LRTF", groupDisplayName = "Less Real Test Flight", guiActive = true)]
        private string deploymentChanceString;

        private double deploymentChance;

        [KSPField(isPersistant = true)]
        private bool parachuteActive = false;

        [KSPField(isPersistant = true)]
        bool GUIDisarm;
        [KSPField(isPersistant = true)]
        bool GUIDeploy;
        [KSPField(isPersistant = true)]
        bool GUIRepack;

        public override void OnLoad(ConfigNode node)
        {
            if (deploymentChanceCurve == null)
            {
                deploymentChanceCurve = new FloatCurve();
                if (node.HasNode("deploymentChanceCurve"))
                    deploymentChanceCurve.Load(node.GetNode("deploymentChanceCurve"));
                else
                    deploymentChanceCurve.Add(0f, 1f);
            }
            
            base.OnLoad(node);
        }

        public override void OnStart(PartModule.StartState state)
        {
            base.OnStart(state);
            core.DisableFailure(this.moduleName);

            deploymentChance = deploymentChanceCurve.Evaluate(core.GetFlightData());
            deploymentChanceString = $"{deploymentChance:P}";
        }

        public override void OnUpdate()
        {
            if (!parachuteActive && HighLogic.CurrentGame.Parameters.CustomParams<LRTFGameSettings>().lrtfParachutes && (chute.armed || chute.IsDeployed))
            {
                parachuteActive = true;
                if (deploymentChance < core.RandomGenerator.NextDouble())
                {
                    core.TriggerNamedFailure(this.moduleName);
                    Failed = true;
                }
            }
        }

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

            chute.DeactivateRC();
            chute.armed = false;
            core.ModifyFlightData(duFail, true);
            deploymentChanceString = failureTitle;
            base.DoFailure();
        }

        public override float DoRepair()
        {
            base.DoRepair();
            parachuteActive = false;

            chute.Events["GUIDisarm"].guiActive = GUIDisarm;
            chute.Events["GUIDeploy"].guiActive = GUIDeploy;
            chute.Events["GUIRepack"].guiActive = GUIRepack;
            chute.ActivateRC();

            deploymentChanceString = $"{deploymentChance:P}";
            return 0f;
        }
    }
}