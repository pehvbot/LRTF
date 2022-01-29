using System;
using TestFlightAPI;

namespace TestFlight.LRTF
{
    public class LRTFFailureBase_Avionics : LRTFFailureBase
    {
        [KSPField]
        public bool includeAxisInPAW = true;

        [KSPField]
        public bool includeTranslate = false;

        public enum FailedState
        {
            Pitch = 0,
            Roll = 1,
            Yaw = 2,
            Rotate = 3,
            TranX = 4,
            TranY = 5,
            TranZ = 6,
            Translate = 7
        }
        
        [KSPField]
        public bool isFlyByWire = false;

        public FailedState failedState;
        public float failedValue;

        private ModuleCommand moduleCommand;

        private bool hasHibernation;

        public override void OnAwake()
        {
            base.OnActive();
            moduleCommand = part.FindModuleImplementing<ModuleCommand>();
        }
        public override void OnLoad(ConfigNode node)
        {
            base.OnLoad(node);
            if (node.HasNode("FAILEDAVIONICS"))
            {
                this.failedState = (FailedState)Enum.Parse(typeof(FailedState), node.GetNode("FAILEDAVIONICS").GetValue("failedState"));
                this.failedValue = float.Parse(node.GetNode("FAILEDAVIONICS").GetValue("failedValue"));
            }
        }

        public override void OnSave(ConfigNode node)
        {
            base.OnSave(node);
            if (failed)
            {
                ConfigNode n = node.AddNode("FAILEDAVIONICS");
                n.AddValue("failedState", this.failedState);
                n.AddValue("failedValue", this.failedValue);
            }
        }

        public override void DoFailure()
        {
            if(core == null)
                core = TestFlightUtil.GetCore(this.part, Configuration);

            System.Random ran = new System.Random();
            if (hasStarted)
            {
                this.failedValue = 1f - (float)Math.Pow(ran.NextDouble(), 2);
                if(includeTranslate)
                    this.failedState = (FailedState)ran.Next(0, 8);
                else
                    this.failedState = (FailedState)ran.Next(0, 4);
            }
            base.vessel.OnFlyByWire -= this.OnFlyByWire;
            base.vessel.OnFlyByWire += this.OnFlyByWire;

            if (includeAxisInPAW)
                pawMessage = failureTitle + " : " + failedState;

            hasHibernation = moduleCommand.hasHibernation;
            moduleCommand.hasHibernation = true;
            moduleCommand.Fields["hibernation"].guiActive = true;

            base.DoFailure();
        }
        public override float DoRepair()
        {
            base.DoRepair();

            if (base.vessel != null)
            {
                base.vessel.OnFlyByWire -= this.OnFlyByWire;
            }

            moduleCommand.hibernation = false; //turn off hibernation just in case button is hidden
            moduleCommand.hasHibernation = hasHibernation;
            moduleCommand.Fields["hibernation"].guiActive = hasHibernation;

            return 0f;
        }
        public virtual void OnFlyByWire(FlightCtrlState s)
        {
            if (base.vessel == null || base.vessel != FlightGlobals.ActiveVessel || base.part.isControlSource != Vessel.ControlLevel.FULL)
            {
                return;
            }

            switch (this.failedState)
            {

                case FailedState.Pitch:
                    s.pitch = Calculate(s.pitch);
                    break;
                case FailedState.Roll:
                    s.roll = Calculate(s.roll);
                    break;
                case FailedState.Yaw:
                    s.yaw = Calculate(s.yaw);
                    break;
                case FailedState.Rotate:
                    s.pitch = Calculate(s.pitch);
                    s.roll = Calculate(s.roll);
                    s.yaw = Calculate(s.yaw);
                    break;
                case FailedState.TranX:
                    s.X = Calculate(s.X);
                    break;
                case FailedState.TranY:
                    s.Y = Calculate(s.Y);
                    break;
                case FailedState.TranZ:
                    s.Z = Calculate(s.Z);
                    break;
                case FailedState.Translate:
                    s.X = Calculate(s.X);
                    s.Y = Calculate(s.Y);
                    s.Z = Calculate(s.Z);
                    break;
            }
        }

        public virtual float Calculate(float value)
        {
            return value;
        }
        public void OnDestroy()
        {
            if (base.vessel != null)
            {
                base.vessel.OnFlyByWire -= this.OnFlyByWire;
            }
        }
    }
}
