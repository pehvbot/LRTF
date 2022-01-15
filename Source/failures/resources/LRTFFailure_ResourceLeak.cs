using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using TestFlightAPI;

namespace TestFlight
{
    public class LRTFFailure_ResourceLeak : LRTFFailureBase_Resource
    {
        [KSPField]
        public string resourceToLeak = "ANY";
        [KSPField]
        public string initialAmount = "10";
        [KSPField]
        public string perSecondAmount = "0.1";
        [KSPField]
        public bool calculatePerTick = false;
        [KSPField]
        public string resourceBlacklist = "";
        [KSPField]
        public bool includeResourceInPAW = false;
        [KSPField]
        public bool allowHiddenResources = false;

        [KSPField(isPersistant = true)]
        public bool isLeaking = false;

        private List<ResourceLeak> leaks;

        private float _initialAmount, _perSecondAmount;

        public class ResourceLeak : IConfigNode
        {
            public int id = 0;
            public double amount;
            public double initialAmount;
            public string resourceName;

            public void Load(ConfigNode node)
            {
                id = int.Parse(node.GetValue("id"));
                amount = double.Parse(node.GetValue("amount"));
                initialAmount = 0d; // if we're loading, the initial leak has occurred.
                resourceName = node.GetValue("resourceName");
            }

            public void Save(ConfigNode node)
            {
                node.AddValue("id", id);
                node.AddValue("amount", amount.ToString("G17"));
                node.AddValue("resourceName", resourceName);
            }

            public ResourceLeak(int newId, double newAmount, double newInit, string newName)
            {
                id = newId;
                amount = newAmount;
                initialAmount = newInit;
                resourceName = newName;
            }

            public ResourceLeak(ConfigNode node)
            {
                Load(node);
            }
        }

        public override void OnAwake()
        {
            base.OnAwake();
            leaks = new List<ResourceLeak>();
        }

        public override void OnSave(ConfigNode node)
        {
            base.OnSave(node);

            node.RemoveNodes("LEAK");

            if (leaks.Count > 0)
            {
                foreach (ResourceLeak leak in leaks)
                {
                    ConfigNode n = node.AddNode("LEAK");
                    leak.Save(n);
                }
            }
        }

        public override void OnStart(StartState state)
        {
            base.OnStart(state);

            //look for any failable resources, disables if nothing is available
            int count = 0;
            List<string> blacklist = this.resourceBlacklist.Split(',').ToList();
            if (resourceToLeak == "" && blacklist.Count > 0)
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

            foreach(PartResource candidate in this.part.Resources)
            {
                if (!blacklist.Contains(candidate.resourceName) && (allowHiddenResources || candidate.isVisible))
                {
                    bool available = true;
                    foreach(ResourceLeak l in leaks)
                    {
                        if (l.resourceName == candidate.resourceName)
                            available = false;
                    }
                    if (available)
                        availableResources.Add(candidate);
                }
            }

            if (hasStarted && availableResources.Count > 0)
            {
                if (resourceToLeak.ToUpper() == "ALL")
                {
                    foreach (PartResource r in availableResources)
                    {
                        int resID = r.info.id;
                        ParseResourceValues(resID);
                        leaks.Add(new ResourceLeak(resID, _perSecondAmount, _initialAmount, r.resourceName));
                    }
                    if (includeResourceInPAW)
                        pawMessage = failureTitle + " : All";
                }
                else if (resourceToLeak.ToUpper() == "ANY")
                {
                    int randomResource = TestFlightUtil.GetCore(this.part, Configuration).RandomGenerator.Next(0, availableResources.Count);
                    int resID = availableResources[randomResource].info.id;
                    ParseResourceValues(resID);
                    leaks.Add(new ResourceLeak(resID, _perSecondAmount, _initialAmount, availableResources[randomResource].resourceName));
                }
                else if (this.part.Resources.Contains(resourceToLeak))
                {
                    int resID = this.part.Resources.Get(resourceToLeak).info.id;
                    ParseResourceValues(resID);
                    leaks.Add(new ResourceLeak(resID, _perSecondAmount, _initialAmount, resourceToLeak));

                }
            }
         
            foreach (ResourceLeak leak in leaks)
            {
                isLeaking = true;
                string message = failureTitle;

                if (resourceTitles.HasValue(leak.resourceName))
                    message = resourceTitles.GetValue(leak.resourceName);

                ParseResourceValues(leak.id);
                this.part.RequestResource(leak.id, leak.initialAmount, ResourceFlowMode.NO_FLOW);

                pawMessage = message;
            }

            if (!hasStarted)
                Failed = true;

            base.DoFailure();

            //checks for partial failures
            //checks there isn't a resourceToLeak and resources still available.
            if (string.IsNullOrEmpty(resourceToLeak) || resourceToLeak.ToUpper() == "ANY")
            {
                foreach (PartResource candidate in availableResources)
                {
                    bool notThere = true;
                    foreach (ResourceLeak leak in leaks)
                    {
                        if (leak.resourceName == candidate.resourceName)
                            notThere = false;
                    }
                    if (notThere)
                    {
                        partialFailed = true;
                        Failed = false;
                    }
                }
            }
        }

