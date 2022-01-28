namespace TestFlight.LRTF
{
    public class LRTFFailure_DecouplerDeploy : LRTFFailureBase_Decoupler
    {
        public override void DoFailure()
        {
            if(decouple != null)
                decouple.Decouple();
            if (anchoredDecouple != null)
                anchoredDecouple.Decouple();
            base.DoFailure();
        }
    }
}