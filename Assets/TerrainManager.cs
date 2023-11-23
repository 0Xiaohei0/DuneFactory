using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

public class TerrainManager : MonoBehaviour
{
    public Terrain terrain; // Assign this in the inspector or via code
    [SerializeField] private float transitionProgress = 0.0f;
    public float transitionSpeed = 0.1f;
    public Vector3 enrichmentPlantPosition;
    public float plantEffectRadius;
    // Example variables for tick-based update
    [SerializeField] private float updateTimer = 0f;
    [SerializeField] private float updateInterval = 5f; // Update every 5 seconds
    private void Start()
    {
        UpdateSplatMap(terrain, enrichmentPlantPosition, 10000, 0);
        Vector3 centerPos = new Vector3(0, 0, 0); // Center position in world space
        SetGrass(terrain, 0, 0);
        //SetGrass(terrain, 0, 150);
        //PrintDetailMapSummary(terrain, 0);
        //GrowGrassInArea(terrain, enrichmentPlantPosition, plantEffectRadius, 0, 150); // Assuming grass is at detail index 0
        //GrowGrassInArea(terrain, enrichmentPlantPosition + new Vector3(0.1f, 0, 0.1f), plantEffectRadius, 0, 16); // Assuming grass is at detail index 0
        //GrowGrassInArea(terrain, enrichmentPlantPosition + new Vector3(0.2f, 0, 0.2f), plantEffectRadius, 0, 16); // Assuming grass is at detail index 0
    }

    void Update()
    {
        updateTimer += Time.deltaTime;
        if (updateTimer >= updateInterval)
        {
            updateTimer = 0f;
            UpdateTerrainAndGrass(terrain, enrichmentPlantPosition, plantEffectRadius, 0, transitionProgress, 5);
        }
        //// Example condition: increment transitionProgress over time
        transitionProgress += Time.deltaTime * transitionSpeed; // transitionSpeed is a variable you define
        transitionProgress = Mathf.Clamp01(transitionProgress); // Ensure value stays between 0 and 1
        //UpdateSplatMap(terrain, enrichmentPlantPosition, plantEffectRadius, transitionProgress);

    }
    void UpdateSplatMap(Terrain terrain, Vector3 centerPoint, float radius, float transitionProgress)
    {
        TerrainData terrainData = terrain.terrainData;
        Vector3 terrainPos = terrain.transform.position;

        int centerX = (int)(((centerPoint.x - terrainPos.x) / terrainData.size.x) * terrainData.alphamapWidth);
        int centerZ = (int)(((centerPoint.z - terrainPos.z) / terrainData.size.z) * terrainData.alphamapHeight);
        int radiusInSplatMap = (int)(radius / terrainData.size.x * terrainData.alphamapWidth);
        print("centerX: " + centerX);
        print("centerZ: " + centerZ);
        print("radiusInSplatMap: " + radiusInSplatMap);
        // Calculate which part of the splatmap we need to modify
        int startX = 0;
        int endX = terrainData.alphamapWidth;
        int startZ = 0;
        int endZ = terrainData.alphamapHeight;


        float[,,] splatmapData = terrainData.GetAlphamaps(startX, startZ, endX - startX, endZ - startZ);
        print("updating splat");
        for (int y = centerZ - radiusInSplatMap; y < centerZ + radiusInSplatMap; y++)
        {
            for (int x = centerX - radiusInSplatMap; x < centerX + radiusInSplatMap; x++)
            {

                int distance = (int)Mathf.Sqrt((x - centerX) * (x - centerX) + (y - centerZ) * (y - centerZ));
                if (distance < radiusInSplatMap)
                {
                    splatmapData[x, y, 0] = 1 - transitionProgress; // Sand
                    splatmapData[x, y, 1] = transitionProgress; // Green
                }


            }
        }

        // Apply the modified splatmap data
        terrainData.SetAlphamaps(startX, startZ, splatmapData);
    }
    void SetGrass(Terrain terrain, int grassIndex, int density)
    {
        TerrainData terrainData = terrain.terrainData;
        Vector3 terrainPos = terrain.transform.position;

        int[,] detailLayer = terrainData.GetDetailLayer(0, 0, terrainData.detailWidth, terrainData.detailHeight, grassIndex);
        for (int y = 0; y <= terrainData.detailHeight; y++)
        {
            for (int x = 0; x <= terrainData.detailWidth; x++)
            {
                if (x < 0 || x >= terrainData.detailWidth || y < 0 || y >= terrainData.detailHeight)
                    continue;
                detailLayer[y, x] = density;
            }
        }
        // Apply the updated detail layer to the terrain
        terrainData.SetDetailLayer(0, 0, grassIndex, detailLayer);
    }
    void UpdateGrassDensity(Terrain terrain, Vector3 centerPoint, float radius, int grassIndex, int grassDensity)
    {
        TerrainData terrainData = terrain.terrainData;
        Vector3 terrainPos = terrain.transform.position;
        print("updating grass");
        // Convert centerWorldPos to terrain local coordinates
        int centerX = (int)(((centerPoint.x - terrainPos.x) / terrainData.size.x) * terrainData.detailWidth);
        int centerY = (int)(((centerPoint.z - terrainPos.z) / terrainData.size.z) * terrainData.detailHeight);
        int radiusInDetailMap = (int)(radius / terrainData.size.x * terrainData.detailWidth);

        int[,] detailLayer = terrainData.GetDetailLayer(0, 0, terrainData.detailWidth, terrainData.detailHeight, grassIndex);

        for (int y = centerY - radiusInDetailMap; y <= centerY + radiusInDetailMap; y++)
        {
            for (int x = centerX - radiusInDetailMap; x <= centerX + radiusInDetailMap; x++)
            {
                if (x < 0 || x >= terrainData.detailWidth || y < 0 || y >= terrainData.detailHeight)
                    continue;

                int distance = (int)Mathf.Sqrt((x - centerX) * (x - centerX) + (y - centerY) * (y - centerY));
                if (distance < radiusInDetailMap)
                {
                    detailLayer[y, x] = grassDensity;
                }
            }
        }

        // Apply the updated detail layer to the terrain
        terrainData.SetDetailLayer(0, 0, grassIndex, detailLayer);
    }
    void PrintDetailMapSummary(Terrain terrain, int detailIndex)
    {
        TerrainData terrainData = terrain.terrainData;
        int[,] detailLayer = terrainData.GetDetailLayer(0, 0, terrainData.detailWidth, terrainData.detailHeight, detailIndex);

        int totalDensity = 0;
        for (int y = 0; y < terrainData.detailHeight; y++)
        {
            for (int x = 0; x < terrainData.detailWidth; x++)
            {
                totalDensity += detailLayer[y, x];
            }
        }

        float averageDensity = (float)totalDensity / (terrainData.detailWidth * terrainData.detailHeight);
        Debug.Log("Average Grass Density: " + averageDensity);
    }
    void UpdateTerrainAndGrass(Terrain terrain, Vector3 centerPoint, float radius, int grassIndex, float transitionProgress, int tickRate)
    {
        // Update splatmap for terrain texture
        UpdateSplatMap(terrain, centerPoint, radius, transitionProgress);

        // Calculate grass density based on transitionProgress
        int grassDensity = (int)(transitionProgress * 150); // Scale density with progress

        // Update grass density in the detail layer
        UpdateGrassDensity(terrain, centerPoint, radius, grassIndex, grassDensity);
    }
}
