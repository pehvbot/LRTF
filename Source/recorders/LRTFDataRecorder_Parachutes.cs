using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TestFlightAPI;
using UnityEngine;

namespace TestFlight.Flight_Recorders
{
    public class LRTFDataRecorder_Parachutes : FlightDataRecorderBase
    {
        ModuleParachute chute;
        public override void OnStart(PartModule.StartState state)
        {
            base.OnStart(state);
            chute = base.part.FindModuleImplementing<ModuleParachute>();
        }
        public override void OnAwake()
        {
            base.OnAwake();
        }

        public override bool IsPartOperating()
        {
            if (!(isEnabled || HighLogic.CurrentGame.Parameters.CustomParams<LRTFGameSettings>().lrtfParachutes) || TimeWarp.CurrentRate > 4)
                return false;

            return (chute.deploymentState == ModuleParachute.deploymentStates.ACTIVE || chute.deploymentState == ModuleParachute.deploymentStates.SEMIDEPLOYED || chute.deploymentState == ModuleParachute.deploymentStates.DEPLOYED);
        }

        public override bool IsRecordingFlightData()
        {
            return IsPartOperating();
        }
    }
}
