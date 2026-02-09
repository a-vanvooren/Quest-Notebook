// Assets/QuestNotebook/Scripts/VirtualKeyboardHandler.cs
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class VirtualKeyboardHandler : MonoBehaviour
{
    [Header("Targets")]
    public NotebookWindow notebookWindow;

    [Header("Layout")]
    public Vector2 keySize = new Vector2(0.04f, 0.04f);
    public float keySpacing = 0.005f;
    public int keysPerRow = 10;

    private RectTransform kbRoot;
    private readonly string keys = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";

    private void Start()
    {
        if (notebookWindow == null) notebookWindow = GetComponentInParent<NotebookWindow>();
        BuildKeyboard();
        Hide();
    }

    public void Show() { if (kbRoot != null) kbRoot.gameObject.SetActive(true); }
    public void Hide() { if (kbRoot != null) kbRoot.gameObject.SetActive(false); }

    private void BuildKeyboard()
    {
        // Create a world-space canvas under the notebook root
        var canvasGO = new GameObject("VirtualKeyboard", typeof(RectTransform), typeof(Canvas), typeof(GraphicRaycaster));
        canvasGO.transform.SetParent(transform, false);
        var canvas = canvasGO.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.WorldSpace;
        kbRoot = canvasGO.GetComponent<RectTransform>();
        kbRoot.sizeDelta = new Vector2((keySize.x + keySpacing) * keysPerRow, (keySize.y + keySpacing) * 4);
        kbRoot.localPosition = new Vector3(0, -0.22f, 0.01f); // below the note
        kbRoot.localScale = Vector3.one * 1.0f;

        int col = 0, row = 0;
        foreach (char c in keys)
        {
            CreateKey(kbRoot, c.ToString(), row, col);
            col++;
            if (col >= keysPerRow) { col = 0; row++; }
        }
        CreateKey(kbRoot, "SPACE", row, 0, 5);
        CreateKey(kbRoot, "←", row, 6, 2); // backspace
        CreateKey(kbRoot, "OK", row, 8, 2);
    }

    private void CreateKey(RectTransform parent, string label, int row, int col, int span = 1)
    {
        var btnGO = new GameObject("Key_" + label, typeof(RectTransform), typeof(Image), typeof(Button));
        var rt = btnGO.GetComponent<RectTransform>();
        btnGO.transform.SetParent(parent, false);
        var w = keySize.x * span + keySpacing * (span - 1);
        rt.sizeDelta = new Vector2(w, keySize.y);
        rt.anchoredPosition = new Vector2(col * (keySize.x + keySpacing) + w/2f, -row * (keySize.y + keySpacing) - keySize.y/2f);

        var txtGO = new GameObject("Text", typeof(RectTransform), typeof(TextMeshProUGUI));
        txtGO.transform.SetParent(btnGO.transform, false);
        var txtrt = txtGO.GetComponent<RectTransform>();
        txtrt.anchorMin = Vector2.zero; txtrt.anchorMax = Vector2.one; txtrt.offsetMin = Vector2.zero; txtrt.offsetMax = Vector2.zero;
        var tmp = txtGO.GetComponent<TextMeshProUGUI>();
        tmp.text = label; tmp.alignment = TextAlignmentOptions.Center;

        var button = btnGO.GetComponent<Button>();
        button.onClick.AddListener(() => OnKeyPress(label));
    }

    private void OnKeyPress(string label)
    {
        if (notebookWindow == null || notebookWindow.textField == null) return;
        var field = notebookWindow.textField;

        if (label == "SPACE") field.text += " ";
        else if (label == "←")
        {
            if (field.text.Length > 0)
                field.text = field.text.Substring(0, field.text.Length - 1);
        }
        else if (label == "OK")
        {
            Hide();
            notebookWindow.Save();
        }
        else
        {
            field.text += label;
        }
        field.caretPosition = field.text.Length;
    }

    // Call these from UI events:
    public void BeginEditing() => Show();
    public void EndEditing() { Hide(); notebookWindow?.Save(); }
}
