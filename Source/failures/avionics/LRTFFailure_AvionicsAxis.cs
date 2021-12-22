using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TestFlightAPI;

namespace TestFlight
{ 
    public class LRTFFailure_AvionicsAxis : LRTFFailureBase_Avionics
    {
        public override float Calculate(float value)
        {
            return 0;
        }
    }
}
