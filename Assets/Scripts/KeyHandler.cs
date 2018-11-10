using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyHandler : MonoBehaviour
{
    private const float CAMERAMOVESTEP = 2;
    private const float CAMERAROTATIONSPEED = 0.2f;
    private const float SCROLLSTEP = 100;

    public GameObject Menu;

    private void Awake()
    {
        Time.timeScale = 0.0f;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Menu.SetActive(!Menu.activeSelf);

            if (Time.timeScale == 1.0f)
                Time.timeScale = 0.0f;
            else
                Time.timeScale = 1.0f;
        }

        GetCameraPositionChange();
    }

    private void GetCameraPositionChange()
    {
        if (!Camera.main.GetComponent<FollowTheBoid>().enabled)
        {
            Vector3 moveCamera = Vector3.zero;
            float scroll = Input.GetAxis("Mouse ScrollWheel");

            if (scroll != 0)
            {
                moveCamera += new Vector3(0, 0, scroll * SCROLLSTEP);
            }

            moveCamera += GetCameraMovement();

            Camera.main.transform.position += moveCamera;

            Camera.main.transform.eulerAngles += GetCameraRotation();
        }
    }

    private Vector3 GetCameraMovement()
    {
        Vector3 moveCamera = Vector3.zero;

        if (Input.GetKey(KeyCode.W))
        {
            moveCamera += new Vector3(0, CAMERAMOVESTEP, 0);
        }
        else if (Input.GetKey(KeyCode.S))
        {
            moveCamera += new Vector3(0, -CAMERAMOVESTEP, 0);
        }

        if (Input.GetKey(KeyCode.D))
        {
            moveCamera += new Vector3(CAMERAMOVESTEP, 0, 0);
        }
        else if (Input.GetKey(KeyCode.A))
        {
            moveCamera += new Vector3(-CAMERAMOVESTEP, 0, 0);
        }

        return moveCamera;
    }

    private Vector3 GetCameraRotation()
    {
        Vector3 cameraRotation = Vector3.zero;

        if (Input.GetKey(KeyCode.UpArrow))
        {
            cameraRotation += new Vector3(-CAMERAROTATIONSPEED, 0, 0);
        }
        else if (Input.GetKey(KeyCode.DownArrow))
        {
            cameraRotation += new Vector3(CAMERAROTATIONSPEED, 0, 0);
        }

        if (Input.GetKey(KeyCode.RightArrow))
        {
            cameraRotation += new Vector3(0, CAMERAROTATIONSPEED, 0);
        }
        else if (Input.GetKey(KeyCode.LeftArrow))
        {
            cameraRotation += new Vector3(0, -CAMERAROTATIONSPEED, 0);
        }

        return cameraRotation;
    }

}
