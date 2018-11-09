using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Boid : MonoBehaviour
{
    private BoidsController controller;

    private Rigidbody rigid;

    //private Vector3 move;
    private List<GameObject> friends;

    private int thinkTimer = 0;

    public BoidsController SetController { set { this.controller = value; } }

    public int FriendsCount { get { return friends.Count; } }

    private Vector3 Move { get { return rigid.velocity; } set { rigid.velocity = value; } }

    public BoxCollider Bounds { get { return controller.Bounds; } }
    private List<GameObject> Boids { get { return controller.Boids; } }
    private float FriendRadius { get { return controller.friendRadius; } }
    private float CrowdRadius { get { return controller.crowdRadius; } }
    private float AvoidRadius { get { return controller.avoidRadius; } }
    private float CoheseRadius { get { return controller.coheseRadius; } }
    private float MaxSpeed { get { return controller.maxSpeed; } }

    private List<GameObject> Avoids { get { return controller.Avoids; } }

    private bool OptionFriend { get { return controller.optionFriend; } }

    private bool OptionCrowd { get { return controller.optionCrowd; } }

    private bool OptionAvoid { get { return controller.optionAvoid; } }

    private bool OptionNoise { get { return controller.optionNoise; } }

    private bool OptionCohese { get { return controller.optionCohese; } }

    private float NoiseMaximum { get { return controller.maxNoise; } }

    private void Start()
    {
        rigid = this.GetComponent<Rigidbody>();

        friends = new List<GameObject>();

        Move = new Vector3(
            Random.Range(-MaxSpeed, MaxSpeed),
            Random.Range(-MaxSpeed, MaxSpeed),
            Random.Range(-MaxSpeed, MaxSpeed));

        //Move.Normalize();
        Move = Vector3.ClampMagnitude(Move, MaxSpeed);

        thinkTimer = Random.Range(0, 10);
    }

    private void FixedUpdate()
    {
        Increment();

        if (thinkTimer == 0)
        {
            GetFriends();
        }

        Flock();

        Move = Vector3.ClampMagnitude(Move, MaxSpeed);

        KeepBoundaries();

        TurnToVelocityDirection();
    }

    private void OnDrawGizmos()
    {
        if (controller.showFriendRadius)
            Gizmos.DrawWireSphere(this.transform.position, FriendRadius);
    }

    private void Increment()
    {
        thinkTimer = (thinkTimer + 1) % 5;
    }

    private void GetFriends()
    {
        List<GameObject> nearby = new List<GameObject>();
        foreach (GameObject actBoid in Boids)
        {
            GameObject test = actBoid;
            if (test != this.gameObject)
            {
                if (Vector3.Distance(this.transform.position, test.transform.position) <= FriendRadius)
                    nearby.Add(test);
            }
        }
        friends = nearby;
    }
    private void Flock()
    {
        Vector3 allign = GetAvarageDirection();
        Vector3 avoidDir = GetCrowdAvoidDirection();
        Vector3 avoidObjects = GetObstacleAvoidDirection();
        Vector3 noise = new Vector3(
            (Random.value * NoiseMaximum) - NoiseMaximum / 2,
            (Random.value * NoiseMaximum) - NoiseMaximum / 2,
            (Random.value * NoiseMaximum) - NoiseMaximum / 2
            );
        Vector3 cohese = GetCohesion();

        Move.Normalize();
        Move *= 1;

        allign *= 1;
        if (!OptionFriend)
            allign *= 0;

        avoidDir *= 1;
        if (!OptionCrowd)
            avoidDir *= 0;

        avoidObjects *= 3;
        if (!OptionAvoid)
            avoidObjects *= 0;

        noise *= 1f;
        if (!OptionNoise)
            noise *= 0;

        cohese *= 1;
        if (!OptionCohese)
            cohese *= 0;

        Move += allign;
        Move += avoidDir;
        Move += avoidObjects;
        Move += noise;
        Move += cohese;

        Move.Normalize();
        Move = Vector3.ClampMagnitude(Move * MaxSpeed, MaxSpeed);
    }

    private Vector3 GetAvarageDirection()
    {
        Vector3 sum = Vector3.zero;
        int count = 0;

        foreach (GameObject other in friends)
        {
            float d = Vector3.Distance(this.transform.position, other.transform.position);
            if (d > 0 && d < FriendRadius)
            {
                Vector3 copy = other.GetComponent<Boid>().Move;
                copy.Normalize();
                copy /= d;
                sum += copy;
                count++;
            }
        }

        sum.Normalize();

        return sum;
    }

    private Vector3 GetCrowdAvoidDirection()
    {
        Vector3 steer = Vector3.zero;
        int count = 0;

        foreach (GameObject other in friends)
        {
            float d = Vector3.Distance(this.transform.position, other.transform.position);
            if (d > 0 && d < CrowdRadius)
            {
                Vector3 difference = this.transform.position - other.transform.position;
                difference.Normalize();
                difference /= d;
                steer += difference;
                count++;
            }
        }
        steer.Normalize();

        return steer;
    }

    public Vector3 GetObstacleAvoidDirection()
    {
        Vector3 steer = Vector3.zero;
        int count = 0;

        foreach (GameObject avoid in Avoids)
        {
            float d = Vector3.Distance(this.transform.position, avoid.GetComponent<Collider>().ClosestPointOnBounds(this.transform.position));

            if (d > 0 && d < AvoidRadius)
            {
                Vector3 difference = this.transform.position - avoid.transform.position;
                difference.Normalize();
                difference /= d;
                steer += difference;
                count++;
            }
        }
        steer.Normalize();

        return steer;
    }

    private Vector3 GetCohesion()
    {
        Vector3 sum = Vector3.zero;
        int count = 0;

        foreach (GameObject other in friends)
        {
            float d = Vector3.Distance(this.transform.position, other.transform.position);
            if (d > 0 && d < CoheseRadius)
            {
                sum += other.transform.position;
                count++;
            }
        }
        if (count > 0)
        {
            sum.Normalize();
            Vector3 desired = sum - this.transform.position;

            return Vector3.ClampMagnitude(desired, 0.05f);
        }
        else
            return Vector3.zero;
    }

    private void KeepBoundaries()
    {
        Vector3 signed = Move;

        if ((this.transform.localPosition.x <= (-Bounds.size.x / 2) && Move.x < 0))
            signed.x = Mathf.Abs(Move.x);
        else if (this.transform.localPosition.x >= (Bounds.size.x / 2) && Move.x > 0)
            signed.x = -Mathf.Abs(Move.x);

        if ((this.transform.localPosition.y <= (-Bounds.size.y / 2) && Move.y < 0))
            signed.y = Mathf.Abs(Move.y);
        else if (this.transform.localPosition.y >= (Bounds.size.y / 2) && Move.y > 0)
            signed.y = -Mathf.Abs(Move.y);

        if ((this.transform.localPosition.z <= (-Bounds.size.z / 2) && Move.z < 0))
            signed.z = Mathf.Abs(Move.z);
        else if (this.transform.localPosition.z >= (Bounds.size.z / 2) && Move.z > 0)
            signed.z = -Mathf.Abs(Move.z);

        Move = signed;
    }

    private void TurnToVelocityDirection()
    {
        this.transform.rotation = Quaternion.LookRotation(Move);
    }

}
