//Lifted from OhScrap
//https://github.com/zer0Kerbal/OhScrap

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;
//using RealChute;
using FerramAerospaceResearch.RealChuteLite;

namespace LRTF
{
    //Collection of small classes used to provide basic support for other mods via reflection. 
    public class ModWrapper
    {
        private static readonly BindingFlags flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static;


        public class RemoteTechWrapper
        {
            private static Assembly RT = null;
            private static bool tried = false;
            public static bool available
            {
                get
                {
                    bool loaded = (RT != null);
                    if (!loaded && !tried)
                    {
                        for (int i = 0; i < AssemblyLoader.loadedAssemblies.Count; i++)
                        {
                            var Asm = AssemblyLoader.loadedAssemblies[i];
                            if (Asm.dllName == "RemoteTech")
                            {
                                loaded = true;
                                //RTAntenna = Asm.assembly.GetType("RemoteTech.Modules.ModuleRTAntenna");
                                RT = Asm.assembly;
                                Debug.Log("[OhScrap]: RemoteTech Detected.");
                            }
                        }
                        tried = true;
                    }
                    return loaded;
                }
            }

            public static void SetRTBrokenStatus(PartModule p, bool value)
            {
                SetReflectionField<bool>(p, "IsRTBroken", value);
                SetReflectionField<bool>(p, "IsRTActive", value);
                if (value == false)
                {
                    p.GetType().GetMethod("OnConnectionRefresh").Invoke(p, null);
                }
            }
            public static bool HasConnectionToKSC(Guid vesselGUID)
            {
                object[] parametersArray = new object[1];
                parametersArray[0] = vesselGUID;
                ParameterInfo[] parameters = RT.GetType("RemoteTech.API.API").GetMethod("HasConnectionToKSC").GetParameters();
                return (bool)RT.GetType("RemoteTech.API.API").GetMethod("HasConnectionToKSC").Invoke(RT, parameters.Length == 0 ? null : parametersArray);
            }
            public static bool GetAntennaDeployed(PartModule p)
            {
                if (GetReflectionProperty<bool>(p, "CanAnimate"))
                {
                    return (bool)p.GetType().GetProperty("AnimOpen", flags).GetValue(p, null);
                }
                else
                {
                    return true;
                }
            }
        }

        public class FerramWrapper
        {
            private static Assembly FAR = null;
            private static bool tried = false;

            public static bool available
            {
                get
                {
                    bool loaded = (FAR != null);
                    if (!loaded && !tried)
                    {
                        for (int i = 0; i < AssemblyLoader.loadedAssemblies.Count; i++)
                        {
                            var Asm = AssemblyLoader.loadedAssemblies[i];
                            if (Asm.dllName == "FerramAerospaceResearch")
                            {
                                loaded = true;
                                FAR = Asm.assembly;
                                Debug.Log("[LRTF]: FAR Detected.");
                            }
                        }
                        tried = true;
                    }
                    return loaded;
                }
            }

            public static float GetCtrlSurfYaw(PartModule p)
            {
                return (float)GetReflectionField<float>(p, "yawaxis");
            }
            public static float GetCtrlSurfPitch(PartModule p)
            {
                return (float)GetReflectionField<float>(p, "pitchaxis");
            }
            public static float GetCtrlSurfRoll(PartModule p)
            {
                return (float)GetReflectionField<float>(p, "rollaxis");
            }
            public static float GetCtrlSurfMaxDeflect(PartModule p)
            {
                return (float)GetReflectionField<float>(p, "maxdeflect");
            }
            public static float GetCtrlSurfBrakeRudder(PartModule p)
            {
                return (float)GetReflectionField<float>(p, "brakeRudder");
            }

            public static bool GetCtrlSurfIsSpoiler(PartModule p)
            {
                return (bool)GetReflectionField<bool>(p, "isSpoiler");
            }
            public static bool GetCtrlSurfIsFlap(PartModule p)
            {
                return (bool)GetReflectionField<bool>(p, "isSpoiler");
            }



            public static void SetCtrlSurfYaw(PartModule p, float value)
            {
                SetReflectionField<float>(p, "yawaxis", value);
            }
            public static void SetCtrlSurfPitch(PartModule p, float value)
            {
                SetReflectionField<float>(p, "pitchaxis", value);
            }
            public static void SetCtrlSurfRoll(PartModule p, float value)
            {
                SetReflectionField<float>(p, "rollaxis", value);
            }
            public static void SetCtrlSurfMaxDeflect(PartModule p, float value)
            {
                SetReflectionField<float>(p, "maxdeflect", value);
            }
            public static void SetCtrlSurfBrakeRudder(PartModule p, float value)
            {
                SetReflectionField<float>(p, "brakeRudder", value);
            }

            //RealChuteLite. 
            public enum DeploymentStates
            {
                NONE,
                STOWED,
                PREDEPLOYED,
                DEPLOYED,
                CUT
            }
            public enum DragCubeStates
            {
                STOWED,
                RCDEPLOYED,
                DEPLOYED,
                SEMIDEPOLOYED,
                PACKED
            }

            public static void CutChute(PartModule p)
            {
                p.GetType().GetMethod("Cut").Invoke(p, null);
            }

            public static void DeployChute(PartModule p)
            {
                float currMinPressure = GetReflectionField<float>(p, "minAirPressureToOpen");
                SetReflectionField<float>(p, "minAirPressureToOpen", 0.0f);
                p.GetType().GetMethod("ActivateRC").Invoke(p, null);
                SetReflectionField<float>(p, "minAirPressureToOpen", currMinPressure);
            }

            public static void PreDeploy(PartModule p)
            {
                SetReflectionField<DeploymentStates>(p, "DeploymentState", DeploymentStates.PREDEPLOYED);
                p.GetType().GetMethod("PreDeploy").Invoke(p, null);
            }

            public static bool IsDeployed(PartModule p)
            {
                return GetReflectionField<bool>(p, "armed") || GetReflectionProperty<bool>(p, "IsDeployed");
            }

            public static DeploymentStates GetDeploymentState(PartModule p)
            {
                return GetReflectionProperty<DeploymentStates>(p, "DeploymentState");
            }

            public static void SetDeploymentAltitude(PartModule p, float alt = 0)
            {
                SetReflectionField<float>(p, "deployAltitude", alt);
            }

            public static float GetDeployAltitude(PartModule p)
            {
                return GetReflectionField<float>(p, "deployAltitude");
            }

            public static void AssumeDragCubePosition(PartModule p, string pos)
            {
                p.GetType().GetMethod("AssumeDragCubePosition").Invoke(p, new object[] { pos });
            }
        }

        //Relfection Helpers. 
        private static T GetReflectionField<T>(PartModule p, string field_name)
        {
            return (T)p.GetType().GetField(field_name, flags).GetValue(p);
        }
        private static void SetReflectionField<T>(PartModule p, string value_name, T value)
        {
            p.GetType().GetField(value_name, flags).SetValue(p, value);
        }
        private static T GetReflectionProperty<T>(PartModule p, String property)
        {
            return (T)p.GetType().GetProperty(property, flags).GetValue(p, null);
        }

    }
}
