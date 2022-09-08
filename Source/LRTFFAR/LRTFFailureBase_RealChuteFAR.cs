using TestFlightAPI;
using UnityEngine;
using FerramAerospaceResearch.RealChuteLite;

namespace TestFlight.LRTF
{

    public class LRTFFailureBase_RealChuteFAR : LRTFFailureBase
    {
        protected RealChuteFAR chute;

        public override void OnStart(PartModule.StartState state)
        {
            base.OnStart(state);
            chute = part.FindModuleImplementing<RealChuteFAR>();

            if (chute == null )
                Debug.Log("[LRTF] RealChuteFAR not found for " + part.name + "!  " + moduleName + " failures will be disabled for this part!");

        }
        public override void DoFailure()
        {
            if (chute == null)
                return;
            base.DoFailure();
        }
    }
}