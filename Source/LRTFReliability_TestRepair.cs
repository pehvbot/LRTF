using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

using TestFlightAPI;

namespace TestFlight
{
    public class LRTFReliability_TestRepair : TestFlightReliabilityBase
    {
        [KSPEvent(guiName = "Force Repair", active = true, guiActive = true,groupName = "TestFlight")]
        public void ForceRepair()
        {
            string message = "Part has been repaired after forced repair";
            message = String.Format("TestFlightReliability({0}[{1}]): {2}", Configuration, Configuration, message);
            TestFlightUtil.Log(message, this.part);

            foreach(ITestFlightFailure f in core.GetActiveFailures())
            {
                core.ForceRepair(f);
            }
        }
        public override void OnUpdate()
        {
            base.OnUpdate();
        }
    }
}
