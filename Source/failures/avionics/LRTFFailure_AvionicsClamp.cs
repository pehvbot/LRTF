using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TestFlightAPI;
using UnityEngine;

namespace TestFlight
{
    public class LRTFFailure_AvionicsClamp : LRTFFailureBase_Avionics
    {
        public override float Calculate(float value)
        {
            return Mathf.Clamp(value, -this.failedValue, this.failedValue);
        }
    }
}