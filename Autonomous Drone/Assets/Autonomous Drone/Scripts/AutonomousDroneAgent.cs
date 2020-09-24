using System.Collections;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;

[RequireComponent(typeof(DroneMovement))]
public class AutonomousDroneAgent : Agent
{
    // PUBLIC VARAIBLES
    [Tooltip("True if running in debug mode")]
    public bool debugMode = true;

    // PRIVATE VARABILES
    [Tooltip("The amount of horizontal distance the drone should be able to receive information")]
    [SerializeField]
    private float maxViewDistance = 10f;

    [Tooltip("The tranform component of the target game object")]
    [SerializeField]
    private Transform target = null;
    private Rigidbody droneRb;
    private TrainingEnvironment environment;
    private EnvironmentParameters m_ResetParams;
    private DroneMovement droneMovement;
    private bool foundGoal;

    /// <summary>
    /// Called when the scene is initialied
    /// </summary>

    public override void Initialize()
    {
        base.Initialize();
        droneRb = this.GetComponent<Rigidbody>();
        m_ResetParams = Academy.Instance.EnvironmentParameters;
        droneMovement = this.GetComponent<DroneMovement>();
        environment = this.GetComponentInParent<TrainingEnvironment>();

        ResetParameters();
    }

    /// <summary>
    /// Called when the episode begins
    /// </summary>
    public override void OnEpisodeBegin()
    {
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
        // observe the direction of the target w.r.t the that of the agent (+3 observations)
        Vector3 toGoalDirection = target.position - this.transform.position;
        sensor.AddObservation(toGoalDirection.normalized);

        // observe the forward direction of the agent (+3 observations)
        sensor.AddObservation(droneMovement.ForwardTf.forward);

        // observe the min of distance of the target from the agent and maxViewDistance (+1 observations)
        float distanceToTarget = Mathf.Min(Vector3.Distance(this.transform.position, target.position), maxViewDistance);
        sensor.AddObservation(distanceToTarget);

        // total observations are 7
    }

    /// <summary>
    /// Called when the decision maker makes a decision and get the vectorAction
    /// from the output of the Neural Network.
    /// The Index i of vectorAction represents:
    /// Index 0: altitude of the drone (+1 = gain altitude, -1 = lose altitude)
    /// Index 1: yaw of the drone (+1 = turn left, -1 = turn right)
    /// Index 2: pitch of the drone (+1 to tilt forward, -1 to tilt backward)
    /// Index 3: roll of the drone (+1 to roll left, -1 to tilt right)
    /// </summary>
    /// <param name="vectorAction">The actions to be follewed by the agent</param>
    public override void OnActionReceived(float[] vectorAction)
    {
        droneMovement.Altitude = vectorAction[0];
        droneMovement.Yaw = vectorAction[1];
        droneMovement.Pitch = vectorAction[2];
        droneMovement.Roll = vectorAction[3];
    }

    /// <summary>
    /// When behaviour type is set to "heuristic only" on the agent's Behaviour parameters,
    /// this function will be called. Its return values will be fed into 
    /// <see cref="OnActionReceived(float[])"> instead of using the neural networks. 
    /// </summary>
    /// <param name="actionsOut"></param>
    public override void Heuristic(float[] actionsOut)
    {
        actionsOut[0] = 0f;
        actionsOut[1] = 0f;
        actionsOut[2] = 0f;
        actionsOut[3] = 0f;

        // press w to gain height or s to loss height
        if (Input.GetKey(KeyCode.W))
        {
            actionsOut[0] = 1f;
        }
        else if (Input.GetKey(KeyCode.S))
        {
            actionsOut[0] = -1f;
        }

        // press a to turn left or d to turn right
        if (Input.GetKey(KeyCode.A))
        {
            actionsOut[1] = 1f;
        }
        else if (Input.GetKey(KeyCode.D))
        {
            actionsOut[1] = -1f;
        }

        // press num 8 to tilt forward or num 2 to tilt backward
        if (Input.GetKey(KeyCode.Keypad8))
        {
            actionsOut[2] = 1;
        }
        else if (Input.GetKey(KeyCode.Keypad2))
        {
            actionsOut[2] = -1;
        }

        // press num 4 to roll left or num 6 to roll right
        if (Input.GetKey(KeyCode.Keypad4))
        {
            actionsOut[3] = 1;
        }
        else if (Input.GetKey(KeyCode.Keypad6))
        {
            actionsOut[3] = -1;
        }
    }

