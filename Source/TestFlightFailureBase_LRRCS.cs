using TestFlightAPI;

namespace TestFlight
{

    public class TestFlightFailureBase_LRRCS : TestFlightFailureBase
    {
        protected ModuleRCS rcsfx;

        public override void OnStart(PartModule.StartState state)
        {
            base.OnStart(state);
            this.rcsfx = base.part.FindModuleImplementing<ModuleRCS>();
        }

    }
    
}