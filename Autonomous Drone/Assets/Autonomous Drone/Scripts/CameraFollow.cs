using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    /// <summary>
    /// The tranform of the target object
    /// </summary>
    [Tooltip("The tranform of the target object")]
    public Transform target;

    /// <summary>
    /// The amount of distance the cam should be from the target object
    /// </summary>
    [Tooltip("The amount of distance the cam should be from the target object")]
    public float offset;

    /// <summary>
    /// The altitude of the camera relative to the target object
    /// </summary>
    [Tooltip("The altitude of the camera relative to the target object")]
    public float altitudeOffset;

    /// <summary>
    /// The smoothness of the movement of camera
    /// </summary>
    [Tooltip("The smoothness of the movement of camera")]
    [Range(0f, 5f)]
    public float smooth;

    private void Start()
    {
        this.transform.LookAt(target);
    }

    private void Update()
    {
        Vector3 targetPosition = new Vector3(target.position.x, target.position.y + altitudeOffset, target.position.z - offset);
        this.transform.position = Vector3.Lerp(this.transform.position, targetPosition, Time.deltaTime * smooth);
        this.transform.LookAt(target);
    }
}