        public void FixedUpdate()
        {
            if (HighLogic.LoadedSceneIsFlight && isLeaking)
            {
                ResourceLeak leak;
                for (int i = leaks.Count - 1; i >= 0; --i)
                {
                    leak = leaks[i];
                    if (calculatePerTick)
                    {
                        leak.amount = ParseValue(perSecondAmount, leak.id);
                    }
                    this.part.RequestResource(leak.id, _perSecondAmount * TimeWarp.fixedDeltaTime, ResourceFlowMode.NO_FLOW);
                }
            }
        }

        private float ParseValue(string rawValue, int leakingResourceID)
        {
            float parsedValue = 0f;
            int index = rawValue.IndexOf("%");
            string trimmedValue = "";
            double calculateFromAmount = 0;

            rawValue = rawValue.ToLowerInvariant();

            if (index > 0)
            {
                if (rawValue.EndsWith("%t"))
                {
                    trimmedValue = rawValue.Substring(0, index);
                    if (!float.TryParse(trimmedValue, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.CreateSpecificCulture("en-US"), out parsedValue))
                        parsedValue = 0f;
                    // Calculate the % value based on the total capacity of the tank
                    calculateFromAmount = this.part.Resources.Get(leakingResourceID).maxAmount;
                    Log(String.Format("Calculating leak amount from maxAmount: {0:F2}", calculateFromAmount));
                }
                else if (rawValue.EndsWith("%c"))
                {
                    trimmedValue = rawValue.Substring(0, index);
                    if (!float.TryParse(trimmedValue, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.CreateSpecificCulture("en-US"), out parsedValue))
                        parsedValue = 0f;
                    // Calculate the % value based on the current resource level of the tank
                    calculateFromAmount = this.part.Resources.Get(leakingResourceID).amount;
                    Log(String.Format("Calculating leak amount from current amount: {0:F2}", calculateFromAmount));
                }
                Log(String.Format("Base value was parsed as: {0:F2}", parsedValue));
                parsedValue = parsedValue * (float)calculateFromAmount;
                Log(String.Format("Calculated leak: {0:F2}", parsedValue));
            }
            else
            {
                if (!float.TryParse(rawValue, out parsedValue))
                    parsedValue = 0f;
            }

            return parsedValue;
        }

        private void ParseResourceValues(int resID)
        {
            _initialAmount = ParseValue(initialAmount, resID);
            _perSecondAmount = ParseValue(perSecondAmount, resID);
        }

        public override float DoRepair()
        {
            isLeaking = false;
            leaks.RemoveRange(0, leaks.Count);
            return base.DoRepair();
        }
    }
}

