using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Testing : MonoBehaviour
{
    public float force;
    public Propellers FrontLeft;
    public Propellers FrontRight;
    public Propellers BackRight;
    public Propellers BackLeft;

    private void FixedUpdate()
    {
        if (Input.GetKey(KeyCode.W))
        {
            FrontLeft.AddForce = force;
            FrontRight.AddForce = force;
            BackLeft.AddForce = force;
            BackRight.AddForce = force;
        }
        else if (Input.GetKey(KeyCode.S))
        {
            FrontLeft.AddForce = -force;
            FrontRight.AddForce = -force;
            BackLeft.AddForce = -force;
            BackRight.AddForce = -force;
        }
    }
}
