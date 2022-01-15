using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace TestFlight
{
    public class LRTFFailure_AvionicsGlitch : LRTFFailureBase_Avionics
    {
        [KSPField]
        public float maxDeadtime = 1f;
        [KSPField]
        public float maxWorkTime = 1f;

        private float currentInterval = 0;
        private float currentTime = 0;
        private bool state = false;


        public override void OnLoad(ConfigNode node)
        {
            base.OnLoad(node);
            if (failed && node.HasNode("FAILEDAVIONICS"))
            {
                node.GetNode("FAILEDAVIONICS").TryGetValue("currentInterval", ref currentInterval);
                node.GetNode("FAILEDAVIONICS").TryGetValue("state", ref state);
             }
        }

        public override void OnSave(ConfigNode node)
        {
            base.OnSave(node);
            if (failed && node.HasNode("FAILEDAVIONICS"))
            {
                ConfigNode n = node.GetNode("FAILEDAVIONICS");
                n.AddValue("currentInterval", this.currentInterval);
                n.AddValue("state", this.state);
            }
        }

        public override float Calculate(float value)
        {
            this.currentTime = this.currentTime + UnityEngine.Time.deltaTime;

            if (this.currentTime > this.currentInterval)
            {
                this.state = !this.state;
                this.currentTime = 0;
                this.currentInterval = (1 - (float)Math.Pow(core.RandomGenerator.NextDouble(), 2));
                if (this.state)
                {
                    this.currentInterval = this.currentInterval * this.maxWorkTime;
                }
                else
                {
                    this.currentInterval = this.currentInterval * this.maxDeadtime;
                }
            }
            if (!this.state)
            {
                return 0;
            }
            return value;
        }
    }
}