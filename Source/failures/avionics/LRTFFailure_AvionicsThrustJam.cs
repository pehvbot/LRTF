namespace TestFlight.LRTF
{
    public class LRTFFailure_AvionicsThrustJam : LRTFFailureBase_Avionics
    {
        [KSPField(isPersistant = true)]
        public float throttle;

        public override void DoFailure()
        {
            if (base.vessel != null && base.part != null && base.vessel.referenceTransformId == base.part.flightID)
            {
                base.vessel.OnFlyByWire -= this.OnFlyByWire;
                base.vessel.OnFlyByWire += this.OnFlyByWire;
                this.throttle = base.vessel.ctrlState.mainThrottle;
            }
            base.DoFailure();
        }
        public override float DoRepair()
        {
            base.DoRepair();
            if (base.vessel != null)
            {
                base.vessel.OnFlyByWire -= this.OnFlyByWire;
            }
            return 0f;
        }
        public void OnFlyByWire(FlightCtrlState s)
        {
            s.mainThrottle = this.throttle;
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
