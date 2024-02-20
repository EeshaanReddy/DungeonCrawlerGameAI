using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class VelManager : MonoBehaviour
{
    public Rigidbody rb;

    public Vector3 velocity;

    private void FixedUpdate()
    {
        rb.velocity = velocity;
    }
}
