using TestFlightAPI;
using UnityEngine;

namespace TestFlight.LRTF
{
    public class LRTFFailure_DecouplerFailure : LRTFFailureBase_Decoupler
    {
        [KSPField]
        public FloatCurve deploymentChanceCurve;

        [KSPField(guiName = "Decoupler Deployment Chance", groupName = "LRTF", groupDisplayName = "Less Real Test Flight", guiActive = true)]
        private string deploymentChanceString;

        private double deploymentChance;

        private bool attemptDecouple = true;

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

            deploymentChance = deploymentChanceCurve.Evaluate(core.GetInitialFlightData());
            deploymentChanceString = $"{deploymentChance:P}";
        }

        public override void SetActiveConfig(string alias)
        {
            base.SetActiveConfig(alias);

            if (currentConfig == null) return;

            // update current values with those from the current config node
            deploymentChanceCurve = new FloatCurve();
            if (currentConfig.HasNode("deploymentChanceCurve"))
            {
                deploymentChanceCurve.Load(currentConfig.GetNode("deploymentChanceCurve"));
            }
            else
            {
                deploymentChanceCurve.Add(0f, 1f);
            }
        }

        public override void OnUpdate()
        {
            bool isDecoupling = false;

            if (decouple != null)
                isDecoupling = decouple.isDecoupling;
            if (anchoredDecouple != null)
                isDecoupling = anchoredDecouple.isDecoupling;
          
            if (attemptDecouple && HighLogic.CurrentGame.Parameters.CustomParams<LRTFGameSettings>().lrtfDecouplers && isDecoupling)
            {
                attemptDecouple = false;
                if (deploymentChance < core.RandomGenerator.NextDouble())
                {
                    if (decouple != null)
                    {
                        decouple.canDecouple = false;
                        core.TriggerNamedFailure(this.moduleName);
                    }
                    if (anchoredDecouple != null)
                    {
                        anchoredDecouple.canDecouple = false;
                        core.TriggerNamedFailure(this.moduleName);
                    }
                    Failed = true;
                }
            }
        }

        public override void DoFailure()
        {
            core.ModifyFlightData(duFail, true);
            deploymentChanceString = failureTitle;
            base.DoFailure();
        }

        public override float DoRepair()
        {
            base.DoRepair();
            if (decouple != null)
                decouple.canDecouple = true;
            if (anchoredDecouple != null)
                anchoredDecouple.canDecouple = true;
            attemptDecouple = true;

            deploymentChanceString = $"{deploymentChance:P}";
            return 0f;
        }
    }
}