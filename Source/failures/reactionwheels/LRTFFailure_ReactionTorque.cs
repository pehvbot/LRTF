using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TestFlightAPI;
using UnityEngine;

namespace TestFlight
{
    public class LRTFFailure_ReactionTorque : LRTFFailureBase_ReactionWheel
    {
        private float PitchTorque;
        private float RollTorque;
        private float YawTorque;
        private float failedPitchTorque;
        private float failedRollTorque;
        private float failedYawTorque;
        private int working;
        private bool loadFailure = false;

        public override void OnLoad(ConfigNode node)
        {
            base.OnLoad(node);
            if (node.HasNode("FAILEDTORQUE"))
            {
                failedPitchTorque = float.Parse(node.GetNode("FAILEDTORQUE").GetValue("failedPitchTorque"));
                failedRollTorque = float.Parse(node.GetNode("FAILEDTORQUE").GetValue("failedRollTorque"));
                failedYawTorque = float.Parse(node.GetNode("FAILEDTORQUE").GetValue("failedYawTorque"));
            }
        }

        public override void OnSave(ConfigNode node)
        {
            base.OnSave(node);
            if(failed)
            {
                ConfigNode n = node.AddNode("FAILEDTORQUE");
                n.AddValue("failedPitchTorque", base.module.PitchTorque);
                n.AddValue("failedRollTorque", base.module.RollTorque);
                n.AddValue("failedYawTorque", base.module.YawTorque);
            }
        }

        public override void OnStartFinished(StartState state)
        {
            base.OnStartFinished(state);
            if (failed)
            {
                loadFailure = true;
            }
        }

        public override void DoFailure()
        {
            base.DoFailure();

            if (base.module != null)
            {
                if (loadFailure)
                {
                    base.module.PitchTorque = failedPitchTorque;
                    base.module.RollTorque = failedRollTorque;
                    base.module.YawTorque = failedYawTorque;
                    loadFailure = false;
                }
                else
                {
                    this.PitchTorque = base.module.PitchTorque;
                    this.RollTorque = base.module.RollTorque;
                    this.YawTorque = base.module.YawTorque;
                    this.working = 7;

                    System.Random ran = new System.Random();
                    int axis = ran.Next(0, 3);
                    float modifier;
                    while ((this.working & 2 ^ axis) != 0)
                    {
                        this.working -= 2 ^ axis;
                        modifier = ((float)ran.NextDouble() * 2f) - 1f;
                        switch (axis)
                        {
                            case 0: //pitch
                                base.module.PitchTorque = this.PitchTorque * modifier;
                                break;
                            case 1: //roll
                                base.module.RollTorque = this.RollTorque * modifier;
                                break;
                            case 2: //yaw
                                base.module.YawTorque = this.YawTorque * modifier;
                                break;
                        }
                        axis = ran.Next(0, 6); //yes axis are only 0 1 2, but this lowers chance for a 2nd axis failure
                    }
                }
            }
        }
        public override float DoRepair()
        {
            base.DoRepair();
            base.module.PitchTorque = this.PitchTorque;
            base.module.RollTorque = this.RollTorque;
            base.module.YawTorque = this.YawTorque;
            return 0f;
        }
    }
}