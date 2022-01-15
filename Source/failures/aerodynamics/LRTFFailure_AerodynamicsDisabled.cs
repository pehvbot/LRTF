using System;
using UnityEngine;

namespace TestFlight
{
    public class LRTFFailure_AerodynamicsDisabled : LRTFFailureBase_Aerodynamics
    {
        bool yawState;
        bool pitchState;
        bool rollState;

        public override void DoFailure()
        {
            controlSurface.Fields["ignorePitch"].guiActive = false;
            controlSurface.Fields["ignoreYaw"].guiActive = false;
            controlSurface.Fields["ignoreRoll"].guiActive = false;

            yawState = controlSurface.ignoreYaw;
            pitchState = controlSurface.ignorePitch;
            rollState = controlSurface.ignoreRoll;

            controlSurface.ignorePitch = true;
            controlSurface.ignoreRoll = true;
            controlSurface.ignoreYaw = true;

            base.DoFailure();
            if (part.PartActionWindow != null)
                part.PartActionWindow.displayDirty = true;
        }

        public override float DoRepair()
        {
            controlSurface.Fields["ignorePitch"].guiActive = true;
            controlSurface.Fields["ignoreYaw"].guiActive = true;
            controlSurface.Fields["ignoreRoll"].guiActive = true;

            controlSurface.ignorePitch = pitchState;
            controlSurface.ignoreRoll = rollState;
            controlSurface.ignoreYaw = yawState;
            return base.DoRepair();
        }
    }
}
