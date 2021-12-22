using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TestFlightAPI;
using UnityEngine;

namespace TestFlight
{
    public class LRTFFailure_WheelMotor : LRTFFailureBase_Wheel
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
            this.state = base.wheelMotor.motorEnabled;
            wheelMotor.motorEnabled = false;
            wheelMotor.Actions["MotorEnable"].active = false;
            wheelMotor.Fields["motorEnabled"].guiActive = false;
             
            pawMessage = failureTitle;
            Fields["pawMessage"].guiActive = true;
        }
        public override float DoRepair()
        {
            base.DoRepair();
            base.wheelMotor.motorEnabled = state;
            wheelMotor.Actions["MotorEnable"].active = true;
            wheelMotor.Fields["motorEnabled"].guiActive = true;
            return 0f;
        }
    }
}
