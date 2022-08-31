using TestFlightAPI;
using UnityEngine;

namespace TestFlight.LRTF
{

    public class LRTFFailureBase_RealChuteFAR : LRTFFailureBase
    {
        protected PartModule chute;

        public override void OnStart(PartModule.StartState state)
        {
            base.OnStart(state);
            foreach (var p in part.Modules)
            {
                if (p.moduleName == "RealChuteFAR")
                    chute = p;
            }
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