// Assets/QuestNotebook/Scripts/PassthroughManager.cs
using UnityEngine;

/// <summary>
/// PassthroughManager:
/// - Today: optional visual tweaks so spawned notes are readable.
/// - Future: Replace TODOs with Meta OpenXR Passthrough APIs (com.unity.xr.meta-openxr).
/// </summary>
public class PassthroughManager : MonoBehaviour
{
    [Range(0f, 1f)] public float sceneDimmer = 0.2f;
    private Material _dimmerMat;

    private void Start()
    {
        // Simple dimmer quad behind UI to improve readability.
        var dimmer = GameObject.CreatePrimitive(PrimitiveType.Quad);
        dimmer.name = "BackgroundDimmer";
        Destroy(dimmer.GetComponent<Collider>());
        dimmer.transform.position = new Vector3(0, 0, 1.5f);
        dimmer.transform.localScale = new Vector3(5f, 3f, 1f);

        _dimmerMat = new Material(Shader.Find("UI/Default"));
        var c = new Color(0, 0, 0, sceneDimmer);
        _dimmerMat.color = c;
        var r = dimmer.GetComponent<MeshRenderer>();
        r.material = _dimmerMat;

        // TODO: When using Meta OpenXR:
        // - Enable Passthrough Feature in Project Settings → OpenXR → Meta.
        // - Here, create and enable a Passthrough layer via Meta API (e.g., XrPassthroughFB).
    }

    private void OnValidate()
    {
        if (_dimmerMat != null)
        {
            var c = _dimmerMat.color;
            c.a = sceneDimmer;
            _dimmerMat.color = c;
        }
    }
}
