using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;

public class DroneAgent : Agent
{
    // public variables
    [Header("Propeller motors")]

    /// <summary>
    /// Propeller component of front left propeller of the drone
    /// </summary>
    [Tooltip("Propeller component of front left propeller of the drone")]
    public Propellers frontLeftPropeller;

    /// <summary>
    /// Propeller component of front right propeller of the drone
    /// </summary>
    [Tooltip("Propeller component of front right propeller of the drone")]

    public Propellers frontRightPropeller;

    /// <summary>
    /// Propeller component of back left propeller of the drone
    /// </summary>
    [Tooltip("Propeller component of back left propeller of the drone")]

    public Propellers backLeftPropeller;

    /// <summary>
    /// Propeller component of back right propeller of the drone
    /// </summary>
    [Tooltip("Propeller component of back right propeller of the drone")]

    public Propellers backRightPropeller;

    [Header("Movement Variables")]
    [Tooltip("The amount the action should me multiplied")]
    public float actionMultiplier;

    // private variables
    private Rigidbody rb;
    EnvironmentParameters m_ResetParams;


    /// <summary>
    /// Called when the scene is initialied
    /// </summary>
    public override void Initialize()
    {
        base.Initialize();
        rb = this.GetComponent<Rigidbody>();
        m_ResetParams = Academy.Instance.EnvironmentParameters;

        ResetParameters();
    }

    /// <summary>
    /// Called when the episode begins
    /// </summary>
    public override void OnEpisodeBegin()
    {
        base.OnEpisodeBegin();

        // reset parameters
        ResetParameters();
    }

    /// <summary>
    /// Collects the observations to the sensor and 
    /// transfers the observations to input of the neural networks
    /// </summary>
    /// <param name="sensor">The sensor component of the agent</param>
    public override void CollectObservations(VectorSensor sensor)
    {
        base.CollectObservations(sensor);

        sensor.AddObservation(1f);
        // TODO: implement the observe the environment code
    }

    /// <summary>
    /// Called when the decision maker makes a decision and get the vectorAction
    /// from the output of the Neural Network.
    /// The Index i of vectorAction represents:
    /// Index 0: motor speed of leftFront motor (+1 = increase the speed, -1 decrease speed)
    /// Index 1: motor speed of rightFront motor (+1 = increase the speed, -1 decrease speed)
    /// Index 2: motor speed of leftBack motor (+1 = increase the speed, -1 decrease speed)
    /// Index 3: motor speed of rightBack motor (+1 = increase the speed, -1 decrease speed)
    /// </summary>
    /// <param name="vectorAction">The actions to be follewed by the agent</param>
    public override void OnActionReceived(float[] vectorAction)
    {
        base.OnActionReceived(vectorAction);

        this.frontLeftPropeller.AddForce = vectorAction[0] * actionMultiplier;
        this.frontRightPropeller.AddForce = vectorAction[1] * actionMultiplier;
        this.backLeftPropeller.AddForce = vectorAction[2] * actionMultiplier;
        this.backLeftPropeller.AddForce = vectorAction[3] * actionMultiplier;
    }

