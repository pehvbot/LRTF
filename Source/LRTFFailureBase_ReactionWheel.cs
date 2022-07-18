using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TestFlightAPI;
using UnityEngine;

namespace TestFlight.LRTF
{
    public class LRTFFailureBase_ReactionWheel : LRTFFailureBase
    {
        protected ModuleReactionWheel module;

        public override void OnStart(StartState state)
        {
            base.OnStart(state);
            module = base.part.FindModuleImplementing<ModuleReactionWheel>();
            if (module == null)
                Debug.Log("[LRTF] ModuleReactionWheel not found for " + part.partName + "!  This failures will be disabled for this part!");
        }
        public override void DoFailure()
        {
            if (module == null)
                return;
            base.DoFailure();
        }
    }
}