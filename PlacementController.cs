// Assets/QuestNotebook/Scripts/PlacementController.cs
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// PlacementController:
/// - Casts a ray from controllerTransform (if set) or from Camera.main forward
/// - On input (controller trigger or left mouse click) spawns the NotebookWindow prefab at the hit pose
/// - Calls AttachToAnchor on the created window to persist it
/// 
/// For XR controller input you can call TryGetXRTriggerPressed() or hook this up to XR Interaction Toolkit actions.
/// This script provides a simple polling-based fallback so the flows work in Editor for prototyping.
/// </summary>
public class PlacementController : MonoBehaviour
{
    [Tooltip("Assign the notebook prefab (a world-space canvas prefab with NotebookWindow component).")]
    public GameObject notebookPrefab;

    [Tooltip("Optional transform representing controller ray origin. If null, Camera.main is used.")]
    public Transform controllerTransform;

    [Tooltip("Maximum distance for placement raycast")]
    public float maxDistance = 10f;

    [Tooltip("Layer mask for placement")]
    public LayerMask placementLayers = ~0;

    private PlaneDetector planeDetector;

    private void Awake()
    {
        planeDetector = FindObjectOfType<PlaneDetector>();
        if (planeDetector == null)
        {
            // create one on-the-fly
            planeDetector = gameObject.AddComponent<PlaneDetector>();
            planeDetector.placementLayerMask = placementLayers;
        }
    }

    private void Update()
    {
        // Input:
        // - Editor: left mouse button -> place
        // - On device: you should wire XR trigger to call PlaceAtRay() directly or rely on an action to set a bool.
        if (Application.isEditor)
        {
            if (Input.GetMouseButtonDown(0))
            {
                var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                TryPlaceFromRay(ray);
            }
        }
        else
        {
            // Simple controller/camera center placement (use with a developer build)
            if (Input.GetButtonDown("Fire1") || TryGetXRTriggerPressed())
            {
                var origin = (controllerTransform != null) ? controllerTransform.position : Camera.main.transform.position;
                var forward = (controllerTransform != null) ? controllerTransform.forward : Camera.main.transform.forward;
                var r = new Ray(origin, forward);
                TryPlaceFromRay(r);
            }
        }
    }

    private bool TryGetXRTriggerPressed()
    {
        // This is a small, conservative fallback using the old Input system.
        // For production, use XR Interaction Toolkit or the new Input System actions mapped to controller triggers.
        // We'll return false here — developers should wire XR callbacks manually if needed.
        return false;
    }

    private void TryPlaceFromRay(Ray r)
    {
        if (planeDetector.TryGetPlacementPose(r, out Pose pose, out Vector3 normal, out RaycastHit hit))
        {
            SpawnNotebookAt(pose);
        }
        else
        {
            Debug.Log("[PlacementController] No placement surface found at ray");
        }
    }

    private void SpawnNotebookAt(Pose pose)
    {
        if (notebookPrefab == null)
        {
            Debug.LogError("[PlacementController] notebookPrefab not assigned");
            return;
        }

        var go = Instantiate(notebookPrefab, pose.position, pose.rotation);
        var nw = go.GetComponent<NotebookWindow>();
        if (nw == null) nw = go.AddComponent<NotebookWindow>();

        nw.Initialize(null, ""); // new note
        nw.AttachToAnchor();
    }
}