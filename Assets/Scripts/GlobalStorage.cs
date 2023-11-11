using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GlobalStorage
{
    private static Dictionary<PlacedObjectTypeSO, int> buildingCounts = new Dictionary<PlacedObjectTypeSO, int>();
    public static event EventHandler OnBuildingAmountChanged;

    public static void AddBuilding(PlacedObjectTypeSO buildingType, int amount = 1)
    {
        if (!buildingCounts.ContainsKey(buildingType))
        {
            buildingCounts[buildingType] = 0;
        }
        buildingCounts[buildingType] += amount;
        OnBuildingAmountChanged?.Invoke(null, EventArgs.Empty);
    }
    public static void RemoveBuilding(PlacedObjectTypeSO buildingType, int amount = 1)
    {
        if (!buildingCounts.ContainsKey(buildingType))
        {
            buildingCounts[buildingType] = 0;
        }
        buildingCounts[buildingType] -= amount;
        OnBuildingAmountChanged?.Invoke(null, EventArgs.Empty);
    }

    public static int GetBuildingCount(PlacedObjectTypeSO buildingType)
    {
        return buildingCounts.ContainsKey(buildingType) ? buildingCounts[buildingType] : 0;
    }
}
