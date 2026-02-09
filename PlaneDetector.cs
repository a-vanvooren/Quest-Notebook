// Assets/QuestNotebook/Scripts/PlaneDetector.cs
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Simple PlaneDetector using Physics.Raycast as default.
/// For Quest 2 production you can replace parts of this with Meta OpenXR scene/plane APIs
/// or ARFoundation plane queries. This class keeps the rest of the project decoupled from
/// the plane detection implementation.
/// 
/// Usage:
/// - Call TryGetPlacementPose(ray, out pose, out normal, out hit) to test where to place a notebook window.
/// </summary>
public class PlaneDetector : MonoBehaviour
{
    [Tooltip("Layers considered 'placeable' (walls, tables). Use a specific layer for real-world colliders or spatial proxies.")]
    public LayerMask placementLayerMask = ~0; // default: everything

    [Tooltip("Maximum distance for the placement ray")]
    public float maxRayDistance = 10f;

    /// <summary>
    /// Try to find a placement pose given an input Ray (controller ray or camera forward).
    /// Returns true if a hit on the placementLayerMask occurs.
    /// </summary>
    public bool TryGetPlacementPose(Ray ray, out Pose pose, out Vector3 hitNormal, out RaycastHit hitInfo)
    {
        hitInfo = new RaycastHit();
        pose = new Pose();
        hitNormal = Vector3.up;

        if (Physics.Raycast(ray, out hitInfo, maxRayDistance, placementLayerMask, QueryTriggerInteraction.Ignore))
        {
            pose = new Pose(hitInfo.point, Quaternion.LookRotation(Vector3.ProjectOnPlane(-Camera.main.transform.forward, hitInfo.normal), hitInfo.normal));
            hitNormal = hitInfo.normal;
            return true;
        }

        // FUTURE: Integrate platform plane/scene queries here (Meta OpenXR / ARFoundation)
        return false;
    }
}