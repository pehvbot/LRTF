using TestFlightAPI;
using UnityEngine;

namespace TestFlight.LRTF
{

    public class LRTFFailureBase_Parachute : LRTFFailureBase
    {
        protected ModuleParachute chute;

        public override void OnStart(PartModule.StartState state)
        {
            base.OnStart(state);
            chute = base.part.FindModuleImplementing<ModuleParachute>();
            if (chute == null)
                Debug.Log("[LRTF] ModuleParachute not found for " + part.name + "!  This failures will be disabled for this part!");

        }
        public override void DoFailure()
        {
            if (chute == null)
                return;
            base.DoFailure();
        }
    }
}