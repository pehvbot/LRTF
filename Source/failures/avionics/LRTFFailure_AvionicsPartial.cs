namespace TestFlight.LRTF
{
    public class LRTFFailure_AvionicsPartial : LRTFFailureBase_Avionics
    {
        public override float Calculate(float value)
        {
            return value * base.failedValue;
        }
    }
}