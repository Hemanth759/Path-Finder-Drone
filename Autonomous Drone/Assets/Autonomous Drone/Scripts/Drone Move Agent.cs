using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;

public class DroneMoveAgent : Agent
{
    // public variables
    [Header("Propeller motors")]


    [Space]

    [Header("Movement Variables")]
    [Tooltip("The speed of the drone movement")]
    public float moveSpeed;

    // private variables



    /// <summary>
    /// Called when the scene is initialied
    /// </summary>
    public override void Initialize()
    {
        base.Initialize();
    }

    /// <summary>
    /// Called when the episode begins
    /// </summary>
    public override void OnEpisodeBegin()
    {
        base.OnEpisodeBegin();
    }

    /// <summary>
    /// Collects the observations to the sensor and 
    /// transfers the observations to input of the neural networks
    /// </summary>
    /// <param name="sensor">The sensor component of the agent</param>
    public override void CollectObservations(VectorSensor sensor)
    {
        base.CollectObservations(sensor);

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

    }

    /// <summary>
    /// When behaviour type is set to "heuristic only" on the agent's Behaviour parameters,
    /// this function will be called. Its return values will be fed into 
    /// <see cref="OnActionReceived(float[])"> instead of using the neural networks. 
    /// </summary>
    /// <param name="actionsOut"></param>
    public override void Heuristic(float[] actionsOut)
    {
        base.Heuristic(actionsOut);

        float altitude = 0f;      // +1 = gain altitude, -1 = lose altitude

        // press w to gain height or s to loss height
        if (Input.GetKey(KeyCode.W))
        {
            altitude = 1f;
        }
        else if (Input.GetKey(KeyCode.S))
        {
            altitude = -1f;
        }

        float yaw = 0f;         // +1 = turn left, -1 = turn right

        // press a to turn left or d to turn right
        if (Input.GetKey(KeyCode.A))
        {
            yaw = 1f;
        }
        else if (Input.GetKey(KeyCode.D))
        {
            yaw = -1f;
        }

        float pitch = 0f;       // +1 to tilt forward, -1 to tilt backward
        // press num 8 to tilt forward or num 2 to tilt backward
        if (Input.GetKey(KeyCode.Keypad8))
        {
            pitch = 1;
        }
        else if (Input.GetKey(KeyCode.Keypad2))
        {
            pitch = -1;
        }

        float roll = 0f;        // +1 to roll left, -1 to tilt right
        // press num 4 to roll left or num 6 to roll right
        if (Input.GetKey(KeyCode.Keypad4))
        {
            roll = 1;
        }
        else if (Input.GetKey(KeyCode.Keypad6))
        {
            roll = -1;
        }

        if (altitude > 0)
        {

        }
    }
}
