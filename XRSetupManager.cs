// Assets/QuestNotebook/Scripts/XRSetupManager.cs
using UnityEngine;

public class XRSetupManager : MonoBehaviour
{
    [Tooltip("If no camera is tagged MainCamera, this prefab will be spawned.")]
    public GameObject fallbackRigPrefab;

    private void Start()
    {
        var mainCam = Camera.main;
        if (mainCam == null)
        {
            // Create a very simple rig with a Camera so Editor tests work.
            var rig = new GameObject("XR_FallbackRig");
            var camGO = new GameObject("Main Camera");
            camGO.tag = "MainCamera";
            camGO.transform.SetParent(rig.transform, false);
            var cam = camGO.AddComponent<Camera>();
            cam.nearClipPlane = 0.01f;
        }

        // NOTE: Real OpenXR setup is done in Project Settings (XR Plug-in Management).
        // When Meta OpenXR is enabled, controller/hand tracking will be available automatically.
    }
}
