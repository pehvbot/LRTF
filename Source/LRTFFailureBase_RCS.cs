using TestFlightAPI;

namespace TestFlight
{

    public class LRTFFailureBase_RCS : LRTFFailureBase
    {
        protected ModuleRCS rcs;

        public override void OnStart(PartModule.StartState state)
        {
            base.OnStart(state);
            this.rcs = base.part.FindModuleImplementing<ModuleRCS>();
        }
    }
}