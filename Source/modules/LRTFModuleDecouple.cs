using System.Collections;

namespace TestFlight.LRTF
{

    public class LRTFModuleDecouple : ModuleDecouple
    {
        [KSPField(isPersistant = true)]
        public bool canDecouple = true;

        public bool isDecoupling = false;

        private IEnumerator DoDecoupleDelayed()
        {
            for (int i = 0; i < 20; i++)
                yield return null;

            isDecoupling = false;
            if (canDecouple)
                base.OnDecouple();
        }

        public override void OnDecouple()
        {
            isDecoupling = true;
            StartCoroutine(DoDecoupleDelayed());
        }
    }

    public class LRTFModuleAnchoredDecoupler : ModuleAnchoredDecoupler
    {
        [KSPField(isPersistant = true)]
        public bool canDecouple = true;

        public bool isDecoupling = false;

        private IEnumerator DoDecoupleDelayed()
        {
            for (int i = 0; i < 20; i++)
                yield return null;

            isDecoupling = false;
            if (canDecouple)
                base.OnDecouple();
        }

        public override void OnDecouple()
        {
            isDecoupling = true;
            StartCoroutine(DoDecoupleDelayed());
        }
    }
}
