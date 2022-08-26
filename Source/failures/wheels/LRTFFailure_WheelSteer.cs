namespace TestFlight.LRTF
{
    public class LRTFFailure_WheelSteer : LRTFFailureBase_Wheel
    {
        [KSPField(isPersistant = true)]
        private bool state;

        public override void DoFailure()
        {
            base.DoFailure();
            this.state = base.wheelSteering.steeringEnabled;
            base.wheelSteering.steeringEnabled = false;
            base.wheelSteering.Actions["SteeringToggle"].active = false;
            base.wheelSteering.Fields["steeringEnabled"].guiActive = false;
            base.DoFailure();
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