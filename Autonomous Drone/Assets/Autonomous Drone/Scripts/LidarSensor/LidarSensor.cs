using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Simulates the lidar sensor by using ray casting.
/// </summary>
public class LidarSensor : MonoBehaviour
{
    public GameObject pointCloud;
    public GameObject mainCamera;

    private float lastUpdate = 0;

    private List<Laser> lasers;
    private float horizontalAngle = 0;

    public int numberOfLasers = 64;
    public float rotationSpeedHz = 1.0f;
    public float rotationAnglePerStep = 0.9f;
    public float rayDistance = 120f;
    public float upperFOV = 10.5f;
    public float lowerFOV = 16f;
    public float offset = 2.85f;
    public float upperNormal = -3.3f;
    public float lowerNormal = 16.9f;
    public static event NewPoints OnScanned;
    public delegate void NewPoints(float time, LinkedList<SphericalCoordinate> data);
    LinkedList<SphericalCoordinate> hits;

    public delegate void PassLidarValuesToPointCloudDelegate(int numberOfLasers, float rotationSpeed, float rotationAnglePerStep);
    public delegate void UpdateMimicValuesDelegate(int noLasers, float upFOV, float lowFOV, float offset, float upNorm, float lowNorm);

    public static event PassLidarValuesToPointCloudDelegate OnPassLidarValuesToPointCloud;
    public static event UpdateMimicValuesDelegate OnPassValuesToLaserMimic;

    public float lapTime = 0;

    // -- private bool isPlaying = false;
    // private bool laserPreviewInitialized = false;

    public GameObject pointCloudObject;
    private float previousUpdate;

    private float lastLapTime;

    public GameObject lineDrawerPrefab;

    // Initialization
    private void Start()
    {
        // -- LidarMenu.OnPassValuesToLidarSensor += UpdateSettings;
        // -- PlayButton.OnPlayToggled += PauseSensor;
        lastLapTime = 0;
        PreviewLidarRays.tellLidarMenuInitialized += LaserPreviewIsInitialized;
        hits = new LinkedList<SphericalCoordinate>();
        InitiateLasers();
        TogglePointCloudActive();
    }

    void OnDestroy()
    {
        // -- LidarMenu.OnPassValuesToLidarSensor -= UpdateSettings;
        // -- PlayButton.OnPlayToggled -= PauseSensor;
        PreviewLidarRays.tellLidarMenuInitialized -= LaserPreviewIsInitialized;
    }

    /*public void UpdateSettings(int numberOfLasers, float rotationSpeedHz, float rotationAnglePerStep, float rayDistance, float upperFOV,
        float lowerFOV, float offset, float upperNormal, float lowerNormal)
    {
        this.numberOfLasers = numberOfLasers;
        this.rotationSpeedHz = rotationSpeedHz;
        this.rotationAnglePerStep = rotationAnglePerStep;
        this.rayDistance = rayDistance;
        this.upperFOV = upperFOV;
        this.lowerFOV = lowerFOV;
        this.offset = offset;
        this.upperNormal = upperNormal;
        this.lowerNormal = lowerNormal;

        InitiateLasers();
    }*/

    private void InitiateLasers()
    {
        // Initialize number of lasers, based on user selection.
        if (lasers != null)
        {
            foreach (Laser l in lasers)
            {
                Destroy(l.GetRenderLine().gameObject);
            }
        }

        lasers = new List<Laser>();

        float upperTotalAngle = upperFOV / 2;
        float lowerTotalAngle = lowerFOV / 2;
        float upperAngle = upperFOV / (numberOfLasers / 2);
        float lowerAngle = lowerFOV / (numberOfLasers / 2);
        offset = (offset / 100) / 2; // Convert offset to centimeters.
        for (int i = 0; i < numberOfLasers; i++)
        {
            GameObject lineDrawer = Instantiate(lineDrawerPrefab);
            lineDrawer.transform.parent = gameObject.transform; // Set parent of drawer to this gameObject.
            if (i < numberOfLasers / 2)
            {
                lasers.Add(new Laser(gameObject, lowerTotalAngle + lowerNormal, rayDistance, -offset, lineDrawer, i));

                lowerTotalAngle -= lowerAngle;
            }
            else
            {
                lasers.Add(new Laser(gameObject, upperTotalAngle - upperNormal, rayDistance, offset, lineDrawer, i));
                upperTotalAngle -= upperAngle;
            }
        }

        // -- isPlaying = true;
    }

    /*public void PauseSensor(bool simulationModeOn)
    {
        if (!simulationModeOn)
        {
            isPlaying = simulationModeOn;
        }
    }*/

