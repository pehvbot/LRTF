using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using TestFlightAPI;

namespace TestFlight
{
    public class LRTFFailure_ParachuteDeploy : LRTFFailureBase
    {
        [KSPField]
        public FloatCurve deploymentChanceCurve;

        [KSPField(guiName = "Parachute Deployment Chance", groupName = "LRTF", groupDisplayName = "Less Real Test Flight", guiActive = true)]
        private string deploymentChanceString;

        private ModuleParachute chute;
        private double deploymentChance;

        [KSPField(isPersistant = true)]
        private bool parachuteActive = false;
        
       
        public override void OnLoad(ConfigNode node)
        {
            if (deploymentChanceCurve == null)
            {
                deploymentChanceCurve = new FloatCurve();
                if (node.HasNode("deploymentChanceCurve"))
                    deploymentChanceCurve.Load(node.GetNode("deploymentChanceCurve"));
                else
                    deploymentChanceCurve.Add(0f, 0.5f);
            }
            
            base.OnLoad(node);
        }

        public override void OnStart(PartModule.StartState state)
        {
            base.OnStart(state);
            core.DisableFailure(this.moduleName);

            this.chute = base.part.FindModuleImplementing<ModuleParachute>();

            deploymentChance = deploymentChanceCurve.Evaluate(core.GetInitialFlightData());
            deploymentChanceString = $"{deploymentChance:P}";
        }

        public override void OnUpdate()
        {
            if(!parachuteActive && HighLogic.CurrentGame.Parameters.CustomParams<LRTFGameSettings>().lrtfParachutes && (chute.deploymentState == ModuleParachute.deploymentStates.ACTIVE || chute.deploymentState == ModuleParachute.deploymentStates.DEPLOYED || chute.deploymentState == ModuleParachute.deploymentStates.SEMIDEPLOYED))
            {
                parachuteActive = true;
                if(deploymentChance < core.RandomGenerator.NextDouble())
                {
                    Failed = true;
                    TestFlightUtil.GetCore(this.part, Configuration).TriggerNamedFailure(this.moduleName);
                }
            }
        }

        public override void DoFailure()
        {
            chute.deploymentState = ModuleParachute.deploymentStates.STOWED;
            chute.enabled = false;
            core.ModifyFlightData(duFail, true);
            deploymentChanceString = failureTitle;
            base.DoFailure();
        }

        public override float DoRepair()
        {
            base.DoRepair();
            chute.enabled = true;
            parachuteActive = false;
            return 0f;
        }
    }
}