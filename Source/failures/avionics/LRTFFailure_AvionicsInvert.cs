﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TestFlight
{
    public class LRTFFailure_AvionicsInvert : LRTFFailureBase_Avionics
    {
        public override float Calculate(float value)
        {
            return -value;
        }
    }
}