using UnityEngine;

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
            if(decouple == null && anchoredDecouple == null)
                Debug.Log("[LRTF] LRTFModuleDecouple/LRTFModuleAnchoredDecoupler not found for " + part.name + "!  This failures will be disabled for this part!");

        }
        public override void DoFailure()
        {
            if (decouple == null && anchoredDecouple == null)
                return;
            base.DoFailure();
        }
    }
}