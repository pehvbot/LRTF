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
    public class FlightDataRecorder_LRResources : FlightDataRecorderBase
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
            bool isRecording = true;

            if (!isEnabled)
                return false;

            if (this.part.vessel.situation == Vessel.Situations.PRELAUNCH)
                return false;

            List<PartResource> partResources = this.part.Resources.ToList();
            foreach (PartResource resource in partResources)
            {
                if (resourceName == "ALL")
                {
                    if (resource.amount <= emptyThreshold)
                        isRecording = false;
                }
                else if (resourceName == "" || resourceName == resource.resourceName)
                {
                    isRecording = false;
                    if (resource.amount > emptyThreshold)
                        return true;
                }
            }
            return isRecording;
        }
    }
}