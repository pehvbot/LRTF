using TestFlightCore;
using UnityEngine;

namespace TestFlightAPI
{
    public class LRTFDataRecorderBase : FlightDataRecorderBase
    {
        [KSPField]
        public string additionalDataRecorders = "";

        private float previousData;
        private float additionalDataRecordersMax = 10000;

        private ITestFlightCore core = null;

        private ListDictionary<string, float> dataRecorders = new ListDictionary<string, float>();

        public new void OnEnable()
        {
            if (core == null)
                core = TestFlightUtil.GetCore(this.part, Configuration);
        }

        public override void OnLoad(ConfigNode node)
        {
            base.OnLoad(node);
            node.TryGetValue("additionalDataRecorders", ref additionalDataRecorders);
            if(additionalDataRecorders.Trim() != "")
            {
                string[] branches = additionalDataRecorders.Split(new char[1] { ',' });
                foreach(string branch in branches)
                {
                    string[] recorder = branch.Split(new char[1] { ':' });
                    if(recorder.Length == 2)
                    {
                        float percent = float.Parse(recorder[1]);
                        if (percent > 0)
                            dataRecorders.Add(recorder[0], percent);
                    }
                }
            }
        }

        public override void OnStart(StartState state)
        {
            base.OnStart(state);
            if (core == null)
                core = TestFlightUtil.GetCore(this.part, Configuration);

            ConfigNode paramsNode = null;

            foreach (ConfigNode n in GameDatabase.Instance.GetConfigNodes("LRTFSETTINGS"))
                paramsNode = n;

            if (paramsNode == null)
                return;

            paramsNode.TryGetValue("additionalDataRecordersMax", ref additionalDataRecordersMax);
        }

        public override void OnSave(ConfigNode node)
        {
            if (HighLogic.LoadedSceneIsFlight)
            {
                if (previousData == 0)
                    previousData = core.GetInitialFlightData();

                foreach (var data in dataRecorders)
                {
                    if (TestFlightManagerScenario.Instance.GetFlightDataForPartName(data.Key) < additionalDataRecordersMax)
                    {
                        float flightData = (core.GetFlightData() - previousData) * (data.Value[0] / 100);
                        if (flightData > 0)
                            TestFlightManagerScenario.Instance.AddFlightDataForPartName(data.Key, flightData);
                        if (TestFlightManagerScenario.Instance.GetFlightDataForPartName(data.Key) > additionalDataRecordersMax)
                            TestFlightManagerScenario.Instance.SetFlightDataForPartName(data.Key, additionalDataRecordersMax);
                    }
                }
                previousData = core.GetFlightData();
            }
            base.OnSave(node);
        }

        public override bool IsRecordingFlightData()
        {
            if(vessel.isActiveVessel)
                return base.IsRecordingFlightData();
            return false;
        }
    }
}
