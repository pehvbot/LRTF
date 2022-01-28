using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TestFlightAPI;

namespace TestFlight.LRTF
{
    public class LRTFFailureBase_ReactionWheel : LRTFFailureBase
    {
        protected ModuleReactionWheel module;

        public override void OnStart(StartState state)
        {
            base.OnStart(state);
            module = base.part.FindModuleImplementing<ModuleReactionWheel>();
        }
    }
}