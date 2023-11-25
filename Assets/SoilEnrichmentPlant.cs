using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

public class SoilEnrichmentPlant : GeothermalGenerator
{
    public Terrain terrain; // Assign this in the inspector or via code
    [SerializeField] private float transitionProgress = 0.0f;
    public float transitionSpeed = 0.1f;
    public Vector3 enrichmentPlantPosition;
    public float plantEffectRadius;
    void Start()
    {
        terrain = FindAnyObjectByType<Terrain>();
        enrichmentPlantPosition = new Vector3(transform.position.x + 4, transform.position.y, transform.position.z + 4);
        //SetSplatMap(terrain, 0);
        //SetGrass(terrain, 0, 0);
        OnItemStorageCountChanged += IncrementProgress;
    }

    // Update is called once per frame

    protected override void Update()
    {
        base.Update();
        bool hasEnoughItemsToCraft = HasEnoughItemsToCraft();
        SetLight(hasEnoughItemsToCraft);
        EnergyConsumer energyConsumer = transform.GetComponent<EnergyConsumer>();
        if (energyConsumer != null)
        {
            energyConsumer.isOn = hasEnoughItemsToCraft;
        }
    }

    private void IncrementProgress(object sender, EventArgs e)
    {
        transitionProgress += transitionSpeed; // transitionSpeed is a variable you define
        transitionProgress = Mathf.Clamp01(transitionProgress); // Ensure value stays between 0 and 1
        UpdateTerrainAndGrass(terrain, enrichmentPlantPosition, plantEffectRadius, 0, transitionProgress, 5);
    }


    void UpdateSplatMap(Terrain terrain, Vector3 centerPoint, float radius, float transitionProgress)
    {
        TerrainData terrainData = terrain.terrainData;
        Vector3 terrainPos = terrain.transform.position;
        print("updating splat");
        int centerX = (int)(((centerPoint.x - terrainPos.x) / terrainData.size.x) * terrainData.alphamapWidth);
        int centerY = (int)(((centerPoint.z - terrainPos.z) / terrainData.size.z) * terrainData.alphamapHeight);
        int radiusInDetailMap = (int)(radius / terrainData.size.x * terrainData.alphamapWidth);

        float[,,] splatmapData = terrainData.GetAlphamaps(0, 0, terrainData.alphamapWidth, terrainData.alphamapHeight);

        for (int y = centerY - radiusInDetailMap; y <= centerY + radiusInDetailMap; y++)
        {
            for (int x = centerX - radiusInDetailMap; x <= centerX + radiusInDetailMap; x++)
            {
                if (x < 0 || x >= terrainData.alphamapWidth || y < 0 || y >= terrainData.alphamapHeight)
                    continue;

                int distance = (int)Mathf.Sqrt((x - centerX) * (x - centerX) + (y - centerY) * (y - centerY));
                if (distance < radiusInDetailMap)
                {
                    splatmapData[y, x, 0] = 1 - transitionProgress; // Sand
                    splatmapData[y, x, 1] = transitionProgress; // Green
                }
            }
        }
        terrainData.SetAlphamaps(0, 0, splatmapData);
    }
    void SetSplatMap(Terrain terrain, float value)
    {
        TerrainData terrainData = terrain.terrainData;
        float[,,] splatmapData = terrainData.GetAlphamaps(0, 0, terrainData.alphamapWidth, terrainData.alphamapHeight);
        print("updating splat");
        for (int y = 0; y < terrainData.alphamapWidth; y++)
        {
            for (int x = 0; x < terrainData.alphamapHeight; x++)
            {
                splatmapData[x, y, 0] = 1; // Sand
                splatmapData[x, y, 1] = value; // Green
            }
        }
        terrainData.SetAlphamaps(0, 0, splatmapData);
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
                    detailLayer[y, x] = Mathf.Max(grassDensity, detailLayer[y, x]);
                }
            }
        }

        // Apply the updated detail layer to the terrain
        terrainData.SetDetailLayer(0, 0, grassIndex, detailLayer);
    }
    void UpdateTerrainAndGrass(Terrain terrain, Vector3 centerPoint, float radius, int grassIndex, float transitionProgress, int tickRate)
    {
        if (transitionProgress == 1) { return; }
        // Update splatmap for terrain texture
        UpdateSplatMap(terrain, centerPoint, radius, transitionProgress);

        // Calculate grass density based on transitionProgress
        int grassDensity = Mathf.Max((int)(transitionProgress * 400) - 200, 0); // Scale density with progress

        // Update grass density in the detail layer
        UpdateGrassDensity(terrain, centerPoint, radius, grassIndex, grassDensity);
    }
}
