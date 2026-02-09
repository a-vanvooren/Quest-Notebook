// Assets/QuestNotebook/Scripts/NotebookMenu.cs
using System.Collections.Generic;
using UnityEngine;

public class NotebookMenu : MonoBehaviour
{
    [Header("References")]
    public GameObject notebookPrefab;
    public Transform attachNearCamera; // where to show menu

    private List<NotebookWindow> spawned = new List<NotebookWindow>();
    private float spawnDistance = 1.2f;

    private void Start()
    {
        if (notebookPrefab == null)
        {
            // Try to pick it off PlacementController
            var pc = FindObjectOfType<PlacementController>();
            if (pc != null) notebookPrefab = pc.notebookPrefab;
        }

        if (attachNearCamera == null && Camera.main != null) attachNearCamera = Camera.main.transform;
    }

    private void Update()
    {
        // Quick dev shortcuts:
        // N = new note in front, L = load all notes from storage
        if (Input.GetKeyDown(KeyCode.N)) CreateNoteInFront("");
        if (Input.GetKeyDown(KeyCode.L)) LoadAllExistingNotes();
    }

    public NotebookWindow CreateNoteInFront(string initialText)
    {
        if (notebookPrefab == null || attachNearCamera == null) return null;
        Vector3 pos = attachNearCamera.position + attachNearCamera.forward * spawnDistance;
        Quaternion rot = Quaternion.LookRotation(attachNearCamera.forward, Vector3.up);
        var go = Instantiate(notebookPrefab, pos, rot);
        var nw = go.GetComponent<NotebookWindow>();
        if (!nw) nw = go.AddComponent<NotebookWindow>();
        nw.Initialize(null, initialText);
        nw.AttachToAnchor();
        spawned.Add(nw);
        return nw;
    }

    public void LoadAllExistingNotes()
    {
        var storage = FindObjectOfType<NoteStorageService>();
        if (storage == null) return;

        var all = storage.LoadAllNotes();
        foreach (var m in all)
        {
            var win = CreateNoteInFront(m.text);
            if (win == null) continue;
            win.noteId = m.id;

            // Try resolve anchor (local fallback)
            var anchor = FindObjectOfType<AnchorManager>();
            if (anchor != null && anchor.TryResolveAnchor(m.id, out var pos, out var rot))
            {
                win.transform.position = pos;
                win.transform.rotation = rot;
            }
            else
            {
                // Place near camera as fallback
            }
        }
    }
}
