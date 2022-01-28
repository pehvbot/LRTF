using System.Collections.Generic;
using System.Linq;

namespace TestFlight.LRTF
{
    public class LRTFFailure_LockGimbal : LRTFFailureBase
    {
        List<bool> gimbalLocks;

        public override void OnAwake()
        {
            gimbalLocks = new List<bool>();
            base.OnAwake();
        }
        public override void OnSave(ConfigNode node)
        {
            if(gimbalLocks.Count > 0)
            {
                node.RemoveNode("GIMBALLOCKS");
                ConfigNode saveLocks = new ConfigNode();
                foreach(bool g in gimbalLocks)
                {
                    saveLocks.AddValue("gimbalLock", g);
                }
                node.AddNode("GIMBALLOCKS", saveLocks);
            }
            base.OnSave(node);
        }

        public override void OnLoad(ConfigNode node)
        {
            if(node.HasNode("GIMBALLOCKS"))
            {
                foreach(string g in node.GetNode("GIMBALLOCKS").GetValues("gimbalLock"))
                {
                    gimbalLocks.Add(bool.Parse(g));
                }
            }
            base.OnLoad(node);
        }
        public override void DoFailure()
        {
            List<ModuleGimbal> gimbals = part.Modules.OfType<ModuleGimbal>().ToList();
            foreach (ModuleGimbal gimbal in gimbals)
            {
                if(hasStarted)
                    gimbalLocks.Add(gimbal.gimbalLock);
                gimbal.gimbalLock = true;
                gimbal.Fields["gimbalLock"].guiActive = false;
                gimbal.Fields["gimbalLimiter"].guiActive = false;
            }
            base.DoFailure();
        }

        public override float DoRepair()
        {
            base.DoRepair();
            List<ModuleGimbal> gimbals = this.part.Modules.OfType<ModuleGimbal>().ToList();
            int g = 0;
            foreach (ModuleGimbal gimbal in gimbals)
            {
                gimbal.gimbalLock = gimbalLocks[g++];
                gimbal.Fields["gimbalLock"].guiActive = true;
                gimbal.Fields["gimbalLimiter"].guiActive = true;
            }
            gimbalLocks = new List<bool>();

            return 0;
        }
    }
}

