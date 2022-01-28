using UnityEngine;

namespace TestFlight.LRTF
{
    public class LRTFFailure_AvionicsClamp : LRTFFailureBase_Avionics
    {
        public override float Calculate(float value)
        {
            return Mathf.Clamp(value, -this.failedValue, this.failedValue);
        }
    }
}