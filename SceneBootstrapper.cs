// Assets/QuestNotebook/Scripts/SceneBootstrapper.cs
using UnityEngine;
using UnityEngine.EventSystems;

public class SceneBootstrapper : MonoBehaviour
{
    [Header("Prefabs (optional)")]
    public GameObject notebookPrefab;

    private void Awake()
    {
        EnsureEventSystem();
        EnsureComponent<NoteStorageService>("NoteStorageService");
        EnsureComponent<AnchorManager>("AnchorManager");
        var pd = EnsureComponent<PlaneDetector>("PlaneDetector");
        var pc = EnsureComponent<PlacementController>("PlacementController");
        if (pc.notebookPrefab == null && notebookPrefab != null) pc.notebookPrefab = notebookPrefab;

        EnsureComponent<XRSetupManager>("XRSetupManager");
        EnsureComponent<PassthroughManager>("PassthroughManager");
        EnsureComponent<NotebookMenu>("NotebookMenu");
    }

    private T EnsureComponent<T>(string goName) where T : Component
    {
        var go = GameObject.Find(goName);
        if (go == null) go = new GameObject(goName);
        var comp = go.GetComponent<T>();
        if (comp == null) comp = go.AddComponent<T>();
        return comp;
    }

    private void EnsureEventSystem()
    {
        if (FindObjectOfType<EventSystem>() != null) return;
        var go = new GameObject("EventSystem");
        go.AddComponent<EventSystem>();
        go.AddComponent<StandaloneInputModule>();
    }
}
