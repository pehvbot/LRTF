using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TestFlightAPI;
using KSP;

namespace TestFlight
{
    public class LRTFFailureBase_Gimbal : LRTFFailureBase
    {
        [KSPField(isPersistant = false)]
        public string gimbalTransformName = "RANDOM";

        public override void OnLoad(ConfigNode node)
        {
            node.TryGetValue("gimbalTransformName", ref gimbalTransformName);
            base.OnLoad(node);
        }

        public override void OnSave(ConfigNode node)
        {
            node.SetValue("gimbalTransformName", gimbalTransformName, true);
            base.OnSave(node);
        }

        protected ModuleGimbal module;

        public override void OnStart(PartModule.StartState state)
        {
            base.OnStart(state);
            List<ModuleGimbal> gimbals = part.Modules.GetModules<ModuleGimbal>();

            if (this.gimbalTransformName != "RANDOM")
            {
                foreach(var g in gimbals)
                {
                    if(g.gimbalTransformName == gimbalTransformName && !g.gimbalLock && g.gimbalRange > 0f)
                    {
                        module = g;
                        return;
                    }
                }
                gimbalTransformName = "RANDOM";
            }
            if (this.gimbalTransformName == "RANDOM")
            {
                List<ModuleGimbal> valid = new List<ModuleGimbal>();
                foreach (var g in gimbals)
                {
                    ModuleGimbal gimbal = g;
                    if (!g.gimbalLock && g.gimbalRange > 0f)
                    {
                        valid.Add(g);
                    }
                }
                int roll = UnityEngine.Random.Range(0, valid.Count);
                module = valid[roll];
                gimbalTransformName = module.gimbalTransformName;
            }
        }
    }
}