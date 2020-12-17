using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(DroneAgent))]
public class DroneController : MonoBehaviour
{
    /// <summary>
    /// The drone agent script of the drone object
    /// </summary>
    private DroneAgent droneAgent;

    // Start is called before the first frame update
    void Start()
    {
        droneAgent = this.GetComponent<DroneAgent>();
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        float[] actions = new float[4];
        droneAgent.Heuristic(actions);
        droneAgent.OnActionReceived(actions);
    }
}
