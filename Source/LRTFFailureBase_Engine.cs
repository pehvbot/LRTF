﻿using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace TestFlight.LRTF
{
    public class LRTFFailureBase_Engine : LRTFFailureBase
    {
        [KSPField]
        public string engineID = "ALL";

        protected class EngineHandler
        {
            public EngineModuleWrapper.EngineIgnitionState ignitionState;
            public EngineModuleWrapper engine;
            public bool failEngine;
        }

        protected List<EngineHandler> engines = null;

        public override void OnStart(StartState state)
        {
            base.OnStart(state);
            StartCoroutine("Attach");
        }

        IEnumerator Attach()
        {
            while (this.part == null || this.part.partInfo == null || this.part.partInfo.partPrefab == null || this.part.Modules == null)
                yield return null;

            Startup();
        }

        public virtual void Startup()
        {
            engines = new List<EngineHandler>();
            if (!String.IsNullOrEmpty(engineID))
            {
                if (engineID.ToUpper() == "ALL")
                {
                    List<ModuleEngines> engineMods = this.part.Modules.GetModules<ModuleEngines>();
                    foreach (ModuleEngines eng in engineMods)
                    {
                        string id = eng.engineID;
                        EngineModuleWrapper engine = new EngineModuleWrapper();
                        engine.InitWithEngine(this.part, id);
                        EngineHandler engineHandler = new EngineHandler();
                        engineHandler.engine = engine;
                        engineHandler.ignitionState = engine.IgnitionState;
                        engines.Add(engineHandler);
                    }
                }
                else if (engineID.Contains(","))
                {
                    string[] sEngineIndices = engineID.Split(new char[1] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (string sEngineIndex in sEngineIndices)
                    {
                        EngineModuleWrapper engine = new EngineModuleWrapper();
                        engine.InitWithEngine(this.part, sEngineIndex);
                        EngineHandler engineHandler = new EngineHandler();
                        engineHandler.engine = engine;
                        engineHandler.ignitionState = engine.IgnitionState;
                        engines.Add(engineHandler);
                    }
                }
                else
                {
                    EngineModuleWrapper engine = new EngineModuleWrapper();
                    engine.InitWithEngine(this.part, engineID);
                    EngineHandler engineHandler = new EngineHandler();
                    engineHandler.engine = engine;
                    engineHandler.ignitionState = engine.IgnitionState;
                    engines.Add(engineHandler);
                }
            }
            else
            {
                EngineModuleWrapper engine = new EngineModuleWrapper();
                engine.Init(this.part);
                EngineHandler engineHandler = new EngineHandler();
                engineHandler.engine = engine;
                engineHandler.ignitionState = engine.IgnitionState;
                engines.Add(engineHandler);
            }
        }

        private void OnEnable()
        {
            StartCoroutine(UpdateEngineStatus());
        }

        private void OnDisable()
        {
            StopCoroutine(UpdateEngineStatus());
        }

        public override void SetActiveConfig(string alias)
        {
            base.SetActiveConfig(alias);

            if (currentConfig == null) return;

            // update current values with those from the current config node
            currentConfig.TryGetValue("engineID", ref engineID);
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

        public IEnumerator UpdateEngineStatus()
        {
            while (true)
            {
                yield return new WaitForFixedUpdate();

                if (Failed && engines != null)
                {
                    foreach (var handler in engines)
                    {
                        handler.engine.Status = "Failed";
                        handler.engine.StatusL2 = failureTitle;
                    }
                }
            }
        }
    }
}