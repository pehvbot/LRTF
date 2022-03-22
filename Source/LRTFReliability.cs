/// Directly copied from the core TestFlight code v2.2.0.1
/// https://github.com/KSP-RO/testflight

using System;
using System.Collections.Generic;
using UnityEngine;
using TestFlightAPI;


namespace TestFlight.LRTF
{
    /// <summary>
    /// This part module determines the part's current reliability and passes that on to the TestFlight core.
    /// </summary>
    public class LRTFReliability : TestFlightReliabilityBase
    {
        [KSPField]
        public ConfigNode lrtfReliabilityCurve;
        [KSPField(isPersistant = true)]
        public FloatCurve calculatedReliabilityCurve;

        private float MTBF = 600f;
        private float maxData = 10000f;
        private float cycleReliabilityStart = 0.25f;
        private float cycleReliabilityEnd = 0.99f;
        private float kinkV = 0.75f;
        private float kinkH = 0.225f;
        private float kinkW = 0.5f;

        public override void OnLoad(ConfigNode node)
        {
            base.OnLoad(node);
            //Checks for game start part loading and sets variables
            if (node.HasNode("lrtfReliabilityCurve"))
            {
                node.GetNode("lrtfReliabilityCurve").TryGetValue("MTBF", ref MTBF);
                node.GetNode("lrtfReliabilityCurve").TryGetValue("maxData", ref maxData);
                node.GetNode("lrtfReliabilityCurve").TryGetValue("cycleReliabilityStart", ref cycleReliabilityStart);
                node.GetNode("lrtfReliabilityCurve").TryGetValue("cycleReliabilityEnd", ref cycleReliabilityEnd);
                node.GetNode("lrtfReliabilityCurve").TryGetValue("kinkV", ref kinkV);
                node.GetNode("lrtfReliabilityCurve").TryGetValue("kinkH", ref kinkH);
                node.GetNode("lrtfReliabilityCurve").TryGetValue("kinkW", ref kinkW);
            }

            
            //Checks for part being loaded into the scene 
            if(HighLogic.CurrentGame != null)
            {
                //partSeed is the same for all parts of that name and that game save
                int partSeed = HighLogic.CurrentGame.Seed + TestFlightUtil.GetPartName(part).GetHashCode();
                float scaler = HighLogic.CurrentGame.Parameters.CustomParams<LRTFGameSettings>().lrtfFailureCurveScaler/5;

                //randomizers.  uses new 'r' variable to avoid doubling randomizing
                //increments partSeed by 1 to get the same random value per save+part combo
                float rcycleReliabilityStart = cycleReliabilityStart + Randomize(cycleReliabilityStart, cycleReliabilityEnd, partSeed, scaler);
                float rcycleReliabilityEnd = cycleReliabilityEnd + Randomize(cycleReliabilityEnd, 1, partSeed + 1, scaler);
                float rkinkH = kinkH + Randomize(0, kinkH, partSeed + 2, scaler);
                float rkinkV = kinkV + Randomize(kinkV, 1, partSeed + 3, scaler);
                float rkinkW = kinkW + Randomize(0, kinkW, partSeed + 4, scaler*2);

                //calculate failchance start and end using cyleReliability
                float failChanceStart = -(float)Math.Log(rcycleReliabilityStart) / MTBF;
                float failChanceEnd = -(float)Math.Log(rcycleReliabilityEnd) / MTBF;

                //calculate kink time
                float kinkTime = rkinkH * maxData; 
                //calculate kink value
                float kinkValue = ((failChanceEnd - failChanceStart) * rkinkV) + failChanceStart;
                //calcualte kink tangent
                float kinkTangent = ((failChanceEnd - failChanceStart) * rkinkW / maxData) + ((failChanceEnd - kinkValue) / (maxData - kinkTime) * (1 - rkinkW));

                //create keys
                reliabilityCurve = new FloatCurve();
                reliabilityCurve.Add(0, failChanceStart);
                reliabilityCurve.Add(kinkTime, kinkValue, kinkTangent, kinkTangent);
                reliabilityCurve.Add(maxData, failChanceEnd, 0, 0);

                //saves a copy of the reliabilityCurve to the save file for evaluation.
                calculatedReliabilityCurve = reliabilityCurve;
            }
        }

        private float Randomize(float start, float end, int seed, float scaler)
        {
            KSPRandom r = new KSPRandom(seed);
            return (((end - start) * (float)r.NextDouble() - (end - start) / 2)) * scaler;
        }

        public override string GetModuleInfo(string configuration, float reliabilityAtTime)
        {
            foreach (var configNode in configs)
            {
                if (!configNode.HasValue("configuration"))
                    continue;

                var nodeConfiguration = configNode.GetValue("configuration");

                if (string.Equals(nodeConfiguration, configuration, StringComparison.InvariantCultureIgnoreCase))
                {
                    if (configNode.HasNode("reliabilityCurve"))
                    {
                        var nodeReliability = new FloatCurve();
                        nodeReliability.Load(configNode.GetNode("reliabilityCurve"));

                        // core is not yet available here
                        float reliabilityMin = TestFlightUtil.FailureRateToReliability(nodeReliability.Evaluate(nodeReliability.minTime), reliabilityAtTime);
                        float reliabilityMax = TestFlightUtil.FailureRateToReliability(nodeReliability.Evaluate(nodeReliability.maxTime), reliabilityAtTime);
                        return $"  Reliability at 0 data: <color=#b1cc00ff>{reliabilityMin:P1}</color>\n  Reliability at max data: <color=#b1cc00ff>{reliabilityMax:p1}</color>";
                    }
                }
            }

            return base.GetModuleInfo(configuration, reliabilityAtTime);
        }

        public override List<string> GetTestFlightInfo(float reliabilityAtTime)
        {
            List<string> infoStrings = new List<string>();

            


            if (core == null)
            {
                Log("Core is null");
                return infoStrings;
            }
            if (reliabilityCurve == null)
            {
                Log("Curve is null");
                return infoStrings;
            }

            float flightData = core.GetFlightData();
            if (flightData < 0f)
                flightData = 0f;

            double currentFailRate = GetBaseFailureRate(flightData);
            double maxFailRate = GetBaseFailureRate(reliabilityCurve.maxTime);

            double currentReliability = TestFlightUtil.FailureRateToReliability(currentFailRate, reliabilityAtTime);
            double maxReliability = TestFlightUtil.FailureRateToReliability(maxFailRate, reliabilityAtTime);

            string currentMTBF = core.FailureRateToMTBFString(currentFailRate, TestFlightUtil.MTBFUnits.SECONDS, 999);
            string maxMTBF = core.FailureRateToMTBFString(maxFailRate, TestFlightUtil.MTBFUnits.SECONDS, 999);

            infoStrings.Add("<b>Base Reliability</b>");
            infoStrings.Add($"<b>Current Reliability</b>: {currentReliability:P1} at full burn, {currentMTBF} <b>MTBF</b>");
            infoStrings.Add($"<b>Maximum Reliability</b>: {maxReliability:P1} at full burn, {maxMTBF} <b>MTBF</b>");

            return infoStrings;
        }
    }
}