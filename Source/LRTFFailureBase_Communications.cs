using UnityEngine;

namespace TestFlight.LRTF
{

    public class LRTFFailureBase_Communications : LRTFFailureBase
    {
        public double originalPower;

        protected ModuleDataTransmitter transmitter;

        public override void OnStart(PartModule.StartState state)
        {
            base.OnStart(state);
            this.transmitter = base.part.FindModuleImplementing<ModuleDataTransmitter>();
            if(transmitter == null)
                Debug.Log("[LRTF] ModuleDataTransmitter not found for " + part.name + "!  This failures will be disabled for this part!");
            else
                originalPower = transmitter.antennaPower;
        }
        public override void DoFailure()
        {
            if (transmitter == null)
                return;
            base.DoFailure();
        }
    }
}