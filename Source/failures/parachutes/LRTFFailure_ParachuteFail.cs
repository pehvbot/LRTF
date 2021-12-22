using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using TestFlightAPI;

namespace TestFlight
{
    public class TestFlightFailure_LRParachuteFail : LRTFFailureBase
    {
        private ModuleParachute chute;

        public override void OnStart(PartModule.StartState state)
        {
            base.OnStart(state);
            this.chute = base.part.FindModuleImplementing<ModuleParachute>();
        }
        public override void OnStartFinished(StartState state)
        {
            base.OnStartFinished(state);
            if (failed)
                TestFlightUtil.GetCore(this.part, Configuration).TriggerNamedFailure(this.moduleName);
        }

        public override void DoFailure()
        {
            pawMessage = failureTitle;
            Fields["pawMessage"].guiActive = true;

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