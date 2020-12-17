using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stabilize : MonoBehaviour
{
    // public varaibles
    /// <summary>
    /// The rigidbody of the drone
    /// </summary>
    private Rigidbody droneRb;
    private Transform droneTf;

    // Start is called before the first frame update
    void Start()
    {
        droneRb = this.GetComponent<Rigidbody>();
        droneTf = this.transform;
    }

    // Update is called once per frame
    void Update()
    {

    }
}
