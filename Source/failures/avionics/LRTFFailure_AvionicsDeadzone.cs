using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TestFlight
{
    public class LRTFFailure_AvionicsDeadzone : LRTFFailureBase_Avionics
    {
        private float deadStart;
        private float deadEnd;

        public override void OnLoad(ConfigNode node)
        {
            base.OnLoad(node);
            if (node.HasNode("FAILEDAVIONICS"))
            {
                this.deadStart = float.Parse(node.GetNode("FAILEDAVIONICS").GetValue("deadStart"));
                this.deadEnd = float.Parse(node.GetNode("FAILEDAVIONICS").GetValue("deadEnd"));
            }
        }

        public override void OnSave(ConfigNode node)
        {
            base.OnSave(node);
            if (failed && node.HasNode("FAILEDAVIONICS"))
            {
                ConfigNode n = node.GetNode("FAILEDAVIONICS");
                n.AddValue("deadStart", this.deadStart);
                n.AddValue("deadEnd", this.deadEnd);
            }
        }

        public override void DoFailure()
        {
            base.DoFailure();
            if (!loadFailure)
            {
                Random ran = new Random();
                float range = (float)ran.NextDouble() * 0.25f;
                float center = (float)ran.NextDouble() * 2 - 1;
                this.deadStart = Math.Max(center - range, -1f);
                this.deadEnd = Math.Min(center + range, 1f);
            }
        }

        public override float Calculate(float value)
        {
            if (value > this.deadStart && value < this.deadEnd)
            {
                return 0;
            }
            return value;
        }
    }
}