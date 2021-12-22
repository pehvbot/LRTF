using System;
using System.Collections.Generic;
using TestFlightAPI;
using KSP.UI.Screens;

namespace TestFlight
{

    public class LRTFFailureBase : TestFlightFailureBase
    {
        [KSPField(groupDisplayName = "Less Real TestFlight Failures", groupName = "LRTestFlight",
            guiActive = false, guiName = "<b>TYPE</b>", guiActiveEditor = false, guiActiveUnfocused = true)]
        public string pawMessage = "Failure";


        public override void DoFailure()
        {
            base.DoFailure();

            ITestFlightCore core = TestFlightUtil.GetCore(this.part, Configuration);

            if (HighLogic.CurrentGame.Parameters.CustomParams<LRTFGameSettings>().lrtfLeaveTimeWarp)
                TimeWarp.SetRate(0, true);

            if (HighLogic.CurrentGame.Parameters.CustomParams<LRTFGameSettings>().lrtfSendMessage)
            {
                var failTime = KSPUtil.PrintTimeCompact((int)Math.Floor(this.vessel.missionTime), false);
                var failMessage = $"[{failTime}] {core.Title} has failed with {failureTitle}";
               
                MessageSystem.Message m = new MessageSystem.Message("TestFlight", failMessage, MessageSystemButton.MessageButtonColor.ORANGE, MessageSystemButton.ButtonIcons.ALERT);
                MessageSystem.Instance.AddMessage(m);
            }
        }
    }
}