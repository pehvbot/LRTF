using TestFlightAPI;
using UnityEngine;

namespace TestFlight.LRTF
{

    public class LRTFFailureBase_Parachute : LRTFFailureBase
    {
        protected ModuleParachute chute;
        protected PartModule far;

        public override void OnStart(PartModule.StartState state)
        {
            base.OnStart(state);
            foreach (var pm in part.Modules)
            {
                if (pm.moduleName == "RealChuteFAR")
                    far = pm;
            }
            
            chute = base.part.FindModuleImplementing<ModuleParachute>();
            if (chute == null && far == null)
                Debug.Log("[LRTF] Parachute module not found for " + part.name + "!  This failures will be disabled for this part!");

        }
        public override void DoFailure()
        {
            if (chute == null)
                return;
            base.DoFailure();
        }
    }
}