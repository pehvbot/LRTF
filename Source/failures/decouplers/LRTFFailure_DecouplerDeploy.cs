using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using TestFlightCore;
using TestFlightAPI;

namespace TestFlight
{
    public class LRTFFailure_DecouplerDeploy : LRTFFailureBase_Decoupler
    {
        public override void DoFailure()
        {
            if(decouple != null)
                decouple.Decouple();
            if (anchoredDecouple != null)
                anchoredDecouple.Decouple();
            base.DoFailure();
        }
    }
}