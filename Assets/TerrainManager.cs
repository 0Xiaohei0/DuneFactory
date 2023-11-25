using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

public class TerrainManager : MonoBehaviour
{
    public Terrain terrain; // Assign this in the inspector or via code
    void Start()
    {
        terrain = FindAnyObjectByType<Terrain>();
        SetSplatMap(terrain, 0);
        SetGrass(terrain, 0, 0);
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
}
