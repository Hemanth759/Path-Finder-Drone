using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroneMovement : MonoBehaviour
{
    // PUBLIC VARAIBLES


    // PRIVATE VARAIBLES
    private Transform droneTf;
    [Tooltip("The tranform which points the forward direction of this drone")]
    [SerializeField]
    private Transform forwardTf = null;

    [Tooltip("The rate at which the drone returns to the ideal rotation")]
    [SerializeField]
    private float idealRotationRate = 1f;

    [Tooltip("The move speed")]
    [Range(5f, 50f)]
    [SerializeField]
    private float moveSpeed = 5f;

    [Tooltip("The yaw change rate")]
    [Range(50f, 200f)]
    [SerializeField]
    private float yawRate = 50f;

    [Tooltip("The pitch change rate")]
    [SerializeField]
    private float pitchRate = 50f;

    [Tooltip("The roll change rate")]
    [SerializeField]
    private float rollRate = 50f;

    private Rigidbody droneRb;
    private float altitude = 0f;      // +1 = gain altitude, -1 = lose altitude
    private float yaw = 0f;         // +1 = turn left, -1 = turn right
    private float pitch = 0f;       // +1 to tilt forward, -1 to tilt backward
    private float roll = 0f;        // +1 to roll left, -1 to tilt right


    /// <summary>
    /// Start is called before the first frame update
    /// </summary>
    private void Start()
    {
        droneTf = this.transform;
        droneRb = this.GetComponent<Rigidbody>();
        ResetInputs();
    }

    /// <summary>
    /// Called for every frame
    /// </summary>
    private void Update()
    {
        ResetInputs();

        // press w to gain height or s to loss height
        if (Input.GetKey(KeyCode.W))
        {
            altitude = 1f;
        }
        else if (Input.GetKey(KeyCode.S))
        {
            altitude = -1f;
        }

        // press a to turn left or d to turn right
        if (Input.GetKey(KeyCode.A))
        {
            yaw = 1f;
        }
        else if (Input.GetKey(KeyCode.D))
        {
            yaw = -1f;
        }

        // press num 8 to tilt forward or num 2 to tilt backward
        if (Input.GetKey(KeyCode.Keypad8))
        {
            pitch = 1;
        }
        else if (Input.GetKey(KeyCode.Keypad2))
        {
            pitch = -1;
        }

        // press num 4 to roll left or num 6 to roll right
        if (Input.GetKey(KeyCode.Keypad4))
        {
            roll = 1;
        }
        else if (Input.GetKey(KeyCode.Keypad6))
        {
            roll = -1;
        }

        // apply the effects of input
        if (altitude > 0)
        {
            droneTf.Translate(forwardTf.up * Time.deltaTime * moveSpeed, Space.World);
        }
        else if (altitude < 0)
        {
            droneTf.Translate(-forwardTf.up * Time.deltaTime * moveSpeed, Space.World);
        }

        if (yaw > 0)
        {
            droneTf.Rotate(-forwardTf.up * Time.deltaTime * yawRate, Space.World);
        }
        else if (yaw < 0)
        {
            droneTf.Rotate(forwardTf.up * Time.deltaTime * yawRate, Space.World);
        }

        if (pitch > 0)
        {
            droneTf.Translate(forwardTf.forward * Time.deltaTime * moveSpeed, Space.World);
            droneTf.Rotate(forwardTf.right * Time.deltaTime * pitchRate, Space.World);
        }
        else if (pitch < 0)
        {
            droneTf.Translate(-forwardTf.forward * Time.deltaTime * moveSpeed, Space.World);
            droneTf.Rotate(-forwardTf.right * Time.deltaTime * pitchRate, Space.World);
        }

        if (roll > 0)
        {
            droneTf.Translate(-forwardTf.right * Time.deltaTime * moveSpeed, Space.World);
            droneTf.Rotate(forwardTf.forward * Time.deltaTime * rollRate, Space.World);
        }
        else if (roll < 0)
        {
            droneTf.Translate(forwardTf.right * Time.deltaTime * moveSpeed, Space.World);
            droneTf.Rotate(-forwardTf.forward * Time.deltaTime * rollRate, Space.World);
        }

        // move towards the ideal position
        droneTf.rotation = Quaternion.Slerp(droneTf.rotation, Quaternion.Euler(0f, droneTf.rotation.eulerAngles.y, 0f), Time.deltaTime * idealRotationRate);
    }

    /// <summary>
    /// Resets the input varaibles
    /// </summary>
    void ResetInputs()
    {
        altitude = 0f;
        yaw = 0f;
        pitch = 0f;
        roll = 0f;
    }

    /// <summary>
    /// fixed Update is called once per every <see cref="Time.fixedDeltaTime">
    /// </summary>
    private void FixedUpdate()
    {

    }
}