    /// <summary>
    /// When behaviour type is set to "heuristic only" on the agent's Behaviour parameters,
    /// this function will be called. Its return values will be fed into 
    /// <see cref="OnActionReceived(float[])"> instead of using the neural networks. 
    /// </summary>
    /// <param name="actionsOut"></param>
    public override void Heuristic(float[] actionsOut)
    {
        float altitude = 0f;      // +1 = gain altitude, -1 = lose altitude
        float yaw = 0f;         // +1 = turn left, -1 = turn right
        float pitch = 0f;       // +1 to tilt forward, -1 to tilt backward
        float roll = 0f;        // +1 to roll left, -1 to tilt right

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

        float frontLeft = 0f;
        float frontRight = 0f;
        float backLeft = 0f;
        float backRight = 0f;

        if (altitude > 0)
        {
            frontLeft += 0.25f;
            frontRight += 0.25f;
            backLeft += 0.25f;
            backRight += 0.25f;
        }
        else if (altitude < 0)
        {
            frontLeft -= 0.25f;
            frontRight -= 0.25f;
            backLeft -= 0.25f;
            backRight -= 0.25f;
        }

        if (yaw > 0)
        {
            frontLeft += !frontLeftPropeller.clockWise ? 0.25f : -0.25f;
            frontRight += !frontRightPropeller.clockWise ? 0.25f : -0.25f;
            backLeft += !backLeftPropeller.clockWise ? 0.25f : -0.25f;
            backRight += !backRightPropeller.clockWise ? 0.25f : -0.25f;
        }
        else if (yaw < 0)
        {
            frontLeft += frontLeftPropeller.clockWise ? 0.25f : -0.25f;
            frontRight += frontRightPropeller.clockWise ? 0.25f : -0.25f;
            backLeft += backLeftPropeller.clockWise ? 0.25f : -0.25f;
            backRight += backRightPropeller.clockWise ? 0.25f : -0.25f;
        }

        if (pitch > 0)
        {
            frontLeft -= 0.25f;
            frontRight -= 0.25f;
            backLeft += 0.25f;
            backRight += 0.25f;
        }
        else if (pitch < 0)
        {
            frontLeft += 0.25f;
            frontRight += 0.25f;
            backLeft -= 0.25f;
            backRight -= 0.25f;
        }

        if (roll > 0)
        {
            frontLeft -= 0.25f;
            backLeft -= 0.25f;
            frontRight += 0.25f;
            backRight += 0.25f;
        }
        else if (roll < 0)
        {
            frontLeft += 0.25f;
            backLeft += 0.25f;
            frontRight -= 0.25f;
            backRight -= 0.25f;
        }

        actionsOut[0] = frontLeft;
        actionsOut[1] = frontRight;
        actionsOut[2] = backLeft;
        actionsOut[3] = backRight;
    }

    /// <summary>
    /// Resets the parameters
    /// </summary>
    void ResetParameters()
    {
        ResetDrone();
        ResetPhysics();
        ResetPropellers();
    }

    /// <summary>
    /// Resets the drone varaibles to previous possitions
    /// </summary>
    void ResetDrone()
    {
        this.rb.mass = m_ResetParams.GetWithDefault("mass", 0.2f);
        this.rb.drag = m_ResetParams.GetWithDefault("drag", 0.2f);
        this.rb.angularDrag = m_ResetParams.GetWithDefault("angular_drag", 0.1f);
        this.rb.useGravity = true;
        float scale = m_ResetParams.GetWithDefault("scale", 1f);
        this.transform.localScale = new Vector3(scale, scale, scale);
    }

    /// <summary>
    /// Resets the physics varaibles of the scene
    /// </summary>
    void ResetPhysics()
    {
        Physics.gravity = new Vector3(0f, m_ResetParams.GetWithDefault("gravity", -9.81f), 0f);
        Time.fixedDeltaTime = m_ResetParams.GetWithDefault("update_time", 0.02f);
    }

    /// <summary>
    /// Resets the propeller varaibles
    /// </summary>
    void ResetPropellers()
    {
        bool leftDiagonal = 1 == m_ResetParams.GetWithDefault("left_diagonal", 1);
        this.frontLeftPropeller.clockWise = leftDiagonal;
        this.backRightPropeller.clockWise = leftDiagonal;
        this.frontRightPropeller.clockWise = !leftDiagonal;
        this.backLeftPropeller.clockWise = !leftDiagonal;
    }

    /// <summary>
    /// Called when the collider of the gameobject detects a collision with other object
    /// </summary>
    /// <param name="other"></param>
    private void OnCollisionEnter(Collision other)
    {
        Debug.Log("collided with some object. resetting the scene");
        EndEpisode();
    }
}
