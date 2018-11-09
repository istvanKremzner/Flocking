using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowTheBoid : MonoBehaviour
{

    public GameObject bounds;
    public Vector3 offset;

    private List<GameObject> boids {
        get {
            return bounds.GetComponent<BoidsController>().Boids;
        }
    }

    private void Update()
    {
        Vector3 avaragePosition = GetAvaragePos();

        this.transform.position = avaragePosition - offset;

        this.transform.LookAt(avaragePosition);
    }

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
