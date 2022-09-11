using System;
using RealAntennas;
using UnityEngine;

namespace TestFlight.LRTF
{
    public class LRTFFailure_RealAntennasPartial : LRTFFailureBase_RealAntennas
    {

        [KSPField(isPersistant = true)]
        float partialFailedValue = 0;

        public override void DoFailure()
        {
            if (hasStarted && partialFailedValue == 0)
                partialFailedValue = originalPower * (float) core.RandomGenerator.NextDouble();

            transmitter.TxPower = partialFailedValue;
            part.ModulesOnStart();

            base.DoFailure();
        }

        public override float DoRepair()
        {
            base.DoRepair();
            transmitter.TxPower = originalPower;
            part.ModulesOnStart();

            return 0f;
        }
    }
}