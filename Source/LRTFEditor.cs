using System;
using System.Reflection;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using KSP.UI.Screens;
using TestFlight.LRTF;

namespace LRTF
{
    using MonoBehavior = UnityEngine.MonoBehaviour;

    [KSPAddon(KSPAddon.Startup.EditorAny, false)]
    public class LRTFEditor : MonoBehavior
    {
        private ApplicationLauncherButton appLauncherButton;
        private bool finishedShowFailedPAWs;

        public void Start()
        {
            StartCoroutine("AddToToolbar");
        }


        IEnumerator AddToToolbar()
        {
            while (!ApplicationLauncher.Ready)
            {
                yield return null;
            }
            try
            {
                // Load the icon for the button
                Texture iconTexture = GameDatabase.Instance.GetTexture("TestFlight/Resources/AppLauncherIcon", false);
                if (iconTexture == null)
                {
                    throw new Exception("TestFlight MasterStatusDisplay: Failed to load icon texture");
                }
                appLauncherButton = ApplicationLauncher.Instance.AddModApplication(
                    ShowFailedPAWs,
                    ShowFailedPAWs,
                    null,
                    null,
                    null,
                    null,
                    ApplicationLauncher.AppScenes.VAB | ApplicationLauncher.AppScenes.SPH,
                    iconTexture);
                //ApplicationLauncher.Instance.AddOnHideCallback(HideButton);
                //ApplicationLauncher.Instance.AddOnRepositionCallback(RepostionWindow);
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        internal void ShowFailedPAWs()
        {
            finishedShowFailedPAWs = false;
            foreach (Part part in EditorLogic.fetch.ship.parts)
            {
                bool showPAW = false;
                foreach(LRTFFailureBase module in part.Modules.GetModules<LRTFFailureBase>())
                {
                    if (module.failed || module.partialFailed)
                    {
                        showPAW = true;
                    }
                }
                if (showPAW)
                {
                    UIPartActionController.Instance.SpawnPartActionWindow(part);
                    part.PartActionWindow.OnPin(true);
                    StartCoroutine("UnPin", part);
                }
            }
            finishedShowFailedPAWs = true;
        }
        IEnumerator UnPin(Part part)
        {
            while (!finishedShowFailedPAWs)
            {
                yield return null;
            }
            part.PartActionWindow.OnPin(false);
        }
    }
}