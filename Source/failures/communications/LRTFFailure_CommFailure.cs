namespace TestFlight.LRTF
{
    public class LRTFFailure_CommFailure : LRTFFailureBase_Communications
    {
        
        public override void DoFailure()
        {
            transmitter.antennaPower = 0;

            base.DoFailure();
        }

        public override float DoRepair()
        {
            base.DoRepair();
            transmitter.antennaPower = originalPower;
            return 0f;
        }
    }
}