using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class DebugEntry
{
    public string text = "";
    public Color color = Color.white;
}

public class DebugWindow : MonoBehaviour {

    private Vector2 scrollPosition;
    private ArrayList entries = new ArrayList();
    private Rect window = new Rect(5, 180, 300, 150);

    public bool ShowDebugger = true;

    void OnGUI()
    {
        if (ShowDebugger)
        {
            window = GUI.Window(1, window, OnDebugWindow, "Debug");
        }
    }

    void OnDebugWindow(int id)
    {
        // Begin a scroll view. All rects are calculated automatically - 
        // it will use up any available screen space and make sure contents flow correctly.
        // This is kept small with the last two parameters to force scrollbars to appear.
        scrollPosition = GUILayout.BeginScrollView(scrollPosition);

        foreach (DebugEntry entry in entries)
        {
            GUILayout.BeginHorizontal();

            GUI.contentColor = entry.color;
            GUILayout.Label(entry.text);
            GUI.contentColor = Color.white; // default

            GUILayout.EndHorizontal();
            GUILayout.Space(3);

        }
        // End the scrollview we began above.
        GUILayout.EndScrollView();
        GUI.DragWindow();
    }

    public void Text(string str)
    {
        DebugText(str, Color.white);
    }

    void DebugText(string str, Color color)
    {
        Debug.Log(str);

        var entry = new DebugEntry();
        entry.text = str;
        entry.color = color;

        entries.Add(entry);

        if (entries.Count > 50)
            entries.RemoveAt(0);

        scrollPosition.y = 1000000;
    }
}
