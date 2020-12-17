using UnityEngine;

public class DroneMovement : MonoBehaviour
{
    // PUBLIC VARAIBLES
    [Header("Propellers")]
    /// <summary>
    /// The front left propelller's PropellerMotion script of the drone
    /// </summary>
    [Tooltip("The front left propelller's PropellerMotion script of the drone")]
    public PropellerMotion frontLeftProp;

    /// <summary>
    /// The front right propelller's PropellerMotion script of the drone
    /// </summary>
    [Tooltip("The front right propelller's PropellerMotion script of the drone")]
    public PropellerMotion frontRightProp;

    /// <summary>
    /// The back left propelller's PropellerMotion script of the drone
    /// </summary>
    [Tooltip("The back left propelller's PropellerMotion script of the drone")]
    public PropellerMotion backLeftProp;

    /// <summary>
    /// The back right propelller's PropellerMotion script of the drone
    /// </summary>
    [Tooltip("The back right propelller's PropellerMotion script of the drone")]
    public PropellerMotion backRightProp;

    // PRIVATE VARAIBLES
    [Space]
    [Header("Drone Components")]
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
    private float altitudeRate = 5f;
    [Tooltip("The move speed")]
    [Range(5f, 50f)]
    [SerializeField]
    private float moveSpeed = 5f;

    [Tooltip("The yaw change rate")]
    [Range(50f, 200f)]
    [SerializeField]
    private float yawRate = 50f;

    [Tooltip("The pitch change rate")]
    [Range(50f, 200f)]
    [SerializeField]
    private float pitchRate = 50f;

    [Tooltip("The roll change rate")]
    [Range(50f, 200f)]
    [SerializeField]
    private float rollRate = 50f;

    [SerializeField]
    [Tooltip("The propeller force to be add or subtracted")]
    private float propellerForce = 1f;

    private Rigidbody droneRb;
    private float altitude = 0f;      // +1 = gain altitude, -1 = lose altitude
    private float yaw = 0f;         // +1 = turn left, -1 = turn right
    private float pitch = 0f;       // +1 to tilt forward, -1 to tilt backward
    private float roll = 0f;        // +1 to roll left, -1 to tilt right

    /// <summary>
    /// The altitude change in the drone
    /// </summary>
    /// <returns></returns>
    [HideInInspector]
    public float Altitude { set => altitude = value; }
    [HideInInspector]
    public float Yaw { set => yaw = value; }
    [HideInInspector]
    public float Pitch { set => pitch = value; }
    [HideInInspector]
    public float Roll { set => roll = value; }
    public Transform ForwardTf { get => forwardTf; }


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
        // ResetInputs();
        CheckReset();

        // CheckInputs();
        ApplyMovementToDrone();
    }

    /// <summary>
    /// Applies the given input values to move the drone 
    /// </summary>
    void ApplyMovementToDrone()
    {
        // apply the effects of input
        if (altitude > 0)
        {
            // drone movement
            droneTf.Translate(forwardTf.up * Time.deltaTime * altitudeRate * altitude, Space.World);

            // propellers forces affected
            frontLeftProp.AddForce = propellerForce;
            frontRightProp.AddForce = propellerForce;
            backLeftProp.AddForce = propellerForce;
            backRightProp.AddForce = propellerForce;
        }
        else if (altitude < 0)
        {
            // drone movement
            droneTf.Translate(forwardTf.up * Time.deltaTime * altitudeRate * altitude, Space.World);

            // propellers forces affected
            frontLeftProp.AddForce = -propellerForce;
            frontRightProp.AddForce = -propellerForce;
            backLeftProp.AddForce = -propellerForce;
            backRightProp.AddForce = -propellerForce;
        }

        if (yaw > 0)
        {
            // drone movement
            droneTf.Rotate(-forwardTf.up * Time.deltaTime * yawRate * yaw, Space.World);

            // propeller forces affected
            frontLeftProp.AddForce = frontLeftProp.clockWise ? -propellerForce : propellerForce;
            frontRightProp.AddForce = frontRightProp.clockWise ? -propellerForce : propellerForce;
            backLeftProp.AddForce = backLeftProp.clockWise ? -propellerForce : propellerForce;
            backRightProp.AddForce = backRightProp.clockWise ? -propellerForce : propellerForce;
        }
        else if (yaw < 0)
        {
            // drone movement
            droneTf.Rotate(-forwardTf.up * Time.deltaTime * yawRate * yaw, Space.World);

            // propeller forces affected
            frontLeftProp.AddForce = frontLeftProp.clockWise ? propellerForce : -propellerForce;
            frontRightProp.AddForce = frontRightProp.clockWise ? propellerForce : -propellerForce;
            backLeftProp.AddForce = backLeftProp.clockWise ? propellerForce : -propellerForce;
            backRightProp.AddForce = backRightProp.clockWise ? propellerForce : -propellerForce;
        }

        if (pitch > 0)
        {
            // drone movement
            droneTf.Translate(forwardTf.forward * Time.deltaTime * moveSpeed * pitch, Space.World);
            droneTf.Rotate(forwardTf.right * Time.deltaTime * pitchRate * pitch, Space.World);

            // propellers affected
            frontLeftProp.AddForce = -propellerForce;
            frontRightProp.AddForce = -propellerForce;
            backLeftProp.AddForce = propellerForce;
            backRightProp.AddForce = propellerForce;
        }
        else if (pitch < 0)
        {
            // drone movement
            droneTf.Translate(forwardTf.forward * Time.deltaTime * moveSpeed * pitch, Space.World);
            droneTf.Rotate(forwardTf.right * Time.deltaTime * pitchRate * pitch, Space.World);

            // propellers affected
            frontLeftProp.AddForce = propellerForce;
            frontRightProp.AddForce = propellerForce;
            backLeftProp.AddForce = -propellerForce;
            backRightProp.AddForce = -propellerForce;
        }

        if (roll > 0)
        {
            // drone movement
            droneTf.Translate(-forwardTf.right * Time.deltaTime * moveSpeed * roll, Space.World);
            droneTf.Rotate(forwardTf.forward * Time.deltaTime * rollRate * roll, Space.World);

            // propeller affected
            frontLeftProp.AddForce = -propellerForce;
            backLeftProp.AddForce = -propellerForce;
            frontRightProp.AddForce = propellerForce;
            backRightProp.AddForce = propellerForce;
        }
        else if (roll < 0)
        {
            // drone movement
            droneTf.Translate(-forwardTf.right * Time.deltaTime * moveSpeed * roll, Space.World);
            droneTf.Rotate(forwardTf.forward * Time.deltaTime * rollRate * roll, Space.World);

            // propeller affected
            frontLeftProp.AddForce = propellerForce;
            backLeftProp.AddForce = propellerForce;
            frontRightProp.AddForce = -propellerForce;
            backRightProp.AddForce = -propellerForce;
        }

        // move towards the ideal position
        droneTf.rotation = Quaternion.Slerp(droneTf.rotation, Quaternion.Euler(0f, droneTf.rotation.eulerAngles.y, 0f), Time.deltaTime * idealRotationRate);
    }

    /// <summary>
    /// Checks for the user input and changes the varaibles 
    /// which are to be changed with the user inputs
    /// </summary>
    void CheckInputs()
    {
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
    }

    /// <summary>
    /// Checks if the reset key is pressed and resets the drone 
    /// velocity and angular velocity accordingly
    /// </summary>
    void CheckReset()
    {
        if (Input.GetKey(KeyCode.R))
        {
            droneRb.velocity = Vector3.zero;
            droneRb.angularVelocity = Vector3.zero;
        }
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
