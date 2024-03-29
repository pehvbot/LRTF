﻿namespace TestFlight.LRTF
{
    public class LRTFFailure_Explode : LRTFFailureBase
    {
        /// <summary>
        /// Triggers the failure controlled by the failure module
        /// </summary>
        public override void DoFailure()
        {
            base.DoFailure();
            this.part.explode();
        }

        /// <summary>
        /// Asks the repair module if all condtions have been met for the player to attempt repair of the failure.  Here the module can verify things such as the conditions (landed, eva, splashed), parts requirements, etc
        /// </summary>
        /// <returns><c>true</c> if this instance can attempt repair; otherwise, <c>false</c>.</returns>
        public override bool CanAttemptRepair()
        {
            return false;
        }

        public override float DoRepair()
        {
            return -1;
        }
    }
}

