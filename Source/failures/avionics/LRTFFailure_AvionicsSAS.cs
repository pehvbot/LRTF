using UnityEngine;
using System.Collections;

namespace TestFlight
{
    public class LRTFFailure_AvionicsSAS : LRTFFailureBase
    {
        private ModuleSAS sas;

        public override void OnStart(PartModule.StartState state)
        {
            base.OnStart(state);
            this.sas = base.part.FindModuleImplementing<ModuleSAS>();
        }


        //SAS won't load until the craft is 'in flight'
        //If DoFailure happens too soon, repairs won't show repaired SAS level.
        //Ugly kludge to get around this.
        public override void OnStartFinished(StartState state)
        {
            doTriggeredFailure = false;
            base.OnStartFinished(state);
            if (failed)
                StartCoroutine(DoFailureDelayed());
        }

        private IEnumerator DoFailureDelayed()
        {
            for (int i = 0; i < 20; i++)
                yield return null;
            DoFailure();
        }

        public override void DoFailure()
        {
            base.DoFailure();
            sas.moduleIsEnabled = false;
        }

        public override float DoRepair()
        {
            base.DoRepair();
            sas.moduleIsEnabled = true;
            return 0f;
        }
    }
}