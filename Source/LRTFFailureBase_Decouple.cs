namespace TestFlight.LRTF
{

    public class LRTFFailureBase_Decoupler : LRTFFailureBase
    {
        protected LRTFModuleDecouple decouple;
        protected LRTFModuleAnchoredDecoupler anchoredDecouple;

        public override void OnStart(PartModule.StartState state)
        {
            base.OnStart(state);
            foreach (var d in base.part.FindModulesImplementing<LRTFModuleDecouple>())
                this.decouple = d;
            foreach (var d in base.part.FindModulesImplementing<LRTFModuleAnchoredDecoupler>())
                this.anchoredDecouple = d;
        }
    }
}