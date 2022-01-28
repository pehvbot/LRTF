namespace TestFlight.LRTF
{
    public class LRTFFailure_ReactionBroken : LRTFFailureBase_ReactionWheel
    {
        private ModuleReactionWheel.WheelState state;

        public override void DoFailure()
        {
            this.state = base.module.wheelState;
            base.module.Events["OnToggle"].active = false;
            base.module.wheelState = ModuleReactionWheel.WheelState.Broken;
            base.DoFailure();
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