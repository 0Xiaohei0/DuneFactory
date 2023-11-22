using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainManager : MonoBehaviour
{
    public Terrain terrain; // Assign this in the inspector or via code
    [SerializeField] private float transitionProgress = 0.0f;
    public float transitionSpeed = 0.1f;
    public Vector3 enrichmentPlantPosition;
    public float plantEffectRadius;
    private void Start()
    {
        UpdateSplatMap(terrain, enrichmentPlantPosition, 10000, 0);
    }

    void Update()
    {
        // Example condition: increment transitionProgress over time
        transitionProgress += Time.deltaTime * transitionSpeed; // transitionSpeed is a variable you define
        transitionProgress = Mathf.Clamp01(transitionProgress); // Ensure value stays between 0 and 1
        UpdateSplatMap(terrain, enrichmentPlantPosition, plantEffectRadius, transitionProgress);
    }
    void UpdateSplatMap(Terrain terrain, Vector3 centerPoint, float radius, float transitionProgress)
    {
        TerrainData terrainData = terrain.terrainData;
        Vector3 terrainPos = terrain.transform.position;

        // Convert centerPoint to terrain local coordinates
        float relativeX = (centerPoint.x - terrainPos.x) / terrainData.size.x;
        float relativeZ = (centerPoint.z - terrainPos.z) / terrainData.size.z;

        // Calculate which part of the splatmap we need to modify
        int startX = (int)((relativeX - radius) * terrainData.alphamapWidth);
        int endX = (int)((relativeX + radius) * terrainData.alphamapWidth);
        int startZ = (int)((relativeZ - radius) * terrainData.alphamapHeight);
        int endZ = (int)((relativeZ + radius) * terrainData.alphamapHeight);

        // Ensure we stay within the bounds of the splatmap array
        startX = Mathf.Clamp(startX, 0, terrainData.alphamapWidth);
        endX = Mathf.Clamp(endX, 0, terrainData.alphamapWidth);
        startZ = Mathf.Clamp(startZ, 0, terrainData.alphamapHeight);
        endZ = Mathf.Clamp(endZ, 0, terrainData.alphamapHeight);

        float[,,] splatmapData = terrainData.GetAlphamaps(startX, startZ, endX - startX, endZ - startZ);

        for (int y = 0; y < (endZ - startZ); y++)
        {
            for (int x = 0; x < (endX - startX); x++)
            {
                // Calculate the distance from the center point within the normalized range [0, 1]
                float distance = Mathf.Sqrt(Mathf.Pow(x / (float)(endX - startX) - relativeX, 2) + Mathf.Pow(y / (float)(endZ - startZ) - relativeZ, 2));
                float lerpFactor = Mathf.Clamp01((radius - distance) / radius);

                // Blend between the current value and the new value based on lerpFactor
                float greenWeight = Mathf.Lerp(splatmapData[x, y, 1], transitionProgress, lerpFactor);

                splatmapData[x, y, 0] = 1 - greenWeight; // Sand
                splatmapData[x, y, 1] = greenWeight; // Green
            }
        }

        // Apply the modified splatmap data
        terrainData.SetAlphamaps(startX, startZ, splatmapData);
    }

}
