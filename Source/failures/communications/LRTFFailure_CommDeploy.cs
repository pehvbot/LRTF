using TestFlightAPI;

namespace TestFlight.LRTF
{
    public class LRTFFailure_CommDeploy : LRTFFailureBase_Communications
    {
        [KSPField]
        public FloatCurve deploymentChanceCurve;

        [KSPField(guiName = "Antenna Deployment Chance", groupName = "LRTF", groupDisplayName = "Less Real Test Flight", guiActive = true)]
        private string deploymentChanceString;

        private ModuleDeployableAntenna antenna;
        private double deploymentChance;

        [KSPField(isPersistant = true)]
        private bool antennaDeployed = false;


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

            this.antenna = base.part.FindModuleImplementing<ModuleDeployableAntenna>();
            
            deploymentChance = deploymentChanceCurve.Evaluate(core.GetInitialFlightData());
            deploymentChanceString = $"{deploymentChance:P}";
        }

        public override void OnUpdate()
        {
            if (!antennaDeployed && HighLogic.CurrentGame.Parameters.CustomParams<LRTFGameSettings>().lrtfCommunications
                && (antenna.deployState == ModuleDeployablePart.DeployState.EXTENDING))
            {
                antennaDeployed = true;
                if (deploymentChance < core.RandomGenerator.NextDouble())
                {
                    Failed = true;
                    
                    TestFlightUtil.GetCore(this.part, Configuration).TriggerNamedFailure(this.moduleName);
                }
            }
            if (antennaDeployed && antenna.deployState == ModuleDeployablePart.DeployState.RETRACTING)
                antennaDeployed = false;
        }

        public override void DoFailure()
        {
            transmitter.StopAllCoroutines();
            antenna.StopAllCoroutines();
            antenna.CheatRepair(); //not great but should never be callable if part is actually broken
            antenna.deployState = ModuleDeployablePart.DeployState.RETRACTED;
            antenna.Events["Extend"].guiActive = false;
            antenna.Events["Retract"].guiActive = false;
            transmitter.Events["StartTransmission"].guiActive = false;
            transmitter.Events["StopTransmission"].guiActive = false;

            core.ModifyFlightData(duFail, true);
            deploymentChanceString = failureTitle;
            base.DoFailure();
        }

        public override float DoRepair()
        {
            Failed = false;
            antennaDeployed = false;

            antenna.Events["Extend"].guiActive = true;
            antenna.Events["Retract"].guiActive = true;
            transmitter.Events["StartTransmission"].guiActive = true;
            transmitter.Events["StopTransmission"].guiActive = false;
            deploymentChanceString = $"{deploymentChance:P}";
            base.DoRepair();
            return 0f;
        }
    }
}