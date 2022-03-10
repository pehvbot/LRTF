using System;
using TestFlightAPI;
using KSP.UI.Screens;
using UnityEngine;
using System.Collections;

namespace TestFlight.LRTF
{

    public class LRTFFailureBase : TestFlightFailureBase
    {
        [KSPField(guiActive = false, guiName = "<b>TYPE</b>", guiActiveEditor = false, guiActiveUnfocused = true)]
        public string pawMessage = "Failure";

        [KSPField]
        public bool forceRepair = false;

        public bool allowRepair = true;
        public bool doTriggeredFailure = true;
        public bool partialFailed = false;
        public bool hasStarted = false;

        protected ITestFlightCore core = null;

        private float evaRepairDistance = 5;

        private double previousRepairChance = 0;

        private bool useRepairKit = false;
        private bool unfocusedRepair = false;


        //adjusters for RepairChance
        private double missionControlAdjuster = 0;
        private double engineerAdjuster = 0;
        private double crewAdjuster = 0;
        private double smartestAdjuster = 0;
        private double evaAdjuster = 0;
        private double repairKitAdjuster = 0;

        private int crewLevelBonus = 5;
        private double missionControlBonus = 0.3;
        private double noEngineerOnEVAPenalty = 0.5;

        public override void OnAwake()
        {
            if (core == null)
                core = TestFlightUtil.GetCore(this.part, Configuration);
            base.OnAwake();
        }
        
        public override void OnLoad(ConfigNode node)
        {
            pawMessage = failureTitle;
            foreach(ConfigNode n in GameDatabase.Instance.GetConfigNodes("LRTFSETTINGS"))
            {
                if(n.HasNode("REPAIRADJUSTERS"))
                {
                    ConfigNode adjusters = n.GetNode("REPAIRADJUSTERS");

                    adjusters.TryGetValue("evaRepairDistance", ref evaRepairDistance);

                    if (adjusters.HasNode(this.failureType.ToUpper()))
                    {
                        adjusters.GetNode(this.failureType.ToUpper()).TryGetValue("missionControlAdjuster", ref missionControlAdjuster);
                        adjusters.GetNode(this.failureType.ToUpper()).TryGetValue("engineerAdjuster", ref engineerAdjuster);
                        adjusters.GetNode(this.failureType.ToUpper()).TryGetValue("crewAdjuster", ref crewAdjuster);
                        adjusters.GetNode(this.failureType.ToUpper()).TryGetValue("smartestAdjuster", ref smartestAdjuster);
                        adjusters.GetNode(this.failureType.ToUpper()).TryGetValue("evaAdjuster", ref evaAdjuster);
                        adjusters.GetNode(this.failureType.ToUpper()).TryGetValue("repairKitAdjuster", ref repairKitAdjuster);
                        adjusters.GetNode(this.failureType.ToUpper()).TryGetValue("crewLevelBonus", ref crewLevelBonus);
                        adjusters.GetNode(this.failureType.ToUpper()).TryGetValue("missionControlBonus", ref missionControlBonus);
                        Math.Max(missionControlBonus, 1);
                        Math.Min(missionControlBonus, 0);
                        adjusters.GetNode(this.failureType.ToUpper()).TryGetValue("noEngineerOnEVAPenalty", ref noEngineerOnEVAPenalty);
                        Math.Max(noEngineerOnEVAPenalty, 1);
                        Math.Min(noEngineerOnEVAPenalty, 0);
                        adjusters.GetNode(this.failureType.ToUpper()).TryGetValue("unfocusedRepair", ref unfocusedRepair);
                    }
                }
            }
           
            node.TryGetValue("previousRepairChance", ref previousRepairChance);
            node.TryGetValue("partialFailed", ref partialFailed);

            node.TryGetValue("forceRepair", ref forceRepair);

            base.OnLoad(node);
        }

        public override void OnSave(ConfigNode node)
        {
            node.SetValue("previousRepairChance", previousRepairChance, true);
            node.SetValue("partialFailed", partialFailed, true);
            base.OnSave(node);
        }
        private void OnDestroy()
        {
            StopCoroutine(UpdatePAW());
            base.OnInactive();
        }
        
        public override void OnStartFinished(StartState state)
        {
            if (HighLogic.LoadedSceneIsFlight)
            {
                base.OnStartFinished(state);
                if ((Failed || partialFailed) && doTriggeredFailure)
                {
                    bool previousFailed = Failed;
                    Failed = false; 
                    core.TriggerNamedFailure(this.moduleName);
                    Failed = previousFailed; 
                }
                hasStarted = true;
            }
        }

