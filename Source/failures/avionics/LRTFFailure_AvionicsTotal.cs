using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TestFlight
{
    public class LRTFFailure_LRAvionicsTotal : LRTFFailureBase_Avionics
    {
        public override void OnFlyByWire(FlightCtrlState s)
        {
            if (!(base.vessel == null || base.vessel != FlightGlobals.ActiveVessel || base.part.isControlSource != Vessel.ControlLevel.FULL))
            {
                s.pitch = 0;
                s.roll = 0;
                s.yaw = 0;
                s.X = 0;
                s.Y = 0;
                s.Z = 0;
            }
        }
    }
}
