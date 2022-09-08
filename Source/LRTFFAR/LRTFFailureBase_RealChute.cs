using TestFlightAPI;
using UnityEngine;
using RealChute;

//[assembly: KSPAssemblyDependency("RealChute", 0, 0)]

namespace TestFlight.LRTF.LRTFRealChute
{

    public class LRTFFailureBase_RealChute : LRTFFailureBase
    {
        protected RealChuteModule chute;

        public override void OnStart(PartModule.StartState state)
        {
            base.OnStart(state);
            chute = part.FindModuleImplementing<RealChuteModule>();

            if (chute == null)
                Debug.Log("[LRTF] RealChute not found for " + part.name + "!  " + moduleName + " failures will be disabled for this part!");

        }
        public override void DoFailure()
        {
            if (chute == null)
                return;
            base.DoFailure();
        }
    }
}