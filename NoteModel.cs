// Assets/QuestNotebook/Scripts/NoteModel.cs
using System;
using UnityEngine;

[Serializable]
public class NoteModel
{
    public string id;
    public string text;
    public Vector3 position;
    public Vector3 rotation; // Euler angles
}