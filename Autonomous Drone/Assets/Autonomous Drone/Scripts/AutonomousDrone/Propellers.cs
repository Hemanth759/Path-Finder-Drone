using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Propellers : MonoBehaviour
{
    // PUBLIC VARAIBLES
    [Header("Propeller properties")]
    [Space]
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

    /// <summary>
    /// The force value obtained from the 
    /// </summary>
    [HideInInspector]
    public float AddForce
    {
        set
        {
            this.currentForce = baseForce + value;
            this.currentTorque = baseRotationTorque + value;
        }
    }

    [Space]
    [Header("Base values")]

    /// <summary>
    /// The base force to be applied by the propeller when drone is in idle mode
    /// </summary>
    [Tooltip("The base force to be applied by the propeller when drone is in idle mode")]
    public float baseForce;

    /// <summary>
    /// The base torque to be applied by the propeller when drone is in idle mode
    /// </summary>
    [Tooltip("The base torque to be applied by the propeller when drone is in idle mode")]
    public float baseRotationTorque;


    // PRIVATE VARAIBLES
    /// <summary>
    /// The rigidbody of the drone
    /// </summary>
    private Rigidbody droneRb;

    /// <summary>
    /// The force value tobe added which is changed from the input values
    /// </summary>
    private float currentForce;
    private float currentTorque;


    /// <summary>
    /// Called when the game object is initailied
    /// </summary>
    private void Start()
    {
        droneRb = this.GetComponentInParent<Rigidbody>();
        currentForce = baseForce;
        currentTorque = baseRotationTorque;
    }

    /// <summary>
    /// Called for every physics update
    /// </summary>
    private void FixedUpdate()
    {
        AddForceToDrone();
        AddTorqueToDrone();
        ResetForce();
    }

    /// <summary>
    /// Called for every frame
    /// </summary>
    private void Update()
    {
        RotatePropeller();
        AddForceToDrone();
        AddTorqueToDrone();
        ResetForce();
    }

    void ResetForce()
    {
        currentForce = baseForce;
        currentTorque = baseRotationTorque;
    }

    void AddTorqueToDrone()
    {
        if (this.clockWise)
            droneRb.AddTorque(droneRb.transform.up * Time.fixedDeltaTime * currentTorque);
        else
            droneRb.AddTorque(-droneRb.transform.up * Time.fixedDeltaTime * currentTorque);
    }

    void AddForceToDrone()
    {
        droneRb.AddForceAtPosition(this.transform.forward * currentForce * Time.fixedDeltaTime, this.transform.position);
    }

    void RotatePropeller()
    {
        int clockWise = this.clockWise ? 1 : -1;
        this.transform.Rotate(Vector3.forward, Time.deltaTime * this.rotationSpeed * clockWise);
    }
}
