using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainingEnvironment : MonoBehaviour
{
    /// <summary>
    /// Drone present in the envirnment
    /// </summary>
    [Tooltip("Drone present in the envirnment")]
    public Transform droneTf;

    /// <summary>
    /// Goal tranform
    /// </summary>
    [Tooltip("Goal tranform")]
    public Transform goalTf;

    /// <summary>
    /// Places the drone in a safe place 
    /// where no object is colliding with drone collider
    /// </summary>
    public void MoveDroneToSafePlace()
    {
        FindSafePositionAndMove(droneTf, 15f, 45f, 50f, 50f);
    }

    /// <summary>
    /// Move the goal game tranform to a random 
    /// safe location in the terrain
    /// </summary>
    public void MoveGoalToSafePlace(bool inFrontOfDrone)
    {
        if (inFrontOfDrone)
        {
            FindSafePositionAndMove(goalTf, 7.5f, 1.5f, droneTf.position.x, droneTf.position.y);
        }
        else
        {
            FindSafePositionAndMove(goalTf, 7.5f, 45f, 50f, 50f);
        }
    }

    /// <summary>
    /// Move the objTf to a safe location in the terrain
    /// </summary>
    /// <param name="objTf">The object tranform that needs to be moved to a new random safe location</param>
    /// <param name="maxHeight">The maximum possible y value of the object's <see cref="Transform"> can take</param>
    /// <param name="maxRadius">The maximum possible radius the object's <see cref="Transform"> can go from the terrain center in the xz plane</param>
    private void FindSafePositionAndMove(Transform objTf, float maxHeight, float maxRadius, float xOffset, float zOffset)
    {
        bool sagePositionFound = false;
        int attemptsRemaining = 100; // Prevents the infinite loop
        Vector3 potentialPosition = Vector3.zero;
        Quaternion potentialRotation = new Quaternion();

        // Loop until safe place is found or we run out of attempts
        while (!sagePositionFound && attemptsRemaining > 0)
        {
            --attemptsRemaining;
            // Pick a random height from the ground 
            float height = UnityEngine.Random.Range(2f, maxHeight);

            // Pick a random radius from the center of the area
            float radius = UnityEngine.Random.Range(0f, maxRadius);

            // Pick a random rotation rotated around the Y axis
            Vector3 direction = new Vector3(UnityEngine.Random.Range(-1f, 1f), 0f, UnityEngine.Random.Range(-1f, 1f));

            // Combine height, radius and direction to pick a potential position
            potentialPosition = new Vector3(xOffset, 0f, zOffset) + this.transform.position + Vector3.up * height + direction * radius;

            // Choose and set random starting pitch and yaw
            float pitch = UnityEngine.Random.Range(-60f, 60f);
            float yaw = UnityEngine.Random.Range(-180f, 180f);
            potentialRotation = Quaternion.Euler(pitch, yaw, 0f);


            // Check if the potential position and rotation are no colliding with any game object
            Collider[] colliders = Physics.OverlapSphere(potentialPosition, 2f);

            // Safe posiion found if no colliders are found
            sagePositionFound = colliders.Length == 0;
        }

        Debug.Assert(sagePositionFound, "Cound not find a safe position to spawn");

        // Set the position and rotation
        objTf.position = potentialPosition;
        objTf.rotation = potentialRotation;
    }
}
