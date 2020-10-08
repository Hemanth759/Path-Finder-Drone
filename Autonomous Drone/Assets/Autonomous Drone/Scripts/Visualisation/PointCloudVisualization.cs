using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;
using UnityEngine;
using UnityEditor;

public class PointCloudVisualization : MonoBehaviour
{
    /// <summary>
    /// object which stores the data collected by lidar sensor
    /// </summary>
    private LidarStorage lidarStorage;
    public GameObject lidarStorageGameObject;
    public string filepath;

    private ExternalPointCloud externalPointCloud;
    private Dictionary<float, List<LinkedList<SphericalCoordinate>>> pointTable;

    // Start is called before the first frame update
    void Start()
    {
        lidarStorageGameObject = GameObject.FindGameObjectWithTag("Lidar");
        lidarStorage = lidarStorageGameObject.GetComponent<LidarStorage>();

        // ExportManager.Loading += externalVisualization.Loading;
        LidarStorage.HaveData += DataExists;
        filepath = Application.dataPath + "/../../../cloudpoints/cloudpoints.txt";
        Open(filepath);
    }

    /// <summary>
    /// Opens the given file
    /// </summary>
    /// <param name="filePath"></param>
    public void Open(string filePath)
    {
        Coroutine async = StartCoroutine(LoadCsv(filePath, lidarStorage));
        /*if (Loading != null)
        {
            Loading(async);
        }*/
    }

    /// <summary>
    /// Creates a single linked list filled with spherical coordinates from a data tablöe
    /// </summary>
    /// <returns></returns>
    private LinkedList<SphericalCoordinate> SquashTable(Dictionary<float, List<LinkedList<SphericalCoordinate>>> data)
    {
        Debug.Log("Squashing table for: " + data.Count);
        LinkedList<SphericalCoordinate> newList = new LinkedList<SphericalCoordinate>();

        foreach (var list in data.Values)
        {
            foreach (var entity in list)
            {
                foreach (SphericalCoordinate s in entity)
                {
                    newList.AddLast(s);
                }
            }
        }
        return newList;
    }

    /// <summary>
    /// Is called when the data structure have data, in order to load the data to the visualization when it have been loaded in the data structure
    /// </summary>
    public void DataExists()
    {
        this.pointTable = lidarStorage.GetData();
        //Set Camera 
        foreach (var v in pointTable.Values)
        {
            foreach (var c in v)
            {
                Vector3 firstCoordinate = v[0].First.Value.GetWorldCoordinate();
                GameObject.Find("Main Camera").GetComponent<Camera>().transform.position = new Vector3(firstCoordinate.x - 5, 10, firstCoordinate.y);
                break;
            }

        }
        externalPointCloud = GetComponent<ExternalPointCloud>();
        externalPointCloud.CreateCloud(SquashTable(pointTable));
    }

    /// <summary>
    /// loads cloud points from the file specified
    /// </summary>
    /// <param name="filename">cloud points sotred file</param>
    /// <param name="storage">LidarStorage Component</param>
    /// <returns></returns>
    public static IEnumerator LoadCsv(string filename, LidarStorage storage)
    {
        StreamReader sr = new StreamReader(File.OpenRead(filename));
        Dictionary<float, List<LinkedList<SphericalCoordinate>>> data = new Dictionary<float, List<LinkedList<SphericalCoordinate>>>();
        // bool internalData = false; //The data to be read was created by our program


        // First line starts with "sep= " internal representation.
        for (int i = 0; i < 2; i++)
        {
            sr.ReadLine(); // Discard first two lines
        }
        while (!sr.EndOfStream)
        {
            try
            {
                float key = 0;



                List<float> values = new List<float>();
                LinkedList<SphericalCoordinate> coorValues = new LinkedList<SphericalCoordinate>();
                string[] columns = sr.ReadLine().Split(' ');

                try
                {
                    if (columns.Length == 8)
                    {
                        key = float.Parse(columns[0]);
                        float x = float.Parse(columns[1]);
                        float z = float.Parse(columns[2]);
                        float y = float.Parse(columns[3]);
                        float radius = float.Parse(columns[4]);
                        float inclination = float.Parse(columns[5]);
                        float azimuth = float.Parse(columns[6]);
                        int laserId = int.Parse(columns[7]);
                        Vector3 vector = new Vector3(x, y, z);
                        SphericalCoordinate sc = new SphericalCoordinate(radius, inclination, azimuth, vector, laserId);
                        coorValues.AddLast(sc);
                    }
                }
                catch (FormatException e)
                {
                    Debug.Log("Exception! |time|radius|inclination|azimuth" + "|" + columns[0] + "|" + columns[2] + "|" + columns[3] + "|" + columns[4] + " \nwith error: " + e.Message);
                }
                catch (ArgumentOutOfRangeException e)
                {
                    Debug.Log("Length:" + columns.Length + " \nwith error: " + e.Message);
                }



                List<LinkedList<SphericalCoordinate>> oldList;
                List<LinkedList<SphericalCoordinate>> dataList = new List<LinkedList<SphericalCoordinate>>();
                dataList.Add(coorValues);
                if (!data.TryGetValue(key, out oldList))
                {
                    data.Add(key, dataList);

                }
                else
                {
                    List<LinkedList<SphericalCoordinate>> existingList = data[key];
                    existingList.Add(coorValues);

                }

            }
            catch (Exception e)
            {
                Debug.Log("Unreadable data: " + e);
            }
        }

        Debug.Log("Setting data in: " + storage.GetHashCode() + " with length" + data.Count);
        storage.SetData(data);

        yield return null;
    }

    /// <summary>
    /// Resets the visualization to it's initial state
    /// </summary>
    private void Reset()
    {
        externalPointCloud.Clear();
    }
}
