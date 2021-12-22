using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using TestFlightAPI;

namespace TestFlight
{
    public class TestFlightFailure_LRSASFailure : LRTFFailureBase
    {
        private ModuleSAS sas;

        public override void OnStart(PartModule.StartState state)
        {
            base.OnStart(state);
            this.sas = base.part.FindModuleImplementing<ModuleSAS>();
        }
        public override void OnStartFinished(StartState state)
        {
            base.OnStartFinished(state);
            if (failed)
                TestFlightUtil.GetCore(this.part, Configuration).TriggerNamedFailure("TestFlightFailure_LRSASFailure");
        }

        public override void DoFailure()
        {
            sas.moduleIsEnabled = false;
            
            base.DoFailure();
            pawMessage = failureTitle;
            Fields["pawMessage"].guiActive = true;
        }
        public override float DoRepair()
        {
            base.DoRepair();
            sas.moduleIsEnabled = true;
            return 0f;
        }
    }
}