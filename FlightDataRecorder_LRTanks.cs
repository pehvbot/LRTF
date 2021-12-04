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
    public class FlightDataRecorder_LRTanks : FlightDataRecorderBase
    {
        [KSPField]
        public double emptyThreshold = 0.1;
        [KSPField]
        public string resourceNames = "";

        public override void OnAwake()
        {
            base.OnAwake();
        }

        public override bool IsRecordingFlightData()
        {
            if (!isEnabled)
                return false;

            if (this.part.vessel.situation == Vessel.Situations.PRELAUNCH)
                return false;

            bool isRecording = false;

            //strip spaces
            resourceNames = String.Concat(resourceNames.Where(c => !Char.IsWhiteSpace(c)));
            string[] names = resourceNames.Split(',');

            List<PartResource> partResources = this.part.Resources.ToList();

            if (resourceNames.ToUpper() == "ALL")
            {
                isRecording = true;
                foreach (PartResource resource in partResources)
                {
                    if (resource.amount < emptyThreshold || !resource.flowState)
                        isRecording = false;
                }
            }
            else if(resourceNames == "")
            {
                foreach (PartResource resource in partResources)
                {
                    if (resource.amount >= emptyThreshold && resource.flowState)
                        isRecording = true;
                }
            }
            else
            {
                isRecording = true;

                foreach (PartResource resource in partResources)
                {
                    bool hasName = false;
                    foreach (string name in names)
                    {
                        if (resource.resourceName == name)
                            hasName = true;
                    }
                    if (!hasName || resource.amount < emptyThreshold|| !resource.flowState)
                        isRecording = false;
                }
            }

            return isRecording;
        }
    }
}