using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using TestFlightAPI;

namespace TestFlight
{
    public class LRTFFailure_ParachuteFail : LRTFFailureBase
    {
        private ModuleParachute chute;

        public override void OnStart(PartModule.StartState state)
        {
            base.OnStart(state);
            this.chute = base.part.FindModuleImplementing<ModuleParachute>();
        }

        public override void DoFailure()
        {
            base.DoFailure();
            if (chute.deploymentState == ModuleParachute.deploymentStates.SEMIDEPLOYED || chute.deploymentState == ModuleParachute.deploymentStates.DEPLOYED)
            {
                chute.CutParachute();
            }
            else
            {
                chute.deploymentState = ModuleParachute.deploymentStates.STOWED;
                chute.enabled = false;
            }
        }

        public override float DoRepair()
        {
            base.DoRepair();
            chute.enabled = true;
            return 0f;
        }
    }
}