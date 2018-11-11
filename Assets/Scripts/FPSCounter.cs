using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Counts Frames Per Second and writes it to the screen.
/// </summary>
public class FPSCounter : MonoBehaviour
{

    float deltaTime = 0.0f;

    void Update()
    {
        deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;
    }

    void OnGUI()
    {
        int w = Screen.width, h = Screen.height;

        GUIStyle style = new GUIStyle();

        Rect rect = new Rect(0, 0, w, h * 2 / 100);
        style.alignment = TextAnchor.UpperRight;
        style.fontSize = h * 4 / 100;
        style.normal.textColor = Color.white;
        float msec = deltaTime * 1000.0f;
        float fps = 1.0f / deltaTime;
        //string text = string.Format("{0:0.0} ms ({1:0.} fps)", msec, fps);
        string text = string.Format("{0:0} fps", fps);
        GUI.Label(rect, text, style);
    }
}
