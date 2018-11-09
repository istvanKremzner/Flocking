using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoidsController : MonoBehaviour
{
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

    private void Awake()
    {
        bounds = this.GetComponent<BoxCollider>();

        avoids = new List<GameObject>();
        boids = new List<GameObject>();

        ////testing
        //MakeCubes();

        GetAvoids();

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

    private void GetAvoids()
    {
        foreach (Transform actTransform in this.transform)
            if (!actTransform.GetComponent<Boid>())
                avoids.Add(actTransform.gameObject);
    }

    //Just for testing
    private void MakeCubes()
    {
        for (int z = 0; z < bounds.size.z; z++)
        {
            for (int y = 0; y < bounds.size.y; y++)
            {
                GameObject current = Instantiate(GameObject.CreatePrimitive(PrimitiveType.Cube), this.transform);

                current.transform.localPosition = new Vector3(0, y - bounds.size.y / 2, z - bounds.size.z / 2);
            }
        }
    }
}
