// Assets/QuestNotebook/Scripts/AnchorManager.cs
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// AnchorManager: lightweight anchor shim.
/// - Current implementation persists transform data locally (PlayerPrefs) as a fallback.
/// - It exposes CreateAnchorAt(transform, id, callback) which saves transform data.
/// - Replace Create / Resolve logic with Meta Spatial Anchors or XR Anchor Subsystem for production.
/// 
/// Note: local transform persistence is NOT a robust world-anchoring solution across device space changes.
/// Use Meta Spatial Anchors or cloud anchors for production persistence and multi-user sharing.
/// </summary>
public class AnchorManager : MonoBehaviour
{
    private const string KEY_PREFIX = "QuestNotebook_Anchor_";

    /// <summary>
    /// Create an "anchor" at transform. Calls callback(true) when done (success).
    /// Current implementation is synchronous and returns true.
    /// </summary>
    public void CreateAnchorAt(Transform t, string anchorId, Action<bool> callback)
    {
        try
        {
            var dto = new AnchorDto()
            {
                position = t.position,
                rotation = t.rotation.eulerAngles,
                timeSavedUtc = DateTime.UtcNow.ToString("o")
            };

            var json = JsonUtility.ToJson(dto);
            PlayerPrefs.SetString(KEY_PREFIX + anchorId, json);
            PlayerPrefs.Save();

            callback?.Invoke(true);
        }
        catch (Exception e)
        {
            Debug.LogError("[AnchorManager] CreateAnchorAt failed: " + e);
            callback?.Invoke(false);
        }
    }

    /// <summary>
    /// Try to resolve an anchor by id. Returns true if found and outputs position/rotation.
    /// </summary>
    public bool TryResolveAnchor(string anchorId, out Vector3 position, out Quaternion rotation)
    {
        position = Vector3.zero;
        rotation = Quaternion.identity;
        var key = KEY_PREFIX + anchorId;
        if (!PlayerPrefs.HasKey(key)) return false;

        try
        {
            var json = PlayerPrefs.GetString(key);
            var dto = JsonUtility.FromJson<AnchorDto>(json);
            position = dto.position;
            rotation = Quaternion.Euler(dto.rotation);
            return true;
        }
        catch (Exception e)
        {
            Debug.LogError("[AnchorManager] TryResolveAnchor failed: " + e);
            return false;
        }
    }

    [Serializable]
    private class AnchorDto
    {
        public Vector3 position;
        public Vector3 rotation;
        public string timeSavedUtc;
    }

    // TODO: Replace local PlayerPrefs storage with Meta Spatial Anchors / ARFoundation XRAnchor subsystem.
    // Example: call the Meta Spatial SDK to create a cloud anchor and map anchorId -> cloudAnchorUuid.
}
