using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

/// <summary>
/// Controlls and instantiates the boids.
/// </summary>
public class BoidsController : MonoBehaviour
{
    private static Vector3 TankRates = new Vector3(2, 1, 1);
    private const float SPACEPERFISH = 5;

    private BoxCollider bounds;
    public GameObject prefab;

    private List<GameObject> avoids;
    private List<GameObject> boids;

    public int InitNumber;

    [Space]
    [Header("Values")]
    //boid control
    public float maxSpeed;
    public float friendRadius;
    public float crowdRadius;
    public float avoidRadius;
    public float coheseRadius;
    [Range(0,1)]
    public float maxNoise;

    [Space]
    [Header("Options")]
    public bool optionFriend;
    public bool optionCrowd;
    public bool optionAvoid;
    public bool optionNoise;
    public bool optionCohese;

    [Space]
    [Header("Debug options")]
    public bool showFriendRadius;

    public float GetMaxSpeed
    {
        get
        {
            return maxSpeed * prefab.transform.localScale.x;
        }
    }

    public BoxCollider Bounds
    {
        get
        {
            return bounds;
        }
    }

    public List<GameObject> Boids
    {
        get
        {
            return boids;
        }
    }

    public List<GameObject> Avoids
    {
        get
        {
            return avoids;
        }
    }

    /// <summary>
    /// Inits the variables, gets the Obstacles and instantiates the Boids.
    /// </summary>
    public void Init()
    {
        bounds = this.GetComponent<BoxCollider>();
        SetBoundsSize();

        avoids = new List<GameObject>();
        boids = new List<GameObject>();

        GetObstacles();

        float minPerc = 0.1f;
        for (int i = 0; i < InitNumber; i++)
        {
            GameObject actGameObj = Instantiate(prefab, this.transform);
            actGameObj.transform.localPosition = new Vector3(
               (Random.Range(minPerc, 1 - minPerc) * bounds.size.x) - bounds.size.x / 2,
                (Random.Range(minPerc, 1 - minPerc) * bounds.size.y) - bounds.size.y / 2,
               (Random.Range(minPerc, 1 - minPerc) * bounds.size.z) - bounds.size.z / 2);

            actGameObj.AddComponent<Boid>();
            actGameObj.GetComponent<Boid>().SetController = this;

            boids.Add(actGameObj);
        }
    }

    /// <summary>
    /// Gets the obstacles.
    /// The children of this GameObject but aren't Boids.
    /// </summary>
    private void GetObstacles()
    {
        foreach (Transform actTransform in this.transform)
            if (!actTransform.GetComponent<Boid>())
                avoids.Add(actTransform.gameObject);
    }

    /// <summary>
    /// Sets the aquarium size according to the number of fishes.
    /// </summary>
    private void SetBoundsSize()
    {
        float volume = SPACEPERFISH * InitNumber;
        Vector3 volume3D = TankRates * volume;

        Bounds.size = volume3D;

        prefab.transform.localScale = volume / 50 * Vector3.one;
    }

}
