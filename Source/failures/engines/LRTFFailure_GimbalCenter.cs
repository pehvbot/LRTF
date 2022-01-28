using System;
using System.Collections.Generic;
using UnityEngine;

namespace TestFlight.LRTF
{
    public class LRTFFailure_GimbalCenter : LRTFFailureBase_Gimbal
    {
        private List<Quaternion> _initRots;

        private float baseRange = 10f;
        private float angle1;
        private float angle2;
        
        private List<Quaternion> initRots
        {
            get
            {
                if (this._initRots == null)
                {
                    this._initRots = base.module.initRots;
                }
                return this._initRots;
            }
        }

        public override void OnLoad(ConfigNode node)
        {
            node.TryGetValue("baseRange", ref baseRange);
            node.TryGetValue("angle1", ref angle1);
            node.TryGetValue("angle2", ref angle2);
            base.OnLoad(node);
        }
        public override void OnSave(ConfigNode node)
        {
            node.SetValue("baseRange", baseRange, true);
            node.SetValue("angle1", angle1, true);
            node.SetValue("angle2", angle2, true);

            base.OnSave(node);
        }

        public override void OnStart(PartModule.StartState state)
        {
            base.OnStart(state);
            List<ModuleGimbal> gimbals = part.Modules.GetModules<ModuleGimbal>(); 
            foreach(var g in gimbals)
            {
                if (g.gimbalTransformName == base.gimbalTransformName)
                {
                    this.baseRange = g.gimbalRange;
                    break;
                }
            }
        }
        public override void DoFailure()
        {
            //initRots[0] = Quaternion.
            if (hasStarted)
            {
                 angle1 = UnityEngine.Random.Range(-this.baseRange, this.baseRange);
                 angle2 = UnityEngine.Random.Range(-this.baseRange, this.baseRange);
            }
            float range = this.baseRange - Math.Max(Math.Abs(angle1), Math.Abs(angle2));
            for (int i = 0; i < base.module.initRots.Count; i++)
            {
                this.module.initRots[i] = Quaternion.AngleAxis(angle1, Vector3.forward) * Quaternion.AngleAxis(angle2, Vector3.left) * initRots[i];
            }
            base.module.gimbalRange = range;
            base.DoFailure();
        }
        public override float DoRepair()
        {
            base.DoRepair();
            base.module.initRots = this.initRots;
            base.module.gimbalRange = this.baseRange;
            return 0f;
        }
    }
}
