using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using TestFlightCore;
using TestFlightAPI;

namespace TestFlight
{
    public class LRTFFailure_RCSShutdown : LRTFFailureBase_RCS
    {
        private float previousThrustPower;

        private bool stateEnabled;
        private bool statercsEnabled;

        public override void DoFailure()
        {   
            stateEnabled = base.rcs.enabled;
            statercsEnabled = base.rcs.rcsEnabled;
            previousThrustPower = base.rcs.thrusterPower;

            base.rcs.thrusterPower = 0;
            base.rcs.enabled = false;
            base.rcs.rcsEnabled = false;
            base.rcs.DeactivateFX();

            base.rcs.Events["ToggleToggles"].active = false;
            base.rcs.Events["ToggleToggles"].guiActive = false;

            base.DoFailure();
        }

        public override float DoRepair()
        {
            base.DoRepair();
            base.rcs.thrusterPower = previousThrustPower;
            base.rcs.enabled = stateEnabled;
            base.rcs.rcsEnabled = statercsEnabled;

            base.rcs.Events["ToggleToggles"].active = !statercsEnabled;
            base.rcs.Events["ToggleToggles"].guiActive = !statercsEnabled;

            return 0f;
        }
    }
}