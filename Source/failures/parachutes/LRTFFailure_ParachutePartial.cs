using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using TestFlightAPI;

namespace TestFlight
{
    public class TestFlightFailure_LRParachuPartial : LRTFFailureBase
    {
        private ModuleParachute chute;
        private float deployAltitude;

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
            
            deployAltitude = chute.deployAltitude;
            chute.deployAltitude = 0;
            chute.Fields["deployAltitude"].guiActive = false;

            if (chute.deploymentState == ModuleParachute.deploymentStates.DEPLOYED)
                chute.AssumeDragCubePosition("SEMIDEPLOYED");
            
            base.DoFailure();

        }

        public override float DoRepair()
        {
            base.DoRepair();
            chute.deployAltitude = deployAltitude;
            chute.Fields["deployAltitude"].guiActive = true;
            Fields["pawMessage"].guiActive = false;
            return 0f;
        }
    }
}