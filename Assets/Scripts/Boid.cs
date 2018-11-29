using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

/// <summary>
/// The script that a fish/bird has that controlls its movement.
/// </summary>
public class Boid : MonoBehaviour
{
    private Random rnd = new Random();

    private BoidsController controller;

    //private Vector3 move;
    private List<Boid> friends;

    private int thinkTimer = 0;

    public int FriendsCount { get { return friends.Count; } }

    private Vector3 localPos;
    public Vector3 Position { get; set; }
    private Vector3 Move { get; set; }
    public Quaternion Rotation { get; set; }

    private Vector3 boundsSize { get; set; }

    private List<Boid> Boids { get; set; }
    private float FriendRadius { get { return controller.GetFriendRadius; } }
    private float CrowdRadius { get { return controller.GetCrowdRadius; } }
    private float AvoidRadius { get { return controller.GetAvoidRadius; } }
    private float CoheseRadius { get { return controller.GetCoheseRadius; } }
    private float MaxSpeed { get { return controller.GetMaxSpeed; } }

    private List<Bounds> Avoids { get; set; }

    private bool OptionFriend { get { return controller.optionFriend; } }

    private bool OptionCrowd { get { return controller.optionCrowd; } }

    private bool OptionAvoid { get { return controller.optionAvoid; } }

    private bool OptionNoise { get { return controller.optionNoise; } }

    private bool OptionCohese { get { return controller.optionCohese; } }

    private float NoiseMaximum { get { return controller.maxNoise; } }

    public void SetController(BoidsController value, Vector3 pos)
    {
        this.controller = value;
        boundsSize = controller.Bounds.size;
        Boids = controller.Boids;
        Avoids = controller.Avoids;
        localPos = pos;
    }

    /// <summary>
    /// Init.
    /// </summary>
    private void Start()
    {
        friends = new List<Boid>();

        Move = new Vector3(
            (float)(rnd.NextDouble() * MaxSpeed * 2 - MaxSpeed),
            (float)(rnd.NextDouble() * MaxSpeed * 2 - MaxSpeed),
            (float)(rnd.NextDouble() * MaxSpeed * 2 - MaxSpeed));

        Move = Vector3.ClampMagnitude(Move, MaxSpeed);

        thinkTimer = rnd.Next(0, 10);
    }

    /// <summary>
    /// Fixed updated because I used Rigidbody. Physics based components should be updated in FixedUpdate.
    /// </summary>
    public void MyUpdate()
    {
        Increment();

        if (thinkTimer == 0)
        {
            GetFriends();
        }

        Flock();

        KeepBoundaries();

        TurnToVelocityDirection();


        this.localPos += Move;
        this.Position = localPos;
    }

    /// <summary>
    /// Draws a sphere with friendRadius radius around the fish/bird if the option is enabled in the BoidController.
    /// Only for debugging.
    /// </summary>
    private void OnDrawGizmos()
    {
        if (controller.showFriendRadius)
            Gizmos.DrawWireSphere(this.Position, FriendRadius);
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
        List<Boid> nearby = new List<Boid>();
        foreach (var actBoid in Boids)
        {
            Boid test = actBoid;
            if (test != this)
            {
                if (Vector3.Distance(this.localPos, test.Position) <= FriendRadius)
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
        Random rnd = new Random();

        Vector3 allign = GetAvarageDirection();
        Vector3 avoidDir = GetCrowdAvoidDirection();
        Vector3 avoidObjects = GetObstacleAvoidDirection();
        Vector3 noise = new Vector3(
            (float)((rnd.NextDouble() * NoiseMaximum * 2) - NoiseMaximum),
            (float)((rnd.NextDouble() * NoiseMaximum * 2) - NoiseMaximum),
            (float)((rnd.NextDouble() * NoiseMaximum * 2) - NoiseMaximum)
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

        noise *= 0.1f;
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
        Move = Vector3.ClampMagnitude(Move, MaxSpeed);
    }

    /// <summary>
    /// Averages the other Boid's heading direction in the friends list.
    /// </summary>
    /// <returns>The avarage heading direction of the group.</returns>
    private Vector3 GetAvarageDirection()
    {
        Vector3 sum = Vector3.zero;
        int count = 0;

        foreach (var other in friends)
        {
            float d = Vector3.Distance(this.localPos, other.Position);
            if (d > 0 && d < FriendRadius)
            {
                Vector3 copy = other.Move;
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

        foreach (var other in friends)
        {
            float d = Vector3.Distance(this.localPos, other.Position);
            if (d > 0 && d < CrowdRadius)
            {
                Vector3 difference = this.localPos - other.Position;
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

        foreach (var avoid in Avoids)
        {
            //Vector3 closePoint = this.ClosestPointOnBounds(this.localPos, avoid);
            float d = Vector3.Distance(this.localPos, avoid.center);

            if (d > 0 && d < AvoidRadius)
            {
                Vector3 difference = this.localPos - avoid.center;
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

        foreach (var other in friends)
        {
            float d = Vector3.Distance(this.localPos, other.Position);
            if (d > 0 && d < CoheseRadius)
            {
                sum += other.Position;
                count++;
            }
        }
        if (count > 0)
        {
            sum /= count;
            Vector3 desired = sum - this.localPos;

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

        if ((this.localPos.x <= (-boundsSize.x / 2) && Move.x < 0))
            signed.x = Mathf.Abs(Move.x);
        else if (this.localPos.x >= (boundsSize.x / 2) && Move.x > 0)
            signed.x = -Mathf.Abs(Move.x);

        if ((this.localPos.y <= (-boundsSize.y / 2) && Move.y < 0))
            signed.y = Mathf.Abs(Move.y);
        else if (this.localPos.y >= (boundsSize.y / 2) && Move.y > 0)
            signed.y = -Mathf.Abs(Move.y);

        if ((this.localPos.z <= (-boundsSize.z / 2) && Move.z < 0))
            signed.z = Mathf.Abs(Move.z);
        else if (this.localPos.z >= (boundsSize.z / 2) && Move.z > 0)
            signed.z = -Mathf.Abs(Move.z);

        Move = signed;
    }

    /// <summary>
    /// Turns the Boid to the heading direction.
    /// </summary>
    private void TurnToVelocityDirection()
    {
        this.Rotation = Quaternion.LookRotation(Move);
    }

    /// <summary>
    /// Gets the closest point to a vector from a BoxCollider.
    /// (I can't use the built in one because it isn't thread safe.)
    /// </summary>
    /// <param name="point"></param>
    /// <param name="collider"></param>
    /// <returns></returns>
    private Vector3 ClosestPointOnBounds(Bounds collider)
    {
        Vector3 dirVector = this.localPos - new Vector3(0,0,1);

        //var dx = Math.Max(collider.min.x - point.x, point.x - collider.max.x);
        //var dy = Math.Max(collider.min.y - point.y, point.y - collider.max.y);
        //var dz = Math.Max(collider.min.y - point.y, point.y - collider.max.y);

        //return new Vector3(dx, dy, dz);

        return Vector3.zero;
    }

}
