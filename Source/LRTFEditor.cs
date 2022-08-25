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
        private ApplicationLauncherButton _button;
        private bool finishedShowFailedPAWs;

        public void Start()
        {
            StartCoroutine("AddToToolbar");
        }

        public void OnDestroy()
        {
            if(_button != null)
                ApplicationLauncher.Instance.RemoveModApplication(_button);
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
                    throw new Exception("LRTF: Failed to load icon texture");
                }
                _button = ApplicationLauncher.Instance.AddModApplication(
                    ShowFailedPAWs,
                    ShowFailedPAWs,
                    null,
                    null,
                    null,
                    null,
                    ApplicationLauncher.AppScenes.VAB | ApplicationLauncher.AppScenes.SPH,
                    iconTexture);
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