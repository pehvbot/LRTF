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
        public bool drainResources = false;

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
            foreach(string pump in pumps)
            {
                ConfigNode n = node.AddNode("FAILEDPUMPS");
                n.AddValue("pump", pump);
            }
        }

        public override void OnStartFinished(StartState state)
        {
            base.OnStartFinished(state);
            string oldResourceName = resourceName;
            started = false;
            foreach (string pump in pumps)
            {
                resourceName = pump;
                TestFlightUtil.GetCore(this.part, Configuration).TriggerNamedFailure(this.moduleName);
            }
            started = true;
            resourceName = oldResourceName;
        }

        public override void DoFailure()
        {
            base.DoFailure();
            SetState(false);
        }

        public override float DoRepair()
        {
            base.DoRepair();
            string oldResourceName = resourceName;
            resourceName = "ALL";
            SetState(true);
            resourceName = oldResourceName;

            Fields["pawMessage"].guiActive = false;
            return 0f;
        }

        private void SetState(bool show)
        {
            List<string> blacklist = this.resourceBlacklist.Split(',').ToList();
            List<PartResource> valid = null;
            for (int i = 0; i < base.part.Resources.ToList().Count; i++)
            {
                PartResource res = base.part.Resources.ToList()[i];
                if (!blacklist.Contains(res.resourceName) && res.info.resourceFlowMode != ResourceFlowMode.NO_FLOW)
                {
                    if (this.resourceName == "ALL" || res.resourceName == this.resourceName)
                    {
                        res.flowState = show;
                        res.hideFlow = !show;

                        if (drainResources)
                            res.amount = 0;

                        pawMessage = res.resourceName + " " + failureTitle;
                        Fields["pawMessage"].guiActive = !show;

                        if (started)
                            pumps.Add(res.resourceName);

                        return;
                    }
                    else if (this.resourceName == "ANY")
                    {
                        if (valid == null)
                        {
                            valid = new List<PartResource>();
                        }
                        valid.Add(res);
                    }
                }
            }
            if (this.resourceName == "ANY" && valid != null)
            {
                System.Random ran = new System.Random();
                int roll = ran.Next(0, valid.Count);

                valid[roll].flowState = show;
                valid[roll].hideFlow = !show;

                if (drainResources)
                    valid[roll].amount = 0;

                pawMessage = valid[roll].resourceName + " " + failureTitle;
                Fields["pawMessage"].guiActive = !show;

                if (started)
                    pumps.Add(valid[roll].resourceName);
            }
            MonoUtilities.RefreshPartContextWindow(part);
        }
    }

}