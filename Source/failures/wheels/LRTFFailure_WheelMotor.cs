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

        public override void DoFailure()
        {
            this.state = base.wheelMotor.motorEnabled;
            wheelMotor.motorEnabled = false;
            wheelMotor.Actions["MotorEnable"].active = false;
            wheelMotor.Fields["motorEnabled"].guiActive = false;
             
            base.DoFailure();
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
