using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace TestFlight
{
    public class LRTFFailure_GimbalSpeed : LRTFFailureBase_Gimbal
    {
        private float baseSpeed = 10.0f;
        private float gimbalSpeed = 0;

        public override void OnLoad(ConfigNode node)
        {
            node.TryGetValue("baseSpeed", ref baseSpeed);
            node.TryGetValue("gimbalSpeed", ref gimbalSpeed);
            base.OnLoad(node);
        }

        public override void OnSave(ConfigNode node)
        {
            node.SetValue("baseSpeed", baseSpeed, true);
            node.SetValue("gimbalSpeed", gimbalSpeed, true);
            base.OnSave(node);
        }

        public override void OnStart(PartModule.StartState state)
        {
            base.OnStart(state);

            List<ModuleGimbal> gimbals = part.Modules.GetModules<ModuleGimbal>();
            foreach (var g in gimbals)
            {
                if (g.gimbalTransformName == base.gimbalTransformName)
                {
                    this.baseSpeed = g.gimbalResponseSpeed;
                    break;
                }
            }
        }
        public override void DoFailure()
        {
            if(hasStarted)
                gimbalSpeed = UnityEngine.Random.Range(0, base.module.gimbalResponseSpeed);
            base.module.gimbalResponseSpeed = gimbalSpeed;
            base.DoFailure();
        }
        public override float DoRepair()
        {
            base.DoRepair();
            base.module.gimbalResponseSpeed = this.baseSpeed;
            return 0f;
        }
    }
}