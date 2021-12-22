using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

using TestFlightAPI;

namespace TestFlight
{
    public class LRTFDataRecorder_Tanks : FlightDataRecorderBase
    {
        [KSPField]
        public double emptyThreshold = 0.1;
        [KSPField]
        public string resourceNames = "ANY";


        private int ticker = 1;
        private bool isRecording;
        private Dictionary<string, double> resourceAmounts = new Dictionary<string, double>();

        public override void OnLoad(ConfigNode node)
        {
            base.OnLoad(node);
        }

        public override bool IsPartOperating()
        {
            if (!(isEnabled && HighLogic.CurrentGame.Parameters.CustomParams<LRTFGameSettings>().lrtfResources))
                return false;

            //strip spaces
            resourceNames = String.Concat(resourceNames.Where(c => !Char.IsWhiteSpace(c))).ToUpper();
            if (resourceNames == "") resourceNames = "ANY";
            string[] needsResources = resourceNames.Split(',');

            bool willRecord = false;

            List<PartResource> partResources = this.part.Resources.ToList();

            //spamming PartResource.amout causes incoherence with the data.
            //checks every 50 cycles. stores the last known state to
            //continue this state until the next check
            //ticker keeps track of cycles.
            if (ticker++ % 50 == 0)
            {
                foreach (PartResource resource in partResources)
                {
                    //looks for change in at least one item in resourceNames or anything
                    if (resourceNames == "ANY" || Array.Exists(needsResources, element => element == resource.resourceName.ToUpper()))
                    {
                        if (resourceAmounts.ContainsKey(resource.resourceName))
                        {
                            if (resource.amount != resourceAmounts[resource.resourceName] && resource.amount >= emptyThreshold)
                            {
                                willRecord = true;
                                resourceAmounts[resource.resourceName] = resource.amount;
                            }
                        }
                        else
                            resourceAmounts.Add(resource.resourceName, resource.amount);
                    }
                }
                //Doesn't check while in time warp
                if (TimeWarp.CurrentRate <= 4)
                    isRecording = willRecord;
                else
                    isRecording = false;
            }
            
            return isRecording;
        }

        public override bool IsRecordingFlightData()
        {
            if (this.part.vessel.situation == Vessel.Situations.PRELAUNCH)
                return false;

            return IsPartOperating();
        }
    }
}