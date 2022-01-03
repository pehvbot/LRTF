using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using TestFlightCore;
using TestFlightAPI;

namespace TestFlight.Failure_Modules
{
    public class LRTFFailure_RCSShutdown : LRTFFailureBase_RCS
    {
        private bool stateEnabled;
        private bool statercsEnabled;

        public override void DoFailure()
        {   
            this.stateEnabled = base.rcs.enabled;
            this.statercsEnabled = base.rcs.rcsEnabled;
           
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
            base.rcs.enabled = this.stateEnabled;
            base.rcs.rcsEnabled = this.statercsEnabled;

            base.rcs.Events["ToggleToggles"].active = !this.statercsEnabled;
            base.rcs.Events["ToggleToggles"].guiActive = !this.statercsEnabled;

            return 0f;
        }
    }
}