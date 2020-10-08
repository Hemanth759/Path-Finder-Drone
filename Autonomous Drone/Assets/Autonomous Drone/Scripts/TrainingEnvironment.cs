using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class TrainingEnvironment : MonoBehaviour
{
    public bool save = false;

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
    /// object which stores the data collected by lidar sensor
    /// </summary>
    private LidarStorage lidarStorage;
    public GameObject lidarStorageGameObject;

    private Terrain terrain;
    private float maxDroneSpawnHeight = 5.5f;
    private float maxGoalSpawnHeight = 5.5f;
    private float spawnOffSet = 40f;
    private float TerrainCenterOffsetX = 50f;
    private float TerrainCenterOffsetY = 50f;
    private readonly float nearSpawnMaxRadius = 5f;

    /// <summary>
    /// Called at the start of the scene
    /// </summary>
    private void Start()
    {
        /*if (displayCloudPoints)
        {
            // ExportManager.Loading += externalVisualization.Loading;
            LidarStorage.HaveData += DataExists;

            filepath = Application.dataPath + "/../../../cloudpoints/cloudpoints.txt";
            exportManager = GetComponent<ExportManager>();
            exportManager.Open(filepath);
        }
        else
        {*/
            terrain = this.GetComponent<Terrain>();
            lidarStorageGameObject = GameObject.FindGameObjectWithTag("Lidar");
            lidarStorage = lidarStorageGameObject.GetComponent<LidarStorage>();
        /*}*/
    }

    /// <summary>
    /// Called when the application is closed
    /// </summary>
    void OnApplicationQuit()
    {
        /*if (!displayCloudPoints)
        {*/
            string _dir_ = Application.dataPath + "/../../../cloudpoints";
            string path = _dir_ + "/cloudpoints.txt";
            /*for (int i = 1; System.IO.File.Exists(path); i++)
            {
                path = _dir_ + "/cloudpoints" + i + ".txt";
            }*/

            if (save)
            {
                Save(path);
            }
        /*}*/
    }

    /// <summary>
    /// Saves to the given filepath
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
    public void MoveDroneToSafePlace()
    {
        FindSafePositionAndMove(droneTf, maxDroneSpawnHeight, spawnOffSet, TerrainCenterOffsetX, TerrainCenterOffsetY);
    }

    /// <summary>
    /// Move the goal game tranform to a random 
    /// safe location in the terrain
    /// </summary>
    public void MoveGoalToSafePlace(bool inFrontOfDrone)
    {
        if (inFrontOfDrone)
        {
            FindSafePositionAndMove(goalTf, maxGoalSpawnHeight, nearSpawnMaxRadius, droneTf.position.x, droneTf.position.y);
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
        bool sagePositionFound = false;
        int attemptsRemaining = 200; // Prevents the infinite loop
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
            potentialPosition = new Vector3(xOffset, 0f, zOffset) + this.transform.position + direction * radius;
            potentialPosition += Vector3.up * (height);

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
