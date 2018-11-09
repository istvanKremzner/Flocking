using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyHandler : MonoBehaviour
{
    public GameObject Menu;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Menu.SetActive(!Menu.activeSelf);

            if(Time.timeScale == 1.0f)
                Time.timeScale = 0.0f;
            else
                Time.timeScale = 1.0f;
        }
    }
}
