namespace TestFlight.LRTF
{
    public class LRTFFailure_AvionicsInvert : LRTFFailureBase_Avionics
    {
        public override float Calculate(float value)
        {
            return -value;
        }
    }
}