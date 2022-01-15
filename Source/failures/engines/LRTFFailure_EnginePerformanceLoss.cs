using System;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using TestFlightAPI;

namespace TestFlight
{
    public class LRTFFailure_EnginePerformanceLoss : LRTFFailureBase_Engine
    {
        [KSPField]
        public float ispMultiplier = 0.7f;
        [KSPField]
        public float ispMultiplierJitter = 0.1f;

        private List<float> multipliers = new List<float>();

        public override void OnSave(ConfigNode node)
        {
            if (multipliers.Count > 0)
            {
                node.RemoveNode("MULIPLIERS");
                ConfigNode saveLocks = new ConfigNode();
                foreach (float m in multipliers)
                {
                    saveLocks.AddValue("multiplier", m);
                }
                node.AddNode("MULIPLIERS", saveLocks);
            }
            base.OnSave(node);
        }

        public override void OnLoad(ConfigNode node)
        {
            if (node.HasNode("MULIPLIERS"))
            {
                foreach (string g in node.GetNode("MULIPLIERS").GetValues("multiplier"))
                {
                    multipliers.Add(float.Parse(g));
                }
            }
            base.OnLoad(node);
        }

        /// <summary>
        /// Triggers the failure controlled by the failure module
        /// </summary>
        public override void DoFailure()
        {
            // for each engine change its specific impulse
            if (hasStarted)
            {
                foreach (EngineHandler engine in engines)
                {
                    float jitter = ispMultiplierJitter - ((float)TestFlightUtil.GetCore(this.part, Configuration).RandomGenerator.NextDouble() * (ispMultiplierJitter * 2));
                    float actualMultiplier = ispMultiplier + jitter;
                    multipliers.Add(actualMultiplier);
                }
            }
            int count = 0;
            foreach (EngineHandler engine in engines)
            { 
                engine.engine.SetFuelIspMult(multipliers[count++]);
                engine.engine.failMessage = failureTitle;
                engine.engine.failed = true;
            }
            base.DoFailure();
        }

        public override float DoRepair()
        {
            base.DoRepair();
            // for each engine restore its specific impulse back to 1.0
            foreach (EngineHandler engine in engines)
            {
                engine.engine.SetFuelIspMult(1.0f);
                engine.engine.failMessage = "";
                engine.engine.failed = false;
            }
            return 0;
        }
        public override void SetActiveConfig(string alias)
        {
            base.SetActiveConfig(alias);

            if (currentConfig == null) return;

            // update current values with those from the current config node
            currentConfig.TryGetValue("ispMultiplier", ref ispMultiplier);
            currentConfig.TryGetValue("ispMultiplierJitter", ref ispMultiplierJitter);
        }

        public override void OnAwake()
        {
            base.OnAwake();
            if (!string.IsNullOrEmpty(configNodeData))
            {
                var node = ConfigNode.Parse(configNodeData);
                OnLoad(node);
            }
        }
    }
}

