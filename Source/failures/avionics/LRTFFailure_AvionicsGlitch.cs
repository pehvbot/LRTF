using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

//using UnityEngine;

namespace TestFlight
{
    public class LRTFFailure_LRAvionicsGlitch : LRTFFailureBase_Avionics
    {
        [KSPField]
        public float maxDeadtime = 1f;
        [KSPField]
        public float maxWorkTime = 1f;

        private float currentInterval = 0;
        private float currentTime = 0;
        private bool state = false;
        private Random random;


        public override void OnLoad(ConfigNode node)
        {
            base.OnLoad(node);
            if (node.HasNode("FAILEDAVIONICS"))
            {
                this.currentInterval = float.Parse(node.GetNode("FAILEDAVIONICS").GetValue("currentInterval"));
                this.state = bool.Parse(node.GetNode("FAILEDAVIONICS").GetValue("currentTime"));
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


        public override void DoFailure()
        {
            base.DoFailure();
            this.random = new Random();
        }
        public override float Calculate(float value)
        {
            this.currentTime = this.currentTime + UnityEngine.Time.deltaTime;
            if (this.currentTime > this.currentInterval)
            {
                this.state = !this.state;
                this.currentTime = 0;
                this.currentInterval = (1 - (float)Math.Pow(this.random.NextDouble(), 2));
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