using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class TrainingEnvironment : MonoBehaviour
{
    /// <summary>
    /// Whether to save the points to cloud or not
    /// </summary>
    [Tooltip("Whether to save the points to cloud or not")]
    public bool saveToFile = false;

    /// <summary>
    /// object which stores the data collected by lidar sensor
    /// </summary>
    private LidarStorage lidarStorage;
    public GameObject lidarStorageGameObject;

    private Terrain terrain;
    private const float maxDroneSpawnHeight = 15.5f;
    private const float maxGoalSpawnHeight = 10f;
    private const float spawnOffSet = 40f;
    private const float TerrainCenterOffsetX = 50f;
    private const float TerrainCenterOffsetY = 50f;
    private const float nearSpawnMaxRadius = 1f;

    /// <summary>
    /// Called when the scene is initialized
    /// </summary>
    private void Awake()
    {
        terrain = this.GetComponent<Terrain>();
        lidarStorageGameObject = GameObject.FindGameObjectWithTag("Lidar");
        lidarStorage = lidarStorageGameObject.GetComponent<LidarStorage>();
    }

    /// <summary>
    /// Called when the application is closed
    /// </summary>
    void OnApplicationQuit()
    {
        string _dir_ = Application.dataPath + "/../../../cloudpoints";
        string path = _dir_ + "/cloudpoints.txt";

        if (saveToFile)
        {
            Save(path);
        }
    }

    /// <summary>
    /// Saves the cloud points from the lidar sensor to the given filepath
    /// </summary>
    /// <param name="filePath"></param>
    public void Save(string filePath)
    {
        Dictionary<float, List<LinkedList<SphericalCoordinate>>> data = lidarStorage.GetData();

        if (data.Equals(null) || data.Count == 0)
        {
            Debug.Log("No data to save!");
        }
        else
        {
            SaveManager.SaveToCsv(data, filePath);
        }
    }

    /// <summary>
    /// Places the drone in a safe place 
    /// where no object is colliding with drone collider
    /// </summary>
    public void MoveDroneToSafePlace(Transform droneTf)
    {
        FindSafePositionAndMove(droneTf, maxDroneSpawnHeight, spawnOffSet, TerrainCenterOffsetX, TerrainCenterOffsetY);
    }

    /// <summary>
    /// Move the goal game tranform to a random 
    /// safe location in the terrain
    /// </summary>
    public void MoveGoalToSafePlace(bool inFrontOfDrone, Transform goalTf, Transform droneTf)
    {
        if (inFrontOfDrone)
        {
            FindSafePositionAndMove(goalTf, maxGoalSpawnHeight, nearSpawnMaxRadius, droneTf.localPosition.x, droneTf.localPosition.z);
        }
        else
        {
            FindSafePositionAndMove(goalTf, maxGoalSpawnHeight, spawnOffSet, TerrainCenterOffsetX, TerrainCenterOffsetY);
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
        bool safePositionFound = false;
        int attemptsRemaining = 100; // Prevents the infinite loop
        Vector3 potentialPosition = Vector3.zero;
        Quaternion potentialRotation = new Quaternion();

        // Loop until safe place is found or we run out of attempts
        while (!safePositionFound && attemptsRemaining > 0)
        {
            --attemptsRemaining;
            // Pick a random height from the ground 
            float height = UnityEngine.Random.Range(5f, maxHeight);

            // Pick a random radius from the center of the area
            float radius = UnityEngine.Random.Range(0f, maxRadius);

            // Pick a random rotation rotated around the Y axis
            Vector3 direction = new Vector3(UnityEngine.Random.Range(-1f, 1f), 0f, UnityEngine.Random.Range(-1f, 1f));

            // Combine height, radius and direction to pick a potential position
            potentialPosition = new Vector3(xOffset, 0f, zOffset) + (direction * radius);
            potentialPosition += Vector3.up * (height + terrain.SampleHeight(potentialPosition));

            // Choose and set random starting pitch and yaw
            float pitch = UnityEngine.Random.Range(-60f, 60f);
            float yaw = UnityEngine.Random.Range(-180f, 180f);
            potentialRotation = Quaternion.Euler(pitch, yaw, 0f);


            // Check if the potential position and rotation are no colliding with any game object
            Collider[] colliders = Physics.OverlapSphere(potentialPosition, 1f);

            // Safe posiion found if no colliders are found
            safePositionFound = colliders.Length == 0;
        }

        Debug.Assert(safePositionFound, "Cound not find a safe position to spawn");

        // Set the position and rotation
        objTf.localPosition = potentialPosition;
        objTf.localRotation = potentialRotation;
    }
}
