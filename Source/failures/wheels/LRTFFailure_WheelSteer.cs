using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TestFlightAPI;
using UnityEngine;

namespace TestFlight
{
    public class LRTFFailure_WheelSteer : LRTFFailureBase_Wheel
    {
        private bool state;

        public override void OnStartFinished(StartState state)
        {
            base.OnStartFinished(state);
            if (failed)
            {
                TestFlightUtil.GetCore(this.part, Configuration).TriggerNamedFailure(this.moduleName);
            }
        }

        public override void DoFailure()
        {
            base.DoFailure();
            this.state = base.wheelSteering.steeringEnabled;
            base.wheelSteering.steeringEnabled = false;
            base.wheelSteering.Actions["SteeringToggle"].active = false;
            base.wheelSteering.Fields["steeringEnabled"].guiActive = false;
            pawMessage = failureTitle;
            Fields["pawMessage"].guiActive = true;
        }
        public override float DoRepair()
        {
            base.DoRepair();
            base.wheelSteering.steeringEnabled = this.state;
            base.wheelSteering.Events["LockSteering"].active = !this.state;
            base.wheelSteering.Events["UnlockSteering"].active = this.state;
            return 0f;
        }
    }
}