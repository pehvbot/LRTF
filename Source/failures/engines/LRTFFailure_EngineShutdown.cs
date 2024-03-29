﻿using System.Collections.Generic;
using UnityEngine;

namespace TestFlight.LRTF
{
    public class LRTFFailure_EngineShutdown : LRTFFailureBase_Engine
    {
        protected struct CachedEngineState
        {
            public bool allowShutdown;
            public int numIgnitions;
        }

        Dictionary<int, CachedEngineState> engineStates;

        public override void OnStart(StartState state)
        {
            base.OnStart(state);
            base.Startup();
        }
        /// <summary>
        /// Triggers the failure controlled by the failure module
        /// </summary>
        public override void DoFailure()
        {
            if (engineStates == null)
                engineStates = new Dictionary<int, CachedEngineState>();
            engineStates.Clear();
            foreach (EngineHandler engine in engines)
            {
                int id = engine.engine.Module.GetInstanceID();
                CachedEngineState engineState = new CachedEngineState();
                engineState.allowShutdown = engine.engine.allowShutdown;
                engineState.numIgnitions = engine.engine.GetIgnitionCount();
                engine.engine.Shutdown();
                var numIgnitionsToRemove = 1;
                if (severity.ToLowerInvariant() == "major")
                {
                    numIgnitionsToRemove = -1;

                    // For some reason, need to disable GUI as well
                    engine.engine.Events["Activate"].active = false;
                    engine.engine.Events["Shutdown"].active = false;
                    engine.engine.Events["Activate"].guiActive = false;
                    engine.engine.Events["Shutdown"].guiActive = false;

                    engine.engine.failed = true;
                    engine.engine.failMessage = failureTitle;
                }
                engineStates.Add(id, engineState);
                engine.engine.RemoveIgnitions(numIgnitionsToRemove);
            }
            base.DoFailure();
        }

        public override float DoRepair()
        {
            base.DoRepair();
            // for each engine restore it
            foreach (EngineHandler engine in engines)
            {
                int id = engine.engine.Module.GetInstanceID();

                //happens when repaired in VAB
                if(engineStates == null)
                {
                    engine.engine.enabled = true;
                    engine.engine.Events["Activate"].active = true;
                    engine.engine.Events["Activate"].guiActive = true;
                    engine.engine.Events["Shutdown"].guiActive = true;
                    engine.engine.failed = false;
                    engine.engine.failMessage = "";
                }
                else if (engineStates.ContainsKey(id))
                {
                    engine.engine.enabled = true;
                    engine.engine.Events["Activate"].active = true;
                    engine.engine.Events["Activate"].guiActive = true;
                    engine.engine.Events["Shutdown"].guiActive = true;
                    engine.engine.allowShutdown = engineStates[id].allowShutdown;
                    engine.engine.SetIgnitionCount(engineStates[id].numIgnitions);
                    engine.engine.failed = false;
                    engine.engine.failMessage = "";
                    engineStates.Clear();
                }
            }
            return 0;
        }
    }
}

