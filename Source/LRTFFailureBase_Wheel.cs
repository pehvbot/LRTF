using ModuleWheels;
using UnityEngine;
namespace TestFlight.LRTF
{
    public class LRTFFailureBase_Wheel : LRTFFailureBase
    {
        protected ModuleWheelBase wheelBase;
        protected ModuleWheelSteering wheelSteering;
        protected ModuleWheelBrakes wheelBrakes;
        protected ModuleWheelMotor wheelMotor;

        public override void OnStart(StartState state)
        {
            base.OnStart(state);
            this.wheelBase = base.part.FindModuleImplementing<ModuleWheelBase>();
            this.wheelSteering = base.part.FindModuleImplementing<ModuleWheelSteering>();
            this.wheelBrakes = base.part.FindModuleImplementing<ModuleWheelBrakes>();
            this.wheelMotor = base.part.FindModuleImplementing<ModuleWheelMotor>();
            if (wheelBase == null && wheelSteering == null && wheelBrakes == null && wheelMotor == null)
                Debug.Log("[LRTF] No ModuleWheel modules not found for " + part.name + "!  Recording will be disabled for this part!");
        }
        public override void DoFailure()
        {
            if (wheelBase == null && wheelSteering == null && wheelBrakes == null && wheelMotor == null)
                return;
            base.DoFailure();
        }
    }
}
