using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Propellers : MonoBehaviour
{
    // PUBLIC VARAIBLES
    /// <summary>
    /// True if the propeller rotates clockwise, 
    /// False if the propeller rotates anticlockwise
    /// </summary>
    [Tooltip("The propeller rotation direction.")]
    public bool clockWise;

    /// <summary>
    /// The speed at which the propeller rotates.
    /// </summary>
    [Tooltip("Rotation speed of the propeller")]
    public float rotationSpeed;

    // PRIVATE VARAIBLES
    /// <summary>
    /// The rigidbody of the drone
    /// </summary>
    private Rigidbody droneRb;

    /// <summary>
    /// Called when the game object is initailied
    /// </summary>
    private void Start()
    {
        droneRb = this.GetComponentInParent<Rigidbody>();
    }

    /// <summary>
    /// Called for every physics update
    /// </summary>
    private void FixedUpdate()
    {
        RotatePropeller();
        // MoveDrone();
    }

    void MoveDrone()
    {
        droneRb.AddForceAtPosition(this.transform.forward * Time.fixedDeltaTime, this.transform.position);
        if (this.clockWise)
            droneRb.AddTorque(droneRb.transform.up * Time.fixedDeltaTime);
        else
            droneRb.AddTorque(-droneRb.transform.up * Time.fixedDeltaTime);
    }

    void RotatePropeller()
    {
        int clockWise = this.clockWise ? 1 : -1;
        this.transform.Rotate(Vector3.forward, Time.fixedDeltaTime * this.rotationSpeed * clockWise);
    }
}
