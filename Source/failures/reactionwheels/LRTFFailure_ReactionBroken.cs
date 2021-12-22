using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using TestFlightAPI;

namespace TestFlight
{
    public class LRTFFailure_ReactionBroken : LRTFFailureBase_ReactionWheel
    {
        private ModuleReactionWheel.WheelState state;

        public override void OnStartFinished(StartState state)
        {
            base.OnStartFinished(state);
            if (failed)
                TestFlightUtil.GetCore(this.part, Configuration).TriggerNamedFailure(this.moduleName);
        }

        public override void DoFailure()
        {
            base.DoFailure();
            this.state = base.module.wheelState;
            base.module.Events["OnToggle"].active = false;
            base.module.wheelState = ModuleReactionWheel.WheelState.Broken;
            pawMessage = failureTitle;
            Fields["pawMessage"].guiActive = true;
        }
        public override float DoRepair()
        {
            base.DoRepair();
            base.module.Events["OnToggle"].active = true;
            base.module.wheelState = this.state;
            return 0f;
        }
    }
}