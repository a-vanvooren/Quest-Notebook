// Assets/QuestNotebook/Scripts/NoteStorageService.cs
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

/// <summary>
/// NoteStorageService: save/load note content locally (JSON files).
/// Each note saved as {persistentPath}/notes/{id}.json
/// This is intentionally simple; a production version should use a safe DB or cloud backend.
/// </summary>
public class NoteStorageService : MonoBehaviour
{
    private string notesFolder;

    private void Awake()
    {
        notesFolder = Path.Combine(Application.persistentDataPath, "notes");
        if (!Directory.Exists(notesFolder)) Directory.CreateDirectory(notesFolder);
    }

    public void SaveNote(NoteModel model)
    {
        try
        {
            var path = Path.Combine(notesFolder, model.id + ".json");
            var json = JsonUtility.ToJson(model, true);
            File.WriteAllText(path, json);
            Debug.Log($"[NoteStorageService] Saved note {model.id} -> {path}");
        }
        catch (Exception e)
        {
            Debug.LogError("[NoteStorageService] SaveNote failed: " + e);
        }
    }

    public NoteModel LoadNote(string id)
    {
        var path = Path.Combine(notesFolder, id + ".json");
        if (!File.Exists(path)) return null;
        try
        {
            var json = File.ReadAllText(path);
            var model = JsonUtility.FromJson<NoteModel>(json);
            return model;
        }
        catch (Exception e)
        {
            Debug.LogError("[NoteStorageService] LoadNote failed: " + e);
            return null;
        }
    }

    public List<NoteModel> LoadAllNotes()
    {
        var list = new List<NoteModel>();
        try
        {
            var files = Directory.GetFiles(notesFolder, "*.json");
            foreach (var f in files)
            {
                try
                {
                    var json = File.ReadAllText(f);
                    var m = JsonUtility.FromJson<NoteModel>(json);
                    list.Add(m);
                }
                catch { /* skip corrupted file */ }
            }
        }
        catch (Exception e)
        {
            Debug.LogError("[NoteStorageService] LoadAllNotes failed: " + e);
        }
        return list;
    }
}