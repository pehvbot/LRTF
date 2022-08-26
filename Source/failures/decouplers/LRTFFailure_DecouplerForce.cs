namespace TestFlight.LRTF
{
    public class LRTFFailure_DecouplerForce : LRTFFailureBase_Decoupler
    {
        [KSPField(isPersistant = true)]
        private float originalForce;

        public override void OnLoad(ConfigNode node)
        {
            base.OnLoad(node);
        }

        public override void OnStart(PartModule.StartState state)
        {
            base.OnStart(state);
        }

        public override void DoFailure()
        {
            if(decouple != null)
            {
                originalForce = decouple.ejectionForce;
                decouple.ejectionForce = 0;
            }
            if(anchoredDecouple != null)
            {
                originalForce = anchoredDecouple.ejectionForce;
                anchoredDecouple.ejectionForce = 0;
            }
            base.DoFailure();
        }

        public override float DoRepair()
        {
            base.DoRepair();
            if (decouple != null)
                decouple.ejectionForce = originalForce;
            if (anchoredDecouple != null)
                anchoredDecouple.ejectionForce = originalForce;

            return 0f;
        }
    }
}