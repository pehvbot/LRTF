using RealAntennas;

namespace TestFlight.LRTF
{
    public class LRTFFailure_RealAntennasFailure : LRTFFailureBase_RealAntennas
    {
        [KSPField(isPersistant = true)]
        float previousPower;

        public override void DoFailure()
        {
            if(hasStarted)
                previousPower = transmitter.TxPower;
            transmitter.TxPower = 0;
            part.ModulesOnStart();

            base.DoFailure();

            transmitter.isEnabled = false;
            part.PartActionWindow.displayDirty = true;

        }

        public override float DoRepair()
        {
            base.DoRepair();
            transmitter.isEnabled = true;
            transmitter.TxPower = previousPower;

            part.ModulesOnStart();

            return 0f;
        }
    }
}