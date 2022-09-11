using UnityEngine;
using RealAntennas;

namespace TestFlight.LRTF
{

    public class LRTFFailureBase_RealAntennas : LRTFFailureBase
    {

        [KSPField(isPersistant = true)]
        public float originalPower = 0;

        protected ModuleRealAntenna transmitter;

        public override void OnStart(PartModule.StartState state)
        {
            base.OnStart(state);
            this.transmitter = base.part.FindModuleImplementing<ModuleRealAntenna>();
            if(transmitter == null)
                Debug.Log("[LRTF] ModuleDataTransmitter not found for " + part.name + "!  This failures will be disabled for this part!");
            else
                if (!Failed && originalPower == 0)
                    originalPower = transmitter.TxPower;

        }
        public override void DoFailure()
        {
            if (transmitter == null)
                return;
            base.DoFailure();
        }
    }
}