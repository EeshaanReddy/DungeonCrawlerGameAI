using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Kinematic
{
    public Vector3 position;
    public float orientation;
    public Vector3 velocity;
    public float rotation;
    public float maxAcceleration;

    public void Update(Steering_output steering, float maxSpeed, float time)
    {
        // Update the position and orientation.
        position += velocity * Time.deltaTime;
        orientation += rotation * Time.deltaTime;

        // Update the velocity and rotation.
        velocity += steering.linear * time;
        rotation += steering.angular * time;

        // Check for speeding and clip.
        if (velocity.magnitude > maxSpeed)
        {
            velocity.Normalize();
            velocity *= maxSpeed;
        }
    }
}

public class Steering_output
{
    public Vector3 linear;
    public float angular;
}

public abstract class Steering : MonoBehaviour
{
    public abstract Steering_output getSteering(Vector3 targetLoc, bool flee);
   // public abstract Kinematic getKinematic();

    void Update()
    { 
        Steering_output steering = getSteering(Vector3.zero, false);
    }
}
