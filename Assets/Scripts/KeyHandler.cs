using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles input.
/// </summary>
public class KeyHandler : MonoBehaviour
{
    private const float CAMERAMOVESTEP = 2;
    private const float CAMERAROTATIONSPEED = 0.2f;
    private const float SCROLLSTEP = 100;

    public GameObject Menu;
    public BoidsController controller;

    /// <summary>
    /// Freezes time for the Start Menu.
    /// </summary>
    private void Awake()
    {
        Time.timeScale = 0.0f;
    }

    /// <summary>
    /// Handles input.
    /// </summary>
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

        FollowFish();
    }

    /// <summary>
    /// Gets input for the camera and applies the offsets for the main camera.
    /// </summary>
    private void GetCameraPositionChange()
    {
        if (!Camera.main.GetComponent<FollowTheBoid>().enabled)
        {
            Vector3 moveCamera = Vector3.zero;
            FollowTheFish followFish = Camera.main.GetComponent<FollowTheFish>();

            float scroll = Input.GetAxis("Mouse ScrollWheel");

            scroll = scroll == 0 && Input.GetKey(KeyCode.KeypadPlus) ? 0.05f * controller.Scale / 10 : scroll;
            scroll = scroll == 0 && Input.GetKey(KeyCode.KeypadMinus) ? -0.05f * controller.Scale / 10 : scroll;

            if (scroll != 0)
            {
                moveCamera += new Vector3(0, 0, scroll * SCROLLSTEP);
            }

            moveCamera += GetCameraMovement();

            if (followFish.enabled)
            {
                followFish.Offset += moveCamera;

                //var rot = GetCameraRotation();
                //var fish = followFish.Fish;
                //var off = followFish.Offset;


                //followFish.RotationOffset += (fish.right + off) * rot.x;
                //followFish.RotationOffset += (fish.up + off) * rot.y;
                //followFish.RotationOffset += (fish.forward + off) * rot.z;
            }
            else
            {
                //Camera.main.transform.position +=  moveCamera;
                Camera.main.transform.position += Camera.main.transform.right * moveCamera.x;
                Camera.main.transform.position += Camera.main.transform.up * moveCamera.y;
                Camera.main.transform.position += Camera.main.transform.forward * moveCamera.z;
                Camera.main.transform.eulerAngles += GetCameraRotation();
            }
        }
    }

    /// <summary>
    /// Gets movement for the camera.
    /// </summary>
    /// <returns>The direction the camera should move to.</returns>
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

        return moveCamera * controller.Scale / 2;
    }

    /// <summary>
    /// Gets roatation for the camera.
    /// </summary>
    /// <returns>The direction the camera should rotate to.</returns>
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

        return cameraRotation * 6;
    }

    private void FollowFish()
    {
        if (!Camera.main.GetComponent<FollowTheBoid>().enabled)
        {
            if (Input.GetKeyDown(KeyCode.F))
            {
                FollowTheFish followFish = Camera.main.GetComponent<FollowTheFish>();

                if (!followFish.enabled)
                {
                    RaycastHit hit;
                    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                    if (Physics.Raycast(ray, out hit))
                    {
                        Transform objectHit = hit.transform;

                        if (hit.transform != controller.transform)
                        {
                            followFish.Fish = objectHit;
                            followFish.Offset = new Vector3(0, 0, -100);

                            followFish.enabled = true;
                        }
                    }
                }
                else
                {
                    followFish.enabled = false;
                }
            }
        }
    }

}
