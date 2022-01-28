using System.Linq;
using System.Collections.Generic;

using UnityEngine;

namespace TestFlight.LRTF
{
    public class LRTFFailure_IgnitionFail : LRTFFailureBase_Engine
    {
        [KSPField]
        public FloatCurve baseIgnitionChance = null;
        [KSPField]
        public FloatCurve pressureCurve = null;
        [KSPField]
        public float additionalFailureChance = 0f;


        [KSPField(guiName = "Ignition Chance", groupName = "LRTF", groupDisplayName = "Less Real Test Flight", guiActive = true)]
        private string ignitionChanceString;
        [KSPField(guiName = "Ignition Penalty for Q", groupName = "LRTF", groupDisplayName = "Less Real Test Flight", guiActive = true)]
        private string dynamicPressurePenaltyString;

        private bool preLaunchFailures = false;
        private bool dynPressurePenalties = true;

        private List<EngineRunData> engineRunData;

        [KSPField(isPersistant = true)]

        private double initialFlightData;

        public override void OnAwake()
        {
            base.OnAwake();
            if (baseIgnitionChance == null)
            {
                baseIgnitionChance = new FloatCurve();
                baseIgnitionChance.Add(0f, 1f);
            }
            if (pressureCurve == null)
            {
                pressureCurve = new FloatCurve();
                pressureCurve.Add(0f, 1f);
            }
        }

        public override void OnLoad(ConfigNode node)
        {
            base.OnLoad(node);
            engineRunData = new List<EngineRunData>();
            foreach (var configNode in node.GetNodes("ENGINE_RUN_DATA"))
            {
                engineRunData.Add(new EngineRunData(configNode));
            }
        }

        public override void OnSave(ConfigNode node)
        {
            base.OnSave(node);
            foreach (var engineRunData in engineRunData)
            {
                var dataNode = node.AddNode("ENGINE_RUN_DATA");
                engineRunData.Save(dataNode);
            }
        }

        public override void OnStart(StartState state)
        {
            base.OnStart(state);
            core.DisableFailure(this.moduleName);
            preLaunchFailures = HighLogic.CurrentGame.Parameters.CustomParams<TestFlightGameSettings>().preLaunchFailures;
            dynPressurePenalties = HighLogic.CurrentGame.Parameters.CustomParams<TestFlightGameSettings>().dynPressurePenalties;
            initialFlightData = core.GetInitialFlightData();
            
        }

        public EngineRunData GetEngineRunDataForID(uint id)
        {
            return engineRunData.FirstOrDefault(data => data.id == id);
        }

        public override void OnUpdate()
        {
            if (Failed || !TestFlightEnabled || !HighLogic.CurrentGame.Parameters.CustomParams<LRTFGameSettings>().lrtfEngines)
                return;

            // For each engine we are tracking, compare its current ignition state to our last known ignition state
            foreach(var engine in engines)
            {
                EngineModuleWrapper.EngineIgnitionState currentIgnitionState = engine.engine.IgnitionState;
                var engineData = GetEngineRunDataForID(engine.engine.Module.PersistentId);
                if (engineData == null)
                {
                    engineData = new EngineRunData(engine.engine.Module.PersistentId);
                    engineRunData.Add(engineData);
                }
                float ignitionChance = 1f;
                float pressureModifier = 1f;

                if (this.vessel.situation != Vessel.Situations.PRELAUNCH || preLaunchFailures)
                {
                    ignitionChance = baseIgnitionChance.Evaluate((float)initialFlightData);
                    if (ignitionChance <= 0)
                        ignitionChance = 1f;
                }

                if (dynPressurePenalties)
                {
                    pressureModifier = Mathf.Clamp(pressureCurve.Evaluate((float)(part.dynamicPressurekPa * 1000d)), 0, 1);
                    Fields["dynamicPressurePenaltyString"].guiActive = true;
                    if (pressureModifier <= 0f)
                        pressureModifier = 1f;
                }
                else
                {
                    Fields["dynamicPressurePenaltyString"].guiActive = false;
                }
                if (this.vessel.situation != Vessel.Situations.PRELAUNCH)
                    ignitionChance = ignitionChance * pressureModifier;

                // If we are transitioning from not ignited to ignited, we do our check
                // The ignitionFailureRate defines the failure rate per flight data
                if (currentIgnitionState == EngineModuleWrapper.EngineIgnitionState.IGNITED)
                {
                    if (engine.ignitionState == EngineModuleWrapper.EngineIgnitionState.NOT_IGNITED || engine.ignitionState == EngineModuleWrapper.EngineIgnitionState.UNKNOWN)
                    {
                        double failureRoll = core.RandomGenerator.NextDouble();

                        if (failureRoll > ignitionChance)
                        {
                            engine.failEngine = true;
                            core.TriggerNamedFailure(this.moduleName);
                            failureRoll = core.RandomGenerator.NextDouble();
                            if (failureRoll < additionalFailureChance)
                            {
                                core.TriggerNamedFailure(this.moduleName);
                            }
                        }
                        else
                        {
                            engineData.hasBeenRun = true;
                        }
                    }
                }
                engine.ignitionState = currentIgnitionState;
              
                ignitionChanceString = $"{ignitionChance:P}";
                if (dynPressurePenalties)
                    dynamicPressurePenaltyString = $"{1 - pressureModifier:P}";
            }
        }

        public override void SetActiveConfig(string alias)
        {
            base.SetActiveConfig(alias);

            if (currentConfig == null) return;

            // update current values with those from the current config node
            currentConfig.TryGetValue("additionalFailureChance", ref additionalFailureChance);
            baseIgnitionChance = new FloatCurve();
            if (currentConfig.HasNode("baseIgnitionChance"))
            {
                baseIgnitionChance.Load(currentConfig.GetNode("baseIgnitionChance"));
            }
            else
            {
                baseIgnitionChance.Add(0f, 1f);
            }
            pressureCurve = new FloatCurve();
            if (currentConfig.HasNode("pressureCurve"))
            {
                pressureCurve.Load(currentConfig.GetNode("pressureCurve"));
            }
            else
            {
                pressureCurve.Add(0f, 1f);
            }
        }

        public override void DoFailure()
        {            
            foreach( var engine in engines)
            {
                if (engine.failEngine)
                {
                    //logic to shut down solids
                    bool allowShutdown = engine.engine.allowShutdown;
                    engine.engine.allowShutdown = true;
                    engine.engine.Shutdown();
                    engine.engine.allowShutdown = allowShutdown;

                    engine.engine.Events["Activate"].active = false;
                    engine.engine.Events["Shutdown"].active = false;
                    engine.engine.Events["Activate"].guiActive = false;
                    engine.engine.Events["Shutdown"].guiActive = false;
                    engine.failEngine = false;
                }
            }
            ignitionChanceString = $"<color=orange>{failureTitle}</color>";
            dynamicPressurePenaltyString = "";
            core.ModifyFlightData(duFail, true);
            base.DoFailure();
        }

        public override float DoRepair()
        {
            base.DoRepair();
            foreach(var engine in engines)
            {
                // Prevent auto-ignition on repair
                engine.engine.Shutdown();
                engine.engine.Events["Activate"].active = true;
                engine.engine.Events["Activate"].guiActive = true;
                engine.engine.Events["Shutdown"].guiActive = true;
                engine.failEngine = false;
            }

            return 0;
        }
    }
}