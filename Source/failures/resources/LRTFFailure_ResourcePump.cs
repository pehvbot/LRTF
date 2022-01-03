using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using TestFlightAPI;

namespace TestFlight.Failure_Modules
{
    public class LRTFFailure_ResourcePump : LRTFFailureBase
    {
        [KSPField]
        public string resourceName = "ANY";
        [KSPField]
        public string resourceBlacklist = "";
        [KSPField]
        public bool drainResource = false;
        [KSPField]
        public bool includeResourceInPAW = true;

        private List<string> pumps;

        private bool started = true;

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

        public override void OnStartFinished(StartState state)
        {
            string oldResourceName = resourceName;
            started = false;
            foreach (string pump in pumps)
            {
                resourceName = pump;
                TestFlightUtil.GetCore(this.part, Configuration).TriggerNamedFailure(this.moduleName);
            }
            started = true;
            resourceName = oldResourceName;

            doTriggeredFailure = false;
            base.OnStartFinished(state);
        }

        public override void DoFailure()
        {
            List<string> blacklist = this.resourceBlacklist.Split(',').ToList();
            List<PartResource> valid = null;

            foreach (PartResource checkResource in part.Resources)
            {
                if (!blacklist.Contains(checkResource.resourceName))
                {
                    if (this.resourceName.ToUpper() == "ALL" || checkResource.resourceName.ToUpper() == this.resourceName.ToUpper())
                    {
                        checkResource.flowState = false;
                        checkResource.hideFlow = true;

                        if (drainResource)
                            checkResource.amount = 0;

                        if (started && !pumps.Contains(checkResource.resourceName))
                        {
                            pumps.Add(checkResource.resourceName);
                        }
                    }
                    else if (this.resourceName.ToUpper() == "ANY")
                    {
                        if (valid == null)
                        {
                            valid = new List<PartResource>();
                        }
                        valid.Add(checkResource);
                    }
                }
            }

            if (this.resourceName.ToUpper() == "ANY" && valid != null)
            {
                System.Random ran = new System.Random();
                int roll = ran.Next(0, valid.Count);

                valid[roll].flowState = false;
                valid[roll].hideFlow = true;

                if (drainResource)
                    valid[roll].amount = 0;

                if(includeResourceInPAW)
                    pawMessage = failureTitle + " : " + valid[roll].resourceName;

                if (started && !pumps.Contains(valid[roll].resourceName))
                    pumps.Add(valid[roll].resourceName);
            }

            if (part.PartActionWindow != null)
                part.PartActionWindow.displayDirty = true;

            base.DoFailure();
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