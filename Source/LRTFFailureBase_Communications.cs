using TestFlightAPI;

namespace TestFlight
{

    public class LRTFFailureBase_Communications : LRTFFailureBase
    {
        public double originalPower;

        protected ModuleDataTransmitter transmitter;

        public override void OnStart(PartModule.StartState state)
        {
            base.OnStart(state);
            this.transmitter = base.part.FindModuleImplementing<ModuleDataTransmitter>();
            originalPower = transmitter.antennaPower;
        }
    }
}