using TestFlightAPI;

namespace TestFlight
{

    public class LRTFFailureBase_Aerodynamics : LRTFFailureBase
    {
        protected ModuleControlSurface controlSurface;

        public override void OnStart(StartState state)
        {
            base.OnStart(state);
            this.controlSurface = base.part.FindModuleImplementing<ModuleControlSurface>();
        }
    }
}