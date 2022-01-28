using ModuleWheels;

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
        }
    }
}