    /// <summary>
    /// Resets the parameters
    /// </summary>
    void ResetParameters()
    {
        ResetDrone();
        ResetPhysics();
        ResetPropellers();
        ResetGoal();
    }

    /// <summary>
    /// Resets the goal location
    /// </summary> 
    void ResetGoal()
    {
        // move the goal to random safe place
        bool inFrontOfDrone = 0.6f < UnityEngine.Random.Range(0f, 1f);
        environment.MoveGoalToSafePlace(inFrontOfDrone);

        // make foundgoal bool to not found
        foundGoal = false;
    }

    /// <summary>
    /// Resets the drone varaibles to previous possitions
    /// </summary>
    void ResetDrone()
    {
        this.droneRb.mass = m_ResetParams.GetWithDefault("mass", 0.2f);
        this.droneRb.drag = m_ResetParams.GetWithDefault("drag", 0.2f);
        this.droneRb.angularDrag = m_ResetParams.GetWithDefault("angular_drag", 0.1f);
        float scale = m_ResetParams.GetWithDefault("scale", 1f);
        this.transform.localScale = new Vector3(scale, scale, scale);
        this.droneRb.rotation = Quaternion.identity;
        this.droneRb.velocity = Vector3.zero;
        this.droneRb.angularVelocity = Vector3.zero;
        environment.MoveDroneToSafePlace();
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
        this.droneMovement.frontLeftProp.clockWise = leftDiagonal;
        this.droneMovement.backRightProp.clockWise = leftDiagonal;
        this.droneMovement.frontRightProp.clockWise = !leftDiagonal;
        this.droneMovement.backLeftProp.clockWise = !leftDiagonal;
    }

    /// <summary>
    /// Called when the collider of the gameobject detects a collision with other object
    /// </summary>
    /// <param name="other"></param>
    private void OnCollisionEnter(Collision other)
    {
        AddReward(-1f);

        // stabilize the drone
        droneRb.velocity = Vector3.zero;
        droneRb.angularVelocity = Vector3.zero;
        this.EndEpisode();
    }

    /// <summary>
    /// Called when the <see cref="Collider"> of this object touches a isTriggered collider 
    /// </summary>
    /// <param name="other">Other game object's collider</param>
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Goal") && !foundGoal)
        {
            foundGoal = true;
            StartCoroutine(MoveGoalToNewPlace());
        }
    }

    /// <summary>
    /// Called when this <see cref="Collider"> of this object stays inside a other collider which is isTriggered
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Goal"))
        {
            // Calculate the reward for staying inside the target location
            float bonus = 0.02f * (1 - Mathf.Clamp01(Vector3.Distance(this.transform.position, target.position)));

            // add small reward for staying in the goal position
            AddReward(0.01f + bonus);
        }
    }

    private IEnumerator MoveGoalToNewPlace()
    {
        // waitf for 3 secs before assigning the goal gameobject a new position
        yield return new WaitForSeconds(3f);

        // moves the goal to a new safe place
        bool moveInFrontOfDrone = 0.6f < UnityEngine.Random.Range(0f, 1f);
        environment.MoveGoalToSafePlace(moveInFrontOfDrone);
        foundGoal = false;
    }

    /// <summary>
    /// Called for each frame
    /// </summary>
    private void Update()
    {
        if (debugMode)
        {
            Vector3 toGoalDirection = target.position - this.transform.position;
            Debug.DrawLine(this.transform.position, this.transform.position + toGoalDirection.normalized, Color.green);
        }
    }
}
