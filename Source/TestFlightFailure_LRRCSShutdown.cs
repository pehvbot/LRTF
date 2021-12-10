using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace TestFlight.Failure_Modules
{
    public class TestFlightFailure_LRRCSShutdown : TestFlightFailureBase_LRRCS
    {
        private bool stateEnabled;
        private bool statercsEnabled;

        public override void DoFailure()
        {
            base.DoFailure();
            this.stateEnabled = base.rcsfx.enabled;
            this.statercsEnabled = base.rcsfx.rcsEnabled;
           
            base.rcsfx.enabled = false;
            base.rcsfx.rcsEnabled = false;
            base.rcsfx.DeactivateFX();

            base.rcsfx.Events["ToggleToggles"].active = false;
            base.rcsfx.Events["ToggleToggles"].guiActive = false;
        }
        public override float DoRepair()
        {
            base.DoRepair();
            base.rcsfx.enabled = this.stateEnabled;
            base.rcsfx.rcsEnabled = this.statercsEnabled;

            base.rcsfx.Events["ToggleToggles"].active = !this.statercsEnabled;
            base.rcsfx.Events["ToggleToggles"].guiActive = !this.statercsEnabled;

            return 0f;
        }
    }
}