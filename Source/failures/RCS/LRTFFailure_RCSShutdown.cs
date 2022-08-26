using System.Collections;

namespace TestFlight.LRTF
{
    public class LRTFFailure_RCSShutdown : LRTFFailureBase_RCS
    {
        [KSPField(isPersistant = true)]
        private float previousThrustPower;
        [KSPField(isPersistant = true)]
        private bool stateEnabled;
        [KSPField(isPersistant = true)]
        private bool statercsEnabled;

        public override void DoFailure()
        {   
            stateEnabled = base.rcs.enabled;
            statercsEnabled = base.rcs.rcsEnabled;
            previousThrustPower = base.rcs.thrustPercentage;

            base.rcs.thrustPercentage = 0;

            StartCoroutine(DoFailureDelayed());

            base.DoFailure();
        }

        public override float DoRepair()
        {
            base.DoRepair();
            base.rcs.thrustPercentage = previousThrustPower;
            base.rcs.enabled = stateEnabled;
            base.rcs.rcsEnabled = statercsEnabled;

            base.rcs.Events["ToggleToggles"].active = !statercsEnabled;
            base.rcs.Events["ToggleToggles"].guiActive = !statercsEnabled;

            return 0f;
        }

        private IEnumerator DoFailureDelayed()
        {
            for (int i = 0; i < 5; i++)
                yield return null;

            base.rcs.enabled = false;
            base.rcs.rcsEnabled = false;

            base.rcs.Events["ToggleToggles"].active = false;
            base.rcs.Events["ToggleToggles"].guiActive = false;
        }
    }
}