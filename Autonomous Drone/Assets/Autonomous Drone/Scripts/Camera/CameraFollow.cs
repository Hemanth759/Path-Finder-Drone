using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{

    /// <summary>
    /// The amount of distance the cam should be from the target object
    /// </summary>
    [Tooltip("The amount of distance the cam should be from the target object")]
    [SerializeField]
    private float offset = 5;

    /// <summary>
    /// The altitude of the camera relative to the target object
    /// </summary>
    [Tooltip("The altitude of the camera relative to the target object")]
    [SerializeField]
    private float altitudeOffset = 2;

    /// <summary>
    /// The smoothness of the movement of camera
    /// </summary>
    [Tooltip("The smoothness of the movement of camera")]
    [Range(0f, 25f)]
    [SerializeField]
    private float smooth = 10;

    /// <summary>
    /// The tranform of the target object
    /// </summary>
    [Tooltip("The tranform of the target object")]
    [SerializeField]
    private Transform target = null;

    /// <summary>
    /// Called when the object is initalized
    /// </summary>
    private void Start()
    {
        this.transform.rotation = target.rotation;
    }

    /// <summary>
    /// Called right after the update method
    /// </summary>
    private void LateUpdate()
    {
        // apply position of the camera
        Vector3 targetPosition = target.position;
        targetPosition = targetPosition - target.forward * offset;
        targetPosition = targetPosition + Vector3.up * altitudeOffset;
        this.transform.position = Vector3.Lerp(this.transform.position, targetPosition, Time.deltaTime * smooth);

        // apply rotation of the camera
        Quaternion toRot = Quaternion.LookRotation(target.position - this.transform.position, target.up);
        this.transform.rotation = Quaternion.Slerp(this.transform.rotation, toRot, smooth * Time.deltaTime);
    }
}
