using TestFlightAPI;
using RealChute;

namespace TestFlight.LRTF.LRTFRealChute
{
    public class LRTFFailure_RealChuteDeploy : LRTFFailureBase_RealChute
    {
        [KSPField]
        public FloatCurve deploymentChanceCurve;

        [KSPField(guiName = "Parachute Deployment Chance", groupName = "LRTF", groupDisplayName = "Less Real Test Flight", guiActive = true)]
        private string deploymentChanceString;

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
            if (!parachuteActive && HighLogic.CurrentGame.Parameters.CustomParams<LRTFGameSettings>().lrtfParachutes && (chute.armed || chute.AnyDeployed))
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
            chute.armed = false;
            chute.DeactivateRC();
            foreach (Parachute p in chute.parachutes)
            {
                p.Cut();
                p.Repack(); 

            }
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
            deploymentChanceString = $"{deploymentChance:P}";
            return 0f;
        }
    }
}