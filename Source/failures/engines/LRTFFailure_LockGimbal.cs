using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using TestFlightAPI;

namespace TestFlight
{
    public class LRTFFailure_LockGimbal : LRTFFailureBase
    {
        /// <summary>
        /// Triggers the failure controlled by the failure module
        /// </summary>

        private float gimbalRange;

        public override void OnStart(StartState state)
        {
            base.OnStart(state);
            if (Failed)
                DoFailure();
        }

        public override void DoFailure()
        {
            base.DoFailure();
            List<ModuleGimbal> gimbals = part.Modules.OfType<ModuleGimbal>().ToList();
            foreach (ModuleGimbal gimbal in gimbals)
            {
                gimbalRange = gimbal.gimbalRange;
                gimbal.gimbalRange = 0f;
                gimbal.gimbalLock = true;
                gimbal.gimbalActive = false;
            }
        }

        public override float DoRepair()
        {
            base.DoRepair();
            List<ModuleGimbal> gimbals = this.part.Modules.OfType<ModuleGimbal>().ToList();
            foreach (ModuleGimbal gimbal in gimbals)
            {
                gimbal.gimbalRange = gimbalRange;
                gimbal.gimbalLock = false;
            }

            return 0;
        }
    }
}

