using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

using TestFlightAPI;

namespace TestFlight
{

    // Method for determing distance from kerbal to part
    // float kerbalDistanceToPart = Vector3.Distance(kerbal.transform.position, targetPart.collider.ClosestPointOnBounds(kerbal.transform.position));
    public class FlightDataRecorder_LRAvionics : FlightDataRecorderBase
    {
        [KSPField]
        public double emptyThreshold = 0.1;
        [KSPField]
        public string resourceName = "";

        public override void OnAwake()
        {
            base.OnAwake();
        }

        public override bool IsRecordingFlightData()
        {
            if (!isEnabled)
                return false;

            //if (this.part.vessel.situation == Vessel.Situations.PRELAUNCH)
            //    return false;

            PartModuleList mods = this.part.Modules;
            ModuleCommand mod = (ModuleCommand)mods.GetModule("ModuleCommand");
            //hibernating protects the probe
            if(!mod.IsHibernating)
                if (mod.ModuleState == ModuleCommand.ModuleControlState.Nominal || mod.ModuleState == ModuleCommand.ModuleControlState.PartialProbe)
                    return true;

            return false;
        }
    }
}