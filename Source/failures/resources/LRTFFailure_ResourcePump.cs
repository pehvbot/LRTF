using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using TestFlightAPI;

namespace TestFlight
{
    public class LRTFFailure_ResourcePump : LRTFFailureBase_Resource
    {
        [KSPField]
        public string resourceName = "ANY";
        [KSPField]
        public string resourceBlacklist = "";
        [KSPField]
        public bool drainResource = false;
        [KSPField]
        public bool includeResourceInPAW = false;
        [KSPField]
        public bool allowHiddenResources = false;

        private List<string> pumps;

        public override void OnAwake()
        {
            base.OnAwake();
            pumps = new List<string>();
        }

        public override void OnLoad(ConfigNode node)
        {
            base.OnLoad(node);
            if (node.HasNode("FAILEDPUMPS"))
            {
                foreach(string pump in node.GetNode("FAILEDPUMPS").GetValues("pump"))
                {
                    pumps.Add(pump);
                }
            }
        }

        public override void OnSave(ConfigNode node)
        {
            base.OnSave(node);

            if(node.HasNode("FAILEDPUMPS"))
                node.RemoveNode("FAILEDPUMPS");
            ConfigNode n = node.AddNode("FAILEDPUMPS");

            foreach (string pump in pumps)
            {
                n.AddValue("pump", pump);
            }
        }

        public override void OnStart(StartState state)
        {
            base.OnStart(state);

            //look for any failable resources, disables if nothing is available
            int count = 0;
            List<string> blacklist = this.resourceBlacklist.Split(',').ToList();
            if (resourceName == "" && blacklist.Count > 0)
            {
                foreach (PartResource candidate in this.part.Resources)
                {
                    if (!blacklist.Contains(candidate.resourceName))
                        count++;
                }
                if (count == 0)
                    core.DisableFailure(this.moduleName);
            }
        }

        public override void DoFailure()
        {
            List<string> blacklist = this.resourceBlacklist.Split(',').ToList();
            List<PartResource> availableResources = new List<PartResource>();
            List<PartResource> failedResources = new List<PartResource>();

            foreach (PartResource candidate in this.part.Resources)
            {
                if (!blacklist.Contains(candidate.resourceName) && !pumps.Contains(candidate.resourceName) && (allowHiddenResources || candidate.isVisible))
                    availableResources.Add(candidate);
            }

            if (hasStarted)
            {
                if (availableResources.Count > 0)
                {
                    if (resourceName.ToUpper() == "ANY")
                    {
                        int randomResource = TestFlightUtil.GetCore(this.part, Configuration).RandomGenerator.Next(0, availableResources.Count);
                        failedResources.Add(availableResources[randomResource]);
                        pumps.Add(availableResources[randomResource].resourceName);
                    }
                    else foreach (PartResource resource in availableResources)
                    {
                            if (this.resourceName.ToUpper() == "ALL" || resource.resourceName.ToUpper() == this.resourceName.ToUpper())
                            {
                                failedResources.Add(resource);
                                pumps.Add(resource.resourceName);
                            }
                    }
                }
            }
            else
            {
                foreach(PartResource resource in this.part.Resources)
                {
                    if (pumps.Contains(resource.resourceName))
                        failedResources.Add(resource);
                }
            }

            foreach(var resource in failedResources)
            {
                resource.flowState = false;
                resource.hideFlow = true;
                string message = failureTitle;

                if (drainResource)
                    resource.amount = 0;

                if (resourceTitles.HasValue(resource.resourceName))
                    message = resourceTitles.GetValue(resource.resourceName);

                if (includeResourceInPAW)
                    pawMessage = message + " : " + resource.resourceName;
            }
            
            if (part.PartActionWindow != null)
                part.PartActionWindow.displayDirty = true;

            if (!hasStarted)
                Failed = true;

            base.DoFailure();

            //checks for partial failures
            //checks there isn't a resourceToLeak and resources still available.
            if (string.IsNullOrEmpty(resourceName) || resourceName.ToUpper() == "ANY")
            {
                foreach (PartResource candidate in availableResources)
                    if (!failedResources.Contains(candidate))
                    {
                        partialFailed = true;
                        Failed = false;
                    }
            }
        }

        public override float DoRepair()
        {
            base.DoRepair();
            foreach (PartResource fixResource in part.Resources)
            {
                fixResource.flowState = true;
                fixResource.hideFlow = false;
            }
            pumps = new List<string>();

            if (part.PartActionWindow != null)
                part.PartActionWindow.displayDirty = true;

            return 0f;
        }
    }
}