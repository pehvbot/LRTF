using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using TestFlightCore;
using TestFlightAPI;

namespace TestFlight
{
    public class LRTFFailure_CommPartial : LRTFFailureBase_Communications
    {
        [KSPField(isPersistant = true)]
        double failedValue = 0;

        public override void OnLoad(ConfigNode node)
        {
            node.TryGetValue("failedValue", ref failedValue);
            base.OnLoad(node);
        }

        public override void DoFailure()
        {
            if (hasStarted)
                failedValue = Math.Pow(core.RandomGenerator.NextDouble(), core.RandomGenerator.Next(2, 10));
            transmitter.antennaPower = failedValue * originalPower;

            base.DoFailure();
        }

        public override float DoRepair()
        {
            base.DoRepair();
            transmitter.antennaPower = originalPower;
            return 0f;
        }
    }
}