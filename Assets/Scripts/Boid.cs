using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

/// <summary>
/// The script that a fish/bird has that controlls its movement.
/// </summary>
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

    /// <summary>
    /// Init.
    /// </summary>
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

    /// <summary>
    /// Fixed updated because I used Rigidbody. Physics based components should be updated in FixedUpdate.
    /// </summary>
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

    /// <summary>
    /// Draws a sphere with friendRadius radius around the fish/bird if the option is enabled in the BoidController.
    /// Only for debugging.
    /// </summary>
    private void OnDrawGizmos()
    {
        if (controller.showFriendRadius)
            Gizmos.DrawWireSphere(this.transform.position, FriendRadius);
    }

    /// <summary>
    /// Increments the think timer.
    /// </summary>
    private void Increment()
    {
        thinkTimer = (thinkTimer + 1) % 5;
    }

    /// <summary>
    /// Checks for other Boids within friend radius and adds them to friends list.
    /// </summary>
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

    /// <summary>
    /// Moves the Boid in the appropriate direction.
    /// </summary>
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

    /// <summary>
    /// Averages the other Boid's heading direction in the friends list.
    /// </summary>
    /// <returns>The avarage heading direction of the group.</returns>
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

    /// <summary>
    /// Checks whether this Boid is too close to other Boids.
    /// </summary>
    /// <returns>The direction this Boid should too not get too close to others.</returns>
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

    /// <summary>
    /// Checks whether there are obstacles in its way.
    /// </summary>
    /// <returns>The direction this Boid should avoid getting too close to the obstacles.</returns>
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

    /// <summary>
    /// Averages the other Boid's direction in the friends list.
    /// </summary>
    /// <returns>The avarage direction of the group.</returns>
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

    /// <summary>
    /// If the Boid hits the bounding walls the appropriate heading direction components gets inverted.
    /// </summary>
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

    /// <summary>
    /// Turns the Boid to the heading direction.
    /// </summary>
    private void TurnToVelocityDirection()
    {
        this.transform.rotation = Quaternion.LookRotation(Move);
    }

}
