using UnityEngine;
namespace TestFlight.LRTF
{

    public class LRTFFailureBase_Aerodynamics : LRTFFailureBase
    {
        protected ModuleControlSurface controlSurface;

        public override void OnStart(StartState state)
        {
            base.OnStart(state);
            controlSurface = base.part.FindModuleImplementing<ModuleControlSurface>();
            if (controlSurface == null)
                Debug.Log("[LRTF] ModuleControlSurface not found for " + part.name + "!  This failures will be disabled for this part!");
        }
        public override void DoFailure()
        {
            if (controlSurface == null)
                return;
            base.DoFailure();
        }
    }
}