using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AvoidCollision : MonoBehaviour
{
    public float repulseThreshold;

    public float maxAccel;

    public VelManager vel;

    private Vector3 lastVelocity = Vector3.zero;

    private void FixedUpdate()
    {
        float timeToCollsion = 2;
        Transform target = null;
        float minSeparationActual = 0;
        float distActual = 0;
        Vector3 relativePosActual = Vector3.zero;
        Vector3 relativeVelActual = Vector3.zero;

        foreach (Enemy enemy in Enemies.enemyManager.enemies)
        {
            if(enemy.gameObject == gameObject)
                continue;
            Vector3 relativePos = enemy.transform.position - transform.position;
            Vector3 relativeVel = enemy.transform.GetComponent<Rigidbody>().velocity - vel.velocity;
            float relativeSpeed = relativeVel.magnitude;
            float curTimeToCollision = Vector3.Dot(relativePos, relativeVel) / (relativeSpeed * relativeSpeed);
            float dist = relativePos.magnitude;
            float minSeparation = dist - relativeSpeed * curTimeToCollision;
            if(minSeparation < repulseThreshold)
                continue;
            if (curTimeToCollision > 0 && curTimeToCollision < timeToCollsion)
            {
                timeToCollsion = curTimeToCollision;
                target = enemy.transform;
                minSeparationActual = minSeparation;
                distActual = dist;
                relativePosActual = relativePos;
                relativeVelActual = relativeVel;
            }
        }

        Vector3 velocity = vel.velocity;
        if (target == null)
        {
            velocity -= lastVelocity;
            lastVelocity = Vector3.zero;
            velocity += lastVelocity;
            vel.velocity = velocity;
            return;
        }
        if (minSeparationActual <= 0 || distActual < repulseThreshold)
        {
            relativePosActual = target.position - transform.position;
        }
        else
        {
            relativePosActual = relativePosActual + relativeVelActual * timeToCollsion;
        }
        relativePosActual.Normalize();
        velocity -= lastVelocity;
        lastVelocity = -relativePosActual * maxAccel * 1/Vector3.Distance(target.position, transform.position);
        velocity += lastVelocity;
        vel.velocity = velocity;
    }
}
