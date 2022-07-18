using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TestFlightAPI;
using ModuleWheels;
using UnityEngine;

namespace TestFlight.LRTF
{
    public class LRTFDataRecorder_Wheels : LRTFDataRecorderBase
    {
        private ModuleWheelSteering wheelSteering;
        private ModuleWheelBrakes wheelBrakes;
        private ModuleWheelBase wheel;
        private ModuleWheelMotor wheelMotor;
        public override void OnStart(PartModule.StartState state)
        {
            base.OnStart(state);
            wheel = base.part.FindModuleImplementing<ModuleWheelBase>();
            wheelSteering = base.part.FindModuleImplementing<ModuleWheelSteering>();
            wheelBrakes = base.part.FindModuleImplementing<ModuleWheelBrakes>();
            wheelMotor = base.part.FindModuleImplementing<ModuleWheelMotor>();
            if (wheel == null && wheelSteering == null && wheelBrakes == null && wheelMotor == null)
            {
                isEnabled = false;
                Debug.Log("[LRTF] No ModuleWheel modules not found for " + part.name + "!  Recording will be disabled for this part!");
            }
        }
        public override void OnAwake()
        {
            base.OnAwake();
        }
        public override bool IsPartOperating()
        {
            if (!wheel.isGrounded)
                return false;

            if ((float)base.vessel.horizontalSrfSpeed > 0f)
            {
                if (this.wheelSteering != null && this.wheelSteering.steeringEnabled && Math.Abs(this.wheelSteering.steeringInput) > 0f)
                    return true;
                if (this.wheelBrakes != null && this.wheelBrakes.enabled && this.wheelBrakes.brakeInput > 0f)
                    return true;
                if (this.wheelMotor != null && this.wheelMotor.motorEnabled && Math.Abs(wheelMotor.driveOutput) > 0f)
                    return true;
            }
            return false;
        }
    }
}
