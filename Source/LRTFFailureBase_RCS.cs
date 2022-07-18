using TestFlightAPI;
using UnityEngine;

namespace TestFlight.LRTF
{

    public class LRTFFailureBase_RCS : LRTFFailureBase
    {
        protected ModuleRCS rcs;

        public override void OnStart(PartModule.StartState state)
        {
            base.OnStart(state);
            rcs = base.part.FindModuleImplementing<ModuleRCS>();
            if (rcs == null)
                Debug.Log("[LRTF] ModuleRCS not found for " + part.name + "!  This failures will be disabled for this part!");
        }
        public override void DoFailure()
        {
            if (rcs == null)
                return;
            base.DoFailure();
        }
    }
}