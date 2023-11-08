using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewCategory", menuName = "Building Category")]
public class BuildingCategorySO : ScriptableObject
{
    public string categoryName; // Name of the category
    public List<PlacedObjectTypeSO> placedObjects; // Objects within this category
    public Sprite icon; // Objects within this category
}