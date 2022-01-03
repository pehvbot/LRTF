using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using TestFlightAPI;

namespace TestFlight
{
    public class LRTFFailure_ResourceLeak : LRTFFailureBase
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
        public bool includeResourceInPAW = true;

        [KSPField(isPersistant = true)]
        public bool isLeaking = false;
     
        private List<ResourceLeak> leaks;
        private List<ResourceLeak> newLeaks;

        private bool started = true;

        private float _initialAmount, _perSecondAmount;

        public class ResourceLeak : IConfigNode
        {
            public int id = 0;
            public double amount;
            public double initialAmount;

            public void Load(ConfigNode node)
            {
                id = int.Parse(node.GetValue("id"));
                amount = double.Parse(node.GetValue("amount"));
                initialAmount = 0d; // if we're loading, the initial leak has occurred.
            }

            public void Save(ConfigNode node)
            {
                node.AddValue("id", id);
                node.AddValue("amount", amount.ToString("G17"));
            }

            public ResourceLeak(int newId, double newAmount, double newInit)
            {
                id = newId;
                amount = newAmount;
                initialAmount = newInit;
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
            newLeaks = new List<ResourceLeak>();
        }

        public override void OnLoad(ConfigNode node)
        {
            base.OnLoad(node);
            if (node.HasNode("LEAK"))
            {
                leaks.Clear();
                foreach (ConfigNode n in node.GetNodes("LEAK"))
                    leaks.Add(new ResourceLeak(n));
            }
        }

        public override void OnSave(ConfigNode node)
        {
            base.OnSave(node);

            if (node.HasNode("LEAK"))
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

        public override void OnStartFinished(StartState state)
        {
            if (isLeaking)
                started = false;
            base.OnStartFinished(state);
        }

        public override void DoFailure()
        {

            //get resource(s) to leak
            if (started)
            {
                List<string> blacklist = this.resourceBlacklist.Split(',').ToList();
                
                if (resourceToLeak.ToUpper() == "ALL")
                {
                    foreach (PartResource r in this.part.Resources)
                    {
                        int resID = r.info.id;
                        ParseResourceValues(resID);
                        if(!blacklist.Contains(r.resourceName))
                            newLeaks.Add(new ResourceLeak(resID, _perSecondAmount, _initialAmount));
                    }
                    if (includeResourceInPAW)
                        pawMessage = failureTitle + " : All";
                }
                else if (resourceToLeak.ToUpper() == "ANY")
                {

                    if (part.Resources.Count > 0)
                    {
                        List<PartResource> allResources = this.part.Resources.ToList();
                        
                        foreach(PartResource r in allResources)
                        {
                            foreach(string l in blacklist)
                            {
                                if (r.resourceName == l)
                                    allResources.Remove(r);
                            }
                        }
                        int randomResource = TestFlightUtil.GetCore(this.part, Configuration).RandomGenerator.Next(0, allResources.Count());
                        int resID = allResources[randomResource].info.id;
                        ParseResourceValues(resID);
                        newLeaks.Add(new ResourceLeak(resID, _perSecondAmount, _initialAmount));
                        if (includeResourceInPAW)
                            pawMessage = failureTitle + " : " + allResources[randomResource].resourceName;
                    }
                }
                else if (this.part.Resources.Contains(resourceToLeak))
                {
                    int resID = this.part.Resources.Get(resourceToLeak).info.id;
                    ParseResourceValues(resID);
                    newLeaks.Add(new ResourceLeak(resID, _perSecondAmount, _initialAmount));
                    if (includeResourceInPAW)
                        pawMessage = failureTitle + " : " + resourceToLeak;
                }
                foreach (ResourceLeak newLeak in newLeaks)
                {
                    //bool save = true;
                    //foreach (ResourceLeak leak in leaks)
                    //{
                    //    if (leak.id == newLeak.id)
                    //        save = false;
                    //}
                    if (!leaks.Contains(newLeak))
                    {
                        leaks.Add(newLeak);
                    }
                }
            }

            foreach (ResourceLeak leak in leaks)
            {
                isLeaking = true;
                started = true;
                ParseResourceValues(leak.id);
                this.part.RequestResource(leak.id, leak.initialAmount, ResourceFlowMode.NO_FLOW);
            }
          
            base.DoFailure();
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
            newLeaks.RemoveRange(0, newLeaks.Count);
            return base.DoRepair();
        }
    }
}