    /// <summary>
    /// Gets called from the "PreviewLidarRays" script when the script has been initialized.
    /// If the "LidarMenu" script has initialized the values of the GUI, it sends the values back to the preview.
    /// </summary>
    /// <param name="isInitialized"></param>
    void LaserPreviewIsInitialized(bool isInitialized)
    {
        // laserPreviewInitialized = true;
        UpdateLaserMimicValues();
    }

    // Update is called once per frame
    private void Update()
    {
        // For debugging, shows visible ray in real time.
        /*
        foreach (Laser laser in lasers)
        {
            laser.DebugDrawRay();
        }
        */
    }

    private void FixedUpdate()
    {
        hits = new LinkedList<SphericalCoordinate>();
        // Do nothing, if the simulator is paused.
        // -- if (!isPlaying)
        // -- {
        // --     return;
        // -- }

        // Check if number of steps is greater than possible calculations by unity.
        float numberOfStepsNeededInOneLap = 360 / Mathf.Abs(rotationAnglePerStep);
        float numberOfStepsPossible = 1 / Time.fixedDeltaTime / 5;
        float precalculateIterations = 1;
        // Check if we need to precalculate steps.
        if (numberOfStepsNeededInOneLap > numberOfStepsPossible)
        {
            precalculateIterations = (int)(numberOfStepsNeededInOneLap / numberOfStepsPossible);
            if (360 % precalculateIterations != 0)
            {
                precalculateIterations += 360 % precalculateIterations;
            }
        }

        // Check if it is time to step. Example: 2hz = 2 rotations in a second.
        if (Time.fixedTime - lastUpdate > (1 / (numberOfStepsNeededInOneLap) / rotationSpeedHz) * precalculateIterations)
        {
            // Update current execution time.
            lastUpdate = Time.fixedTime;

            for (int i = 0; i < precalculateIterations; i++)
            {
                // Perform rotation.
                transform.Rotate(0, rotationAnglePerStep, 0);
                horizontalAngle += rotationAnglePerStep; // Keep track of our current rotation.
                if (horizontalAngle >= 360)
                {
                    horizontalAngle -= 360;
                    //GameObject.Find("RotSpeedText").GetComponent<Text>().text =  "" + (1/(Time.fixedTime - lastLapTime));
                    lastLapTime = Time.fixedTime;

                }


                // Execute lasers.
                foreach (Laser laser in lasers)
                {
                    RaycastHit hit = laser.ShootRay();
                    float distance = hit.distance;
                    if (distance != 0 && distance > 3) // Didn't hit anything, don't add to list or it hit the drone itself.
                    {
                        float verticalAngle = laser.GetVerticalAngle();
                        hits.AddLast(new SphericalCoordinate(distance, verticalAngle, horizontalAngle, hit.point, laser.GetLaserId()));
                    }
                }
            }


            // Notify listeners that the lidar sensor have scanned points. 
            //if (OnScanned != null  && pointCloudObject != null && pointCloudObject.activeInHierarchy)
            //{
            OnScanned?.Invoke(lastLapTime, hits);
            //}

        }
    }

    /// <summary>
    /// Invokes an event to sync the values of the lidar preview with the values of the Lidar menu GUI
    /// </summary>
    void UpdateLaserMimicValues()
    {
        try
        {
            OnPassValuesToLaserMimic((int)numberOfLasers, upperFOV, lowerFOV, offset, upperNormal, lowerNormal);
        }
        catch (NullReferenceException e)
        {
            Debug.Log("Event has no delegates: " + e);
        }
    }

    /// <summary>
    /// Invokes an event to sync the point clouds settings with the values of the lidar menu GUI
    /// </summary>
    void PassLidarValuesToPointCloud()
    {
        try
        {
            OnPassLidarValuesToPointCloud((int)numberOfLasers, rotationSpeedHz, rotationAnglePerStep);
        }
        catch (NullReferenceException e)
        {
            Debug.Log("Event has no delegates: " + e);
        }
    }

    /// <summary>
    /// Toggles the split screen visualisation through handling the activity of the point cloud and sets the main camera size to split or non split screen size
    /// </summary>
    public void TogglePointCloudActive()
    {
        pointCloud.SetActive(!pointCloud.activeInHierarchy);

        if (!pointCloud.activeInHierarchy)
        {
            mainCamera.GetComponent<Camera>().rect = new Rect(0, 0, 1, 1);
        }
        else
        {
            mainCamera.GetComponent<Camera>().rect = new Rect(0, 0, 0.5f, 1);
            PassLidarValuesToPointCloud();
        }

        mainCamera.GetComponent<Camera>().enabled = true;
    }
}