        public override void DoFailure()
        {
            base.DoFailure();

            if (HighLogic.CurrentGame.Parameters.CustomParams<LRTFGameSettings>().lrtfLeaveTimeWarp)
                TimeWarp.SetRate(0, true);

            if (HighLogic.CurrentGame.Parameters.CustomParams<LRTFGameSettings>().lrtfSendMessage && hasStarted)
            {
                var failTime = KSPUtil.PrintTimeCompact((int)Math.Floor(this.vessel.missionTime), false);
                var failMessage = $"[{failTime}] {core.Title} has failed with {failureTitle}";
                MessageSystem.Message m = new MessageSystem.Message("TestFlight", failMessage, MessageSystemButton.MessageButtonColor.ORANGE, MessageSystemButton.ButtonIcons.ALERT);
                MessageSystem.Instance.AddMessage(m);
            }

            if (HighLogic.CurrentGame.Parameters.CustomParams<LRTFGameSettings>().lrtfOpenPAW && part.PartActionWindow == null && vessel.isActiveVessel && hasStarted)
                UIPartActionController.Instance.SpawnPartActionWindow(part);

            BasePAWGroup group = new BasePAWGroup();
            group.displayName = "[" + this.failureType + "] " + pawMessage;
            group.name = this.moduleName;

            Fields["pawMessage"].group = group;
            Events["TryRepair"].group = group;

            if (HighLogic.CurrentGame.Parameters.CustomParams<LRTFGameSettings>().lrtfEnableRepair && CanAttemptRepair())
            {
                Events["TryRepair"].guiName = $"<b>Attempt Repair</b>: {RepairChance():P}";
                Events["TryRepair"].guiActive = true;
                if (unfocusedRepair)
                {
                    Events["TryRepair"].guiActiveUnfocused = true;
                    Events["TryRepair"].guiActiveUncommand = true;
                    Events["TryRepair"].unfocusedRange = evaRepairDistance;
                }
                StartCoroutine(UpdatePAW());
            }
            else
            {
                Fields["pawMessage"].guiActive = true;
            }
        }

        private IEnumerator UpdatePAW()
        {
            while(failed || partialFailed)
            {
                double repairChance = RepairChance();
                if (repairChance > previousRepairChance) 
                {
                    Events["TryRepair"].guiName = $"<b>Attempt Repair</b>: {repairChance:P}";
                    Events["TryRepair"].guiActive = true;
                    if (unfocusedRepair)
                    {
                        Events["TryRepair"].guiActiveUnfocused = true;
                        Events["TryRepair"].guiActiveUncommand = true;
                        Events["TryRepair"].unfocusedRange = evaRepairDistance;
                    }
                    Fields["pawMessage"].guiActive = false;
                }
                else
                {
                    Events["TryRepair"].guiActive = false;
                    Events["TryRepair"].guiActiveUnfocused = false;
                    Events["TryRepair"].guiActiveUncommand = false;
                    Fields["pawMessage"].guiName = $"Last Repair Attempt: {previousRepairChance:P}";
                    Fields["pawMessage"].guiActive = true;
                }

                yield return null;
            }
        }

        public override float DoRepair()
        {

            Fields["pawMessage"].guiActive = false;
            Events["TryRepair"].guiActive = false;
            Events["TryRepair"].guiActiveUnfocused = false;
            Events["TryRepair"].guiActiveUncommand = false;

            if (part.PartActionWindow != null)
                part.PartActionWindow.displayDirty = true;

            partialFailed = false;

            return base.DoRepair();
        }

        [KSPEvent(guiName = "Repair", active = true, guiActive = false, guiActiveEditor = false, guiActiveUnfocused = false)]
        public void TryRepair()
        {
            double currentRepairChance = RepairChance();

            if (currentRepairChance > core.RandomGenerator.NextDouble() || forceRepair)
            {

                ScreenMessages.PostScreenMessage("The " + this.failureTitle + " on this part has been repaired!", 7);
                previousRepairChance = 0;

                DoRepair();
            }
            else
            {
                previousRepairChance = currentRepairChance;
                ScreenMessages.PostScreenMessage("<color=orange>Repairing the " + this.failureTitle + " on this part has failed!</color>\nYou can try again once you increase your understanding of this part or improve your repair capability", 7);
            }

            //remove repair kit if it exists on EVA

            bool consumedKit = false;

            if (useRepairKit)
            {
                //first try to remmove the evaRepairKit from active EVA kerbal 
                Vessel a = FlightGlobals.ActiveVessel;
                if(a.isEVA)
                {
                    float kerbalDistanceToPart = Vector3.Distance(a.transform.position, part.collider.ClosestPointOnBounds(a.transform.position));
                    if (kerbalDistanceToPart < evaRepairDistance && a.evaController.ModuleInventoryPartReference.ContainsPart("evaRepairKit"))
                    {
                        a.evaController.ModuleInventoryPartReference.RemoveNPartsFromInventory("evaRepairKit", 1);
                        consumedKit = true;
                    }
                }
                //otherwise try taking from another EVA kerbal
                if (!consumedKit)
                {
                    foreach (Vessel v in FlightGlobals.VesselsLoaded)
                    {
                        //first checks for Kerbals in physics range
                        if (v.isEVA && !consumedKit)
                        {
                            float kerbalDistanceToPart = Vector3.Distance(v.transform.position, part.collider.ClosestPointOnBounds(v.transform.position));
                            if (kerbalDistanceToPart < evaRepairDistance && v.evaController.ModuleInventoryPartReference.ContainsPart("evaRepairKit"))
                            {
                                v.evaController.ModuleInventoryPartReference.RemoveNPartsFromInventory("evaRepairKit", 1);
                                consumedKit = true;
                            }
                        }
                    }
                }
            }
            if (consumedKit)
                ScreenMessages.PostScreenMessage("<color=orange>You have used up an EVA Repair Kit in your attempt.</color>", 7);

        }

