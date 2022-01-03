﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using TestFlightAPI;

namespace TestFlight
{
    public class LRTFFailure_ParachuPartial : LRTFFailureBase
    {
        private ModuleParachute chute;
        private float deployAltitude;

        public override void OnStart(PartModule.StartState state)
        {
            base.OnStart(state);
            this.chute = base.part.FindModuleImplementing<ModuleParachute>();
        }
 
        public override void DoFailure()
        {
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
            if (chute.deploymentState == ModuleParachute.deploymentStates.DEPLOYED)
                chute.AssumeDragCubePosition("DEPLOYED");
            return 0f;
        }
    }
}