using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowTheFish : MonoBehaviour
{

    private Transform fish;
    private Vector3 offset;
    private Vector3 rotationOffset;

    public Transform Fish
    {
        get { return fish; }
        set
        {
            fish = value;
        }
    }

    public Vector3 Offset
    {
        get
        {
            return offset;
        }

        set
        {
            offset = value;
        }
    }

    public Vector3 RotationOffset
    {
        get
        {
            return rotationOffset;
        }

        set
        {
            rotationOffset = value;
        }
    }

    /// <summary>
    /// Applies the offset for the camera.
    /// </summary>
    void Update()
    {
        Camera.main.transform.position = fish.transform.position + offset;
        //Camera.main.transform.eulerAngles = rotationOffset;
    }
}
