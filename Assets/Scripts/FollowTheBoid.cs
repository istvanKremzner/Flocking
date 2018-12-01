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

    private BoidsController controller;

    private List<Boid> boids
    {
        get
        {
            return controller.Boids;
        }
    }

    private void Start()
    {
        controller = bounds.GetComponent<BoidsController>();
    }

    /// <summary>
    /// If the BoidController's GameObject is active, sets the camera's position to the avarage position of the Boids with the applied offsets.
    /// </summary>
    private void Update()
    {
        if (bounds.activeSelf)
        {
            Vector3 avaragePosition = GetAvaragePos();

            if (avaragePosition != Vector3.zero)
            {
                this.transform.position = avaragePosition - (offset * controller.Scale / 100);

                //this.transform.LookAt(avaragePosition);
            }
        }
    }

    /// <summary>
    /// Gets the avarage position of the Boids.
    /// </summary>
    /// <returns>The avarage position.</returns>
    private Vector3 GetAvaragePos()
    {
        Vector3 sum = Vector3.zero;

        if (boids != null)
        {
            foreach (Boid actBoid in boids)
                sum += actBoid.Position;

            if (boids.Count > 0)
                return sum / boids.Count;
        }

        return sum;
    }

}
