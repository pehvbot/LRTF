namespace TestFlight.LRTF
{
    public class LRTFFailure_EngineCoolant : LRTFFailureBase_Engine
    {
        [KSPField]
        public float heatMultiplier = 3.0F;

        public override void DoFailure()
        {
            foreach (EngineHandler engine in engines)
            {
                ModuleEngines module = (ModuleEngines)engine.engine.Module;
                if (module != null)
                {
                    module.heatProduction *= heatMultiplier;
                }
            }
            base.DoFailure();

        }
        public override float DoRepair()
        {
            base.DoRepair();
            foreach (EngineHandler engine in engines)
            {
                ModuleEngines module = (ModuleEngines)engine.engine.Module;
                if (module != null)
                {
                    module.heatProduction /= heatMultiplier;
                }
            }
            return 0f;
        }
    }
}
