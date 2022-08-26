namespace TestFlight.LRTF
{
    public class LRTFFailure_WheelBrake : LRTFFailureBase_Wheel
    {
        [KSPField(isPersistant = true)]
        private float breakTweakable;

        public override void DoFailure()
        {
            this.breakTweakable = wheelBrakes.brakeTweakable;
            wheelBrakes.Actions["BrakeAction"].active = false;
            wheelBrakes.Fields["brakeTweakable"].guiActive = false;
            wheelBrakes.brakeTweakable = 0;
      
            base.DoFailure();
        }
        public override float DoRepair()
        {
            base.DoRepair();
            wheelBrakes.Actions["BrakeAction"].active = true;
            wheelBrakes.Fields["brakeTweakable"].guiActive = true;
            wheelBrakes.brakeTweakable = this.breakTweakable;
            return 0f;

        }
    }
}
