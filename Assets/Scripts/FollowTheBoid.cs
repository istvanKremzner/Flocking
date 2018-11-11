using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Sets the camera to the avarage position of the Boids with offset.
/// </summary>
public class FollowTheBoid : MonoBehaviour
{
    public GameObject bounds;
    public Vector3 offset;

    private List<GameObject> boids
    {
        get
        {
            return bounds.GetComponent<BoidsController>().Boids;
        }
    }

    /// <summary>
    /// If the BoidController's GameObject is active, sets the camera's position to the avarage position of the Boids with the applied offsets.
    /// </summary>
    private void Update()
    {
        if (bounds.activeSelf)
        {
            Vector3 avaragePosition = GetAvaragePos();

            this.transform.position = avaragePosition - offset;

            this.transform.LookAt(avaragePosition);
        }
    }

    /// <summary>
    /// Gets the avarage position of the Boids.
    /// </summary>
    /// <returns>The avarage position.</returns>
    private Vector3 GetAvaragePos()
    {
        Vector3 sum = Vector3.zero;

        foreach (GameObject actBoid in boids)
            sum += actBoid.transform.position;

        if (boids.Count > 0)
            return sum / boids.Count;

        return this.transform.position + offset;
    }

}
