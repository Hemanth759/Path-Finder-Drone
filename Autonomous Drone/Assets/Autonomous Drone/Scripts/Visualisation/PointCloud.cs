﻿using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Script for updating a particle system.
/// </summary>
public class PointCloud : MonoBehaviour
{
    public GameObject particleGameObject;
    public GameObject pointCloudBase;

    private int maxParticleSystems = 50;
    private int maxParticlesPerCloud = 500; // maximum number of particles in a particle system
    private float particleSize = 0.1f;
    // private bool isEnabled = false;
    private int usedParticleSystem = 0;
    private int currentNumberOfSystems;
    private Dictionary<int, ParticleSystem> particleSystemIdMap;
    private Stack<List<ParticleSystem.Particle>> particleListPool;
    private List<ParticleSystem.Particle> particleBuffer; //allows for reusing particles
    private ParticleSystem.Particle[] oldParticles;
    private List<ParticleSystem.Particle> particleCloud;

    /// <summary>
    /// Initialization
    /// </summary>
    void Awake()
    {
        particleSystemIdMap = new Dictionary<int, ParticleSystem>();
        pointCloudBase = GameObject.FindGameObjectWithTag("PointCloudBase");
        currentNumberOfSystems = 0;
        CreateNeededParticleSystems();
        LidarSensor.OnPassLidarValuesToPointCloud += UpdateSpecs;
        particleBuffer = new List<ParticleSystem.Particle>();
        particleListPool = new Stack<List<ParticleSystem.Particle>>();
        oldParticles = new ParticleSystem.Particle[maxParticlesPerCloud];
        particleCloud = new List<ParticleSystem.Particle>();

    }

    void OnDestroy()
    {
        LidarSensor.OnScanned -= OnUpdatePoints;
        LidarSensor.OnPassLidarValuesToPointCloud -= UpdateSpecs;
    }


    //TODO: Find a way to fill each system before next iteration. 

    /// <summary>
    /// Creates an array of Shuriken Particles for the lidar sensor hits.
    /// </summary>
    /// <param name="positions"></param>
    /// <returns></returns>
    private void UpdateParticle(LinkedList<SphericalCoordinate> positions)
    {

        if (particleListPool.Count > 0)
        {
            particleCloud = particleListPool.Pop();
        }
        else
        {
            particleCloud = new List<ParticleSystem.Particle>();
        }

        ParticleSystem currentParticleSystem = particleSystemIdMap[usedParticleSystem];

        oldParticles = new ParticleSystem.Particle[currentParticleSystem.particleCount];

        if (currentParticleSystem.particleCount <= maxParticlesPerCloud)
        {
            currentParticleSystem.GetParticles(oldParticles);
            particleCloud.AddRange(oldParticles);
        }
        else
        {
            currentParticleSystem.GetParticles(oldParticles);
            particleBuffer.AddRange(oldParticles);
        }
        foreach (var coordinate in positions)
        {
            ParticleSystem.Particle particle;
            if (particleBuffer.Count > 0)
            {
                particle = particleBuffer[0];
                particleBuffer.RemoveAt(0);
            }
            else
            {
                particle = new ParticleSystem.Particle();
            }
            particle.position = coordinate.ToCartesian();
            if (coordinate.GetRadius() < 5)
            {
                particle.startColor = Color.red;

            }
            else if (coordinate.GetRadius() > 5 && coordinate.GetRadius() < 10)
            {
                particle.startColor = Color.yellow;

            }
            else
            {
                particle.startColor = Color.green;

            }

            particle.startSize = particleSize;
            particle.startLifetime = 1500f;
            particle.remainingLifetime = 3000f;
            particleCloud.Add(particle);

        }
        particleSystemIdMap[usedParticleSystem].SetParticles(particleCloud.ToArray(), particleCloud.Count);
        particleCloud.Clear();
        particleListPool.Push(particleCloud);

    }

    /// <summary>
    /// Updates the points to be added to the point cloud (the latest from the lidar sensor)
    /// </summary>
    /// <param name="points"></param>
    public void OnUpdatePoints(float time, LinkedList<SphericalCoordinate> points)
    {
        if (particleSystemIdMap[usedParticleSystem].particleCount > maxParticlesPerCloud)
        {
            usedParticleSystem = (usedParticleSystem + 1) % maxParticleSystems;
        }
        UpdateParticle(points);

    }

    void OnDisable()
    {
        Pause();
    }
    void OnEnable()
    {
        Play();
    }

    /// <summary>
    /// Resumes the point cloud after a pause
    /// </summary>
    public void Play()
    {
        LidarSensor.OnScanned += OnUpdatePoints;
        // isEnabled = true;
        foreach (var entity in particleSystemIdMap)
        {
            entity.Value.Play();
        }

    }


    /// <summary>
    /// Pauses the visualization
    /// </summary>
    public void Pause()
    {
        LidarSensor.OnScanned -= OnUpdatePoints;
        // isEnabled = false;          
        foreach (var entity in particleSystemIdMap)
        {
            entity.Value.Clear();
            entity.Value.Pause();
        }

    }

    /// <summary>
    /// This method is called when the lidar specifications are changed. Calculates the number of particle systems needed, point size and number of particles / system.
    /// </summary>
    public void UpdateSpecs(int numberOfLasers, float rotationSpeed, float rotationAnglePerStep)
    {
        int maxNumParticlesPerLap = (int)Mathf.Ceil((360 * numberOfLasers) / rotationAnglePerStep); // maximum number of raycast hits per lap

        if (maxNumParticlesPerLap < 250000)
        {
            particleSize = 0.1f;

        }
        else if (maxNumParticlesPerLap < 750000)
        {
            particleSize = 0.05f;
        }
        else
        {
            particleSize = 0.01f;
        }

        int newMaxparticleSystems = (int)Mathf.Ceil((float)maxNumParticlesPerLap / (float)maxParticlesPerCloud);

        if (this.maxParticleSystems < newMaxparticleSystems)
        {
            maxParticleSystems = newMaxparticleSystems;
            CreateNeededParticleSystems();
        }

    }


    /// <summary>
    /// Creates the needed number of particle systems.
    /// </summary>
    private void CreateNeededParticleSystems()
    {
        for (int i = currentNumberOfSystems; i < maxParticleSystems; i++)
        {
            GameObject newGO = Instantiate(particleGameObject, pointCloudBase.transform.position, Quaternion.identity);
            newGO.name = "pSyst" + i;
            ParticleSystem p = newGO.GetComponent<ParticleSystem>();
            p.transform.SetParent(GameObject.Find("ParticleSystems").transform);
            particleSystemIdMap.Add(i, p);
            currentNumberOfSystems++;
        }

    }

}
