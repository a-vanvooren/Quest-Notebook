// Assets/QuestNotebook/Scripts/NotebookWindow.cs
using System;
using UnityEngine;
using TMPro;

/// <summary>
/// NotebookWindow: attach this to your notebook prefab root (world-space Canvas).
/// It expects:
/// - a child TMP_InputField (or InputField) assigned to 'textField'
/// - optional visible handles for scale/rotate (not implemented here)
/// 
/// Responsibilities:
/// - accept Initialize(...) to set initial content and id
/// - Save / Load calls (delegates to NoteStorageService and AnchorManager)
/// - AttachToAnchor: persist the transform via AnchorManager
/// </summary>
public class NotebookWindow : MonoBehaviour
{
    [Header("UI")]
    public TMP_InputField textField;

    [Header("Meta")]
    public string noteId; // unique id (generated on create)

    [Header("Behaviour")]
    public bool autoSaveOnEditEnd = true;

    private NoteStorageService storage;
    private AnchorManager anchorManager;

    private void Awake()
    {
        storage = FindObjectOfType<NoteStorageService>();
        anchorManager = FindObjectOfType<AnchorManager>();
    }

    public void Initialize(string id = null, string initialText = "")
    {
        noteId = string.IsNullOrEmpty(id) ? Guid.NewGuid().ToString() : id;
        if (textField != null)
        {
            textField.text = initialText;
            textField.onEndEdit.AddListener(OnEndEdit);
        }
    }

    private void OnDestroy()
    {
        if (textField != null)
            textField.onEndEdit.RemoveListener(OnEndEdit);
    }

    private void OnEndEdit(string newText)
    {
        if (autoSaveOnEditEnd)
            Save();
    }

    /// <summary>
    /// Attach to an anchor (anchor creation handled by AnchorManager).
    /// This requests an anchor at the current transform and saves note metadata.
    /// </summary>
    public void AttachToAnchor()
    {
        if (anchorManager == null)
        {
            Debug.LogWarning("[NotebookWindow] No AnchorManager found. Saving transform locally.");
            Save();
            return;
        }

        anchorManager.CreateAnchorAt(transform, noteId, (success) =>
        {
            if (success)
            {
                Debug.Log($"[NotebookWindow] Anchor created for note {noteId}");
                Save();
            }
            else
            {
                Debug.LogWarning($"[NotebookWindow] Anchor creation failed for note {noteId}. Will save local transform.");
                Save();
            }
        });
    }

    /// <summary>
    /// Save content + transform via NoteStorageService (and anchor metadata via AnchorManager if available).
    /// </summary>
    public void Save()
    {
        if (storage == null)
        {
            Debug.LogWarning("[NotebookWindow] No NoteStorageService found. Skipping save.");
            return;
        }

        var model = new NoteModel()
        {
            id = noteId,
            text = (textField != null) ? textField.text : "",
            position = transform.position,
            rotation = transform.rotation.eulerAngles
        };

        storage.SaveNote(model);
    }

    /// <summary>
    /// Load note contents into this window.
    /// </summary>
    public void Load()
    {
        if (storage == null) return;
        var loaded = storage.LoadNote(noteId);
        if (loaded != null)
        {
            if (textField != null) textField.text = loaded.text;
            transform.position = loaded.position;
            transform.rotation = Quaternion.Euler(loaded.rotation);
        }
    }
}