        private double RepairChance()
        {
            //Gets base repair chance which gets adjusted downwards depending on situation.
            float repairAdjuster = HighLogic.CurrentGame.Parameters.CustomParams<LRTFGameSettings>().lrtfRepairAdjuster - 1;

            double repairChance = core.FailureRateToMTBF(core.GetBaseReliabilityCurve().Evaluate(0), TestFlightUtil.MTBFUnits.SECONDS) * core.GetBaseFailureRate();
          
            repairChance = 1 - (repairAdjuster * 10 + 1) * repairChance / (repairAdjuster * 10 * repairChance + 1);

            //get situational adjusters
            double situationalAdjuster;
            double slackAdjuster = 1 - (missionControlAdjuster + engineerAdjuster + crewAdjuster + smartestAdjuster + evaAdjuster + repairKitAdjuster);

            double missionControlAdd = 0, engineerAdd = 0, crewAdd = 0, smartestAdd = 0, evaAdd = 0, repairKitAdd = 0;

            //mission control adjuster gives a min value of missionControlAdd for any connection
            if(this.vessel.connection != null && this.vessel.connection.IsConnected)
                missionControlAdd = missionControlBonus * missionControlAdjuster
                    + (1 - missionControlBonus) * missionControlAdjuster * ScenarioUpgradeableFacilities.GetFacilityLevel(SpaceCenterFacility.MissionControl);
         
            //crew adjusters
            bool hasTrainedCrew = false;
            bool hasEVA = false;
            bool hasEngineerEVA = false;
            bool hasRepairKit = false;
            double engineerLevel = 0;
            double highestLevel = 0;
            double stupidity = 1;

            foreach (var c in vessel.GetVesselCrew())
            {
                if (c.type == ProtoCrewMember.KerbalType.Crew)
                    hasTrainedCrew = true;
                if (c.stupidity < stupidity)
                    stupidity = c.stupidity;
                if (c.trait == "Engineer")
                    engineerLevel = Math.Max(engineerLevel, c.experienceLevel + crewLevelBonus);
                highestLevel = Math.Max(highestLevel, c.experienceLevel + crewLevelBonus);
            }

            //EVA adjuster
            if (evaAdjuster > 0)
            {
                foreach (var v in FlightGlobals.VesselsLoaded)
                {
                    //first checks for Kerbals in physics range
                    if (v.isEVA)
                    {
                        ProtoCrewMember member = v.GetVesselCrew()[0];
                        float kerbalDistanceToPart = Vector3.Distance(v.transform.position, part.collider.ClosestPointOnBounds(v.transform.position));
                        if (kerbalDistanceToPart < evaRepairDistance)
                        {
                            hasEVA = true;
                            if (member.type == ProtoCrewMember.KerbalType.Crew)
                                hasTrainedCrew = true;
                            if (member.stupidity < stupidity)
                                stupidity = member.stupidity;
                            if (member.trait == "Engineer")
                            {
                                hasEngineerEVA = true;
                                engineerLevel = Math.Max(engineerLevel, member.experienceLevel + crewLevelBonus);
                            }
                            highestLevel = Math.Max(highestLevel, member.experienceLevel + crewLevelBonus);
                            if (repairKitAdjuster > 0 && v.evaController.ModuleInventoryPartReference.ContainsPart("evaRepairKit"))
                                hasRepairKit = true;
                        }
                    }
                }
            }

            if (hasEVA)
            {
                evaAdd = evaAdjuster;
                if (!hasEngineerEVA)
                    evaAdd = evaAdjuster * noEngineerOnEVAPenalty;
            }

            if (hasTrainedCrew)
            {
                engineerAdd = engineerAdjuster * (float) engineerLevel / (crewLevelBonus + 5);
                crewAdd = crewAdjuster * (float) highestLevel / (crewLevelBonus + 5);
                smartestAdd = smartestAdjuster * (1 - stupidity);
            }

            if (hasRepairKit)
            {
                repairKitAdd = repairKitAdjuster;
                useRepairKit = true;
            }

            situationalAdjuster = missionControlAdd + engineerAdd + crewAdd + smartestAdd + evaAdd + repairKitAdd;

            //if adjusters are wrong, this corrects for it.
            //fugly math here.  Better method?
            if (slackAdjuster > 0)
                situationalAdjuster += slackAdjuster;
            else
                situationalAdjuster /= (slackAdjuster + 1);

            //keeps things between 0.01 and .99 chance
            return Math.Max(Math.Min(Math.Ceiling(repairChance * situationalAdjuster * 100) / 100, 0.99), 0.01);            
        }

    }
}