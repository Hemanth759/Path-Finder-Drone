using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class PropellerMotion : MonoBehaviour
{
    [Tooltip("True if the propeller should rotate clockwise")]
    public bool clockWise;

    public float AddForce
    {
        set
        {
            addForce = value;
            lineRenderer.SetPosition(1, Vector3.forward + Vector3.forward * 0.5f * addForce);
        }
    }

    [Tooltip("The rotation speed of the propeller")]
    [SerializeField]
    private readonly float baseRotateSpeed = 1500;
    private Transform propTf;
    private LineRenderer lineRenderer;
    private float addForce;

    /// <summary>
    /// Called when the gameobject is initialized
    /// </summary>
    private void Start()
    {
        propTf = this.transform;
        lineRenderer = this.GetComponent<LineRenderer>();
        addForce = 0f;
    }

    private void Update()
    {
        RotatePropeller();
        ExtendLine();
        ResetRotationSpeed();
    }

    void ExtendLine()
    {
        Vector3 currentEnd = lineRenderer.GetPosition(1);
        lineRenderer.SetPosition(1, Vector3.Lerp(currentEnd, Vector3.forward, Time.deltaTime));
    }

    void RotatePropeller()
    {
        if (clockWise)
            propTf.Rotate(Vector3.forward * Time.deltaTime * (baseRotateSpeed + (addForce * 500f)));
        else
            propTf.Rotate(-Vector3.forward * Time.deltaTime * (baseRotateSpeed + (addForce * 500f)));
    }

    void ResetRotationSpeed()
    {
        addForce = 0f;
    }
}
