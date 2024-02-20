using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flee : Steering
{
    public GameObject target;

    public bool hord1;

    Vector3 currentLocation;
    
    private Vector3 lastVelocity = Vector3.zero;

    public override Steering_output getSteering(Vector3 targetLoc, bool flee)
    {
        Steering_output steering = new Steering_output();

        Vector3 directionToTarget =
            (flee) ? -targetLoc + transform.position : targetLoc - transform.position;
        steering.linear = directionToTarget.normalized;

        steering.angular = 0f;

        return steering;
    }

    void Start()
    {
        currentLocation = transform.position;
    }

    void FixedUpdate()
    {

        if ((hord1) ? target.transform.position.x < 44 && target.transform.position.z < 32 :
           target.transform.position.x > 58 && target.transform.position.z > 46)
        {
            Steering_output steering = getSteering(target.transform.position, true);
            steering.linear *= 1.5f;
            Vector3 vel = base.vel.velocity;
            vel -= lastVelocity;
            lastVelocity = steering.linear;
            vel += lastVelocity;
            base.vel.velocity = vel;
        }
        else if ((transform.position - currentLocation).magnitude >= 0.5f)
        {
            Steering_output steering = getSteering(currentLocation, false);
            steering.linear *= 1.5f;
            Vector3 vel = base.vel.velocity;
            vel -= lastVelocity;
            lastVelocity = steering.linear;
            vel += lastVelocity;
            base.vel.velocity = vel;
        }

    }
}

