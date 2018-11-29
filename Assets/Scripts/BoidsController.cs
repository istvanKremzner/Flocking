﻿using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

/// <summary>
/// Controlls and instantiates the boids.
/// </summary>
public class BoidsController : MonoBehaviour
{
    private static Vector3 TankRates = new Vector3(3, 1.5f, 1);
    private const float SPACEPERFISH = 20;

    private BoxCollider bounds;
    public GameObject prefab;

    private List<Bounds> avoids;
    private List<GameObject> boidObjects;
    private List<Boid> boids;

    public int InitNumber;

    [Space]
    [Header("Values")]
    //boid control
    public float maxSpeed;
    public float friendRadius;
    public float crowdRadius;
    public float avoidRadius;
    public float coheseRadius;
    [Range(0, 1)]
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

    private float realMaxSpeed;
    public float GetMaxSpeed
    {
        get
        {
            return realMaxSpeed;
            //return maxSpeed/** prefab.transform.localScale.x*/;
        }
    }

    [SerializeField]
    private float realFriendRadius;
    public float GetFriendRadius { get { return realFriendRadius; } }
    [SerializeField]
    private float realCrowdRadius;
    public float GetCrowdRadius { get { return realCrowdRadius; } }
    [SerializeField]
    private float realAvoidRadius;
    public float GetAvoidRadius { get { return realAvoidRadius; } }
    [SerializeField]
    private float realCoheseRadius;
    public float GetCoheseRadius { get { return realCoheseRadius; } }

    public BoxCollider Bounds
    {
        get
        {
            return bounds;
        }
    }

    public List<Boid> Boids
    {
        get
        {
            return boids;
        }
    }

    public List<Bounds> Avoids
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

        avoids = new List<Bounds>();
        boids = new List<Boid>();
        boidObjects = new List<GameObject>();

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
            actGameObj.GetComponent<Boid>().SetController(this, actGameObj.transform.localPosition);

            boidObjects.Add(actGameObj);
            boids.Add(actGameObj.GetComponent<Boid>());
        }

        ParallelInit();

        realMaxSpeed = maxSpeed * prefab.transform.localScale.x;
        inited = true;
    }

    /// <summary>
    /// Gets the obstacles.
    /// The children of this GameObject but aren't Boids.
    /// </summary>
    private void GetObstacles()
    {
        foreach (Transform actTransform in this.transform)
            if (!actTransform.GetComponent<Boid>())
                if (actTransform.gameObject.GetComponent<BoxCollider>())
                    avoids.Add(actTransform.gameObject.GetComponent<BoxCollider>().bounds);
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
        float scale = prefab.transform.localScale.x;

        realFriendRadius = friendRadius * scale / 2;
        realCoheseRadius = coheseRadius * scale / 4;
        realCrowdRadius = crowdRadius * scale / 4;
        realAvoidRadius = avoidRadius* scale / 10;

        SetCameraSize();
    }

    private void SetCameraSize()
    {
        //Camera.main;
    }

    /*Here comes the parralel part.*/

    private bool inited = false;
    private List<Thread> threads;
    private ManualResetEvent sleepingEvent;
    private ConcurrentQueue<Boid> workQueue;
    private CancellationTokenSource cts;
    private int tasksRan = 0;

    public int GetTasksRun { get { return tasksRan; } }
    public int GetThreadCount { get { return threads.Count; } }

    /// <summary>
    /// Prallel init
    /// </summary>
    private void ParallelInit()
    {
        workQueue = new ConcurrentQueue<Boid>();
        threads = new List<Thread>();
        cts = new CancellationTokenSource();
        sleepingEvent = new ManualResetEvent(false);

        for (int i = 0; i < Environment.ProcessorCount - 1; i++)
        {
            Thread t = new Thread(() => Process(cts.Token, sleepingEvent));
            t.Start();
            threads.Add(t);
        }
    }

    private void Update()
    {
        if (inited)
        {
            Boid temp;
            foreach (var item in workQueue)
            {
                workQueue.TryDequeue(out temp);
            }

            for (int i = 0; i < boids.Count; i++)
            {
                UpdatePositions(boidObjects[i], boids[i]);
                workQueue.Enqueue(boids[i]);
            }

            sleepingEvent.Set();
        }
    }

    private void UpdatePositions(GameObject boidObject, Boid boid)
    {
        boidObject.transform.localPosition = boid.Position;
        boidObject.transform.rotation = boid.Rotation;
    }

    private void Process(CancellationToken ct, ManualResetEvent sleepEvent)
    {
        System.Random rnd = new System.Random();
        ManualResetEvent _event = new ManualResetEvent(true);
        Boid currentBoid;

        while (!ct.IsCancellationRequested)
        {
            currentBoid = null;
            workQueue.TryDequeue(out currentBoid);

            if (currentBoid != null)
            {
                currentBoid.MyUpdate();
            }
            else
            {
                //Thread.Sleep(rnd.Next(5, 30));
                Interlocked.Increment(ref tasksRan);
                sleepEvent.WaitOne();
            }
        }
    }

    private void OnDestroy()
    {
        cts.Cancel();
        sleepingEvent.Close();

        foreach (var t in threads)
        {
            t.Join();
        }
    }

}