using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;
using static UnityEngine.Rendering.DebugUI;

public class TerrainManager : MonoBehaviour
{
    public static TerrainManager Instance { get; private set; }
    public Terrain terrain; // Assign this in the inspector or via code
    public int detailIndex = 0;
    [SerializeField] private float averageDensity;
    [SerializeField] private float calculationIntervalSeconds;
    private void Awake()
    {
        Instance = this;
    }
    void Start()
    {
        terrain = FindAnyObjectByType<Terrain>();
        SetSplatMap(terrain, 0);
        SetGrass(terrain, 0, 0);
        StartCoroutine(RepeatedCalculationCoroutine());
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
    private IEnumerator RepeatedCalculationCoroutine()
    {
        while (true) // Infinite loop to keep repeating
        {
            yield return StartCoroutine(CalculateTerraformPercentage());

            // Wait for specified interval before repeating
            yield return new WaitForSeconds(calculationIntervalSeconds);
        }
    }
    private IEnumerator CalculateTerraformPercentage()
    {
        TerrainData terrainData = terrain.terrainData;
        float[,,] splatmapData = terrainData.GetAlphamaps(0, 0, terrainData.alphamapWidth, terrainData.alphamapHeight);

        int totalDensity = 0;
        int totalCount = terrainData.alphamapWidth * terrainData.alphamapWidth;

        // Chunk size - number of cells to process per frame
        int chunkSize = 10000; // Adjust this number based on performance needs

        for (int y = 0; y < terrainData.alphamapWidth; y++)
        {
            for (int x = 0; x < terrainData.alphamapHeight; x++)
            {
                totalDensity += Mathf.RoundToInt(splatmapData[x, y, 1]);
                if ((y * terrainData.alphamapWidth + x) % chunkSize == 0)
                {
                    print(totalDensity);
                    yield return null; // Wait for the next frame
                }
            }
        }

        averageDensity = (float)totalDensity / totalCount;
        Debug.Log("Average Grass Density: " + averageDensity);
    }

    public float GetAverageDensity()
    {
        return averageDensity;
    }

}