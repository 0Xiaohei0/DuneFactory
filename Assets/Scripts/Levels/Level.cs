using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class Level : ScriptableObject
{
    public float levelCost;
    public List<PlacedObjectTypeSO> unlockBuildings;
    public List<ItemRecipeSO> unlockItems;
}
