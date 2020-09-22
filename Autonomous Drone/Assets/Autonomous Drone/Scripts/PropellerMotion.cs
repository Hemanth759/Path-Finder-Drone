using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PropellerMotion : MonoBehaviour
{
    [Tooltip("True if the propeller should rotate clockwise")]
    public bool clockWise;

    [Tooltip("The rotation speed of the propeller")]
    [SerializeField]
    private float rotateSpeed = 50f;
    private Transform propTf;
    private Rigidbody propRb;

    /// <summary>
    /// Called when the gameobject is initialized
    /// </summary>
    private void Start()
    {
        propTf = this.transform;
        propRb = this.GetComponent<Rigidbody>();
    }

    private void Update()
    {
        if (clockWise)
            propTf.Rotate(Vector3.forward * Time.deltaTime * rotateSpeed);
        else
            propTf.Rotate(-Vector3.forward * Time.deltaTime * rotateSpeed);
    }
}
