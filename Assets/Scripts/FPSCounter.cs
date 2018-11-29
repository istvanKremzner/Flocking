using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Counts Frames Per Second and writes it to the screen.
/// </summary>
public class FPSCounter : MonoBehaviour
{
    public BoidsController controller;

    float deltaTime = 0.0f;
    float lastThread = 0.0f;
    float deltaThread = 0.0f;

    void Update()
    {
        deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;

        if (controller != null && controller.Boids != null)
        {
            float currentThread = controller.GetTasksRun;
            deltaThread = (currentThread - lastThread) / controller.Boids.Count;
            lastThread = currentThread;
        }
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
        float fpsParallel = deltaThread;
        string text = string.Format("{0:0} fps", fpsParallel);
        GUI.Label(rect, text, style);
    }
}